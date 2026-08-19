using System.Collections.Concurrent;
using System.Security.Claims;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Identity.Api.Middleware;

public sealed class RateLimitPolicyProvider(IdentityDbContext db, IMemoryCache cache)
{
    private const string CacheKey = "rate-limit-policies";
    public Task<List<RateLimitPolicy>> GetAsync(CancellationToken token) => cache.GetOrCreateAsync(CacheKey, entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
        return db.RateLimitPolicies.AsNoTracking().Where(x => x.IsActive && !x.IsDeleted).OrderByDescending(x => x.Priority).ToListAsync(token);
    })!;
    public void Invalidate() => cache.Remove(CacheKey);
}

public sealed class RateLimitCounterStore
{
    private sealed class CounterState
    {
        public required string Algorithm { get; init; }
        public long FixedWindowStartedAt { get; set; }
        public int FixedWindowCount { get; set; }
        public Queue<long> SlidingHits { get; } = new();
        public double Tokens { get; set; }
        public long LastRefillAtMilliseconds { get; set; }
    }

    private readonly ConcurrentDictionary<string, CounterState> _states = new();

    public (bool Allowed, int Remaining, long ResetAt) Hit(
        string key,
        string algorithm,
        int permitLimit,
        int windowSeconds,
        int burstLimit)
    {
        var capacity = checked(permitLimit + burstLimit);
        var now = DateTimeOffset.UtcNow;
        var state = _states.AddOrUpdate(
            key,
            _ => CreateState(algorithm, capacity, now),
            (_, current) => string.Equals(current.Algorithm, algorithm, StringComparison.Ordinal)
                ? current
                : CreateState(algorithm, capacity, now));

        lock (state)
        {
            return algorithm switch
            {
                "TokenBucket" => HitTokenBucket(state, permitLimit, capacity, windowSeconds, now),
                "SlidingWindow" => HitSlidingWindow(state, capacity, windowSeconds, now),
                _ => HitFixedWindow(state, capacity, windowSeconds, now),
            };
        }
    }

    private static CounterState CreateState(string algorithm, int capacity, DateTimeOffset now) => new()
    {
        Algorithm = algorithm,
        FixedWindowStartedAt = now.ToUnixTimeSeconds(),
        Tokens = capacity,
        LastRefillAtMilliseconds = now.ToUnixTimeMilliseconds(),
    };

    private static (bool Allowed, int Remaining, long ResetAt) HitFixedWindow(
        CounterState state, int capacity, int windowSeconds, DateTimeOffset now)
    {
        var nowSeconds = now.ToUnixTimeSeconds();
        if (nowSeconds - state.FixedWindowStartedAt >= windowSeconds)
        {
            state.FixedWindowStartedAt = nowSeconds;
            state.FixedWindowCount = 0;
        }
        state.FixedWindowCount++;
        return (
            state.FixedWindowCount <= capacity,
            Math.Max(0, capacity - state.FixedWindowCount),
            state.FixedWindowStartedAt + windowSeconds);
    }

    private static (bool Allowed, int Remaining, long ResetAt) HitSlidingWindow(
        CounterState state, int capacity, int windowSeconds, DateTimeOffset now)
    {
        var nowSeconds = now.ToUnixTimeSeconds();
        var cutoff = nowSeconds - windowSeconds;
        while (state.SlidingHits.TryPeek(out var hit) && hit <= cutoff)
            state.SlidingHits.Dequeue();

        var allowed = state.SlidingHits.Count < capacity;
        if (allowed) state.SlidingHits.Enqueue(nowSeconds);
        var resetAt = state.SlidingHits.TryPeek(out var oldest)
            ? oldest + windowSeconds
            : nowSeconds + windowSeconds;
        return (allowed, Math.Max(0, capacity - state.SlidingHits.Count), resetAt);
    }

    private static (bool Allowed, int Remaining, long ResetAt) HitTokenBucket(
        CounterState state, int permitLimit, int capacity, int windowSeconds, DateTimeOffset now)
    {
        var nowMilliseconds = now.ToUnixTimeMilliseconds();
        var elapsedSeconds = Math.Max(0, nowMilliseconds - state.LastRefillAtMilliseconds) / 1000d;
        state.Tokens = Math.Min(capacity, state.Tokens + elapsedSeconds * permitLimit / windowSeconds);
        state.LastRefillAtMilliseconds = nowMilliseconds;
        var allowed = state.Tokens >= 1d;
        if (allowed) state.Tokens -= 1d;
        var secondsPerToken = (double)windowSeconds / permitLimit;
        var resetAt = now.AddSeconds(allowed ? secondsPerToken : Math.Max(0, (1d - state.Tokens) * secondsPerToken)).ToUnixTimeSeconds();
        return (allowed, Math.Max(0, (int)Math.Floor(state.Tokens)), resetAt);
    }
}
public sealed class DynamicRateLimitMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, RateLimitPolicyProvider provider, RateLimitCounterStore counters, IOptions<Identity.Api.Authentication.JwtOptions> jwt)
    {
        var route = (context.GetEndpoint() as RouteEndpoint)?.RoutePattern.RawText?.TrimStart('/');
        if (string.IsNullOrWhiteSpace(route)) { await next(context); return; }
        var method = context.Request.Method;
        var policies = await provider.GetAsync(context.RequestAborted);
        foreach (var policy in policies.Where(x => Matches(x, route, method)))
        {
            var key = BuildKey(policy, context, route, jwt.Value.ApplicationCode);
            var permitLimit = checked((int)policy.PermitLimit);
            var burstLimit = checked((int)(policy.BurstLimit ?? 0));
            var limit = checked(permitLimit + burstLimit);
            var result = counters.Hit(key, policy.Algorithm, permitLimit, checked((int)policy.WindowSeconds), burstLimit);
            context.Response.Headers["RateLimit-Limit"] = limit.ToString();
            context.Response.Headers["RateLimit-Remaining"] = result.Remaining.ToString();
            context.Response.Headers["RateLimit-Reset"] = result.ResetAt.ToString();
            if (result.Allowed) continue;
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers.RetryAfter = Math.Max(1, result.ResetAt - DateTimeOffset.UtcNow.ToUnixTimeSeconds()).ToString();
            await context.Response.WriteAsJsonAsync(new { title = "Too many requests", status = 429, detail = $"Rate limit policy '{policy.Name}' was exceeded." });
            return;
        }
        await next(context);
    }

    private static bool Matches(RateLimitPolicy policy, string route, string method)
    {
        var methods = policy.HttpMethods?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (methods is { Length: > 0 } && !methods.Contains(method, StringComparer.OrdinalIgnoreCase)) return false;
        var pattern = policy.RoutePattern.TrimStart('/');
        return pattern == "*" || (pattern.EndsWith("/**") && route.StartsWith(pattern[..^3], StringComparison.OrdinalIgnoreCase)) || string.Equals(pattern, route, StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildKey(RateLimitPolicy policy, HttpContext context, string route, string application)
    {
        var dimensions = policy.PartitionBy.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var parts = new List<string> { policy.Id.ToString() };
        foreach (var dimension in dimensions)
            parts.Add(dimension switch
            {
                "User" => context.User.FindFirstValue("uid") ?? "anonymous",
                "IpAddress" => context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                "Application" => policy.ApplicationId?.ToString() ?? application,
                "Endpoint" => $"{context.Request.Method}:{route}",
                _ => dimension
            });
        return string.Join(':', parts);
    }
}