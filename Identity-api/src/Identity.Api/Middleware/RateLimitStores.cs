using System.Collections.Concurrent;
using StackExchange.Redis;

namespace Identity.Api.Middleware;

public sealed class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";
    public bool Enabled { get; set; } = true;
    public string Store { get; set; } = "InMemory";
    public string FailureMode { get; set; } = "Open";
    public string KeyPrefix { get; set; } = "identity:rl";
    public int PolicyCacheSeconds { get; set; } = 30;
    public bool FailOpen => string.Equals(FailureMode, "Open", StringComparison.OrdinalIgnoreCase);
}

public sealed record RateLimitLease(
    bool Acquired,
    string Key,
    string Algorithm,
    string? LeaseId,
    int Limit,
    int Remaining,
    long ResetAt)
{
    public bool RequiresRelease => Algorithm == RateLimitAlgorithms.Concurrency && LeaseId is not null;
}

public static class RateLimitAlgorithms
{
    public const string FixedWindow = "FixedWindow";
    public const string SlidingWindow = "SlidingWindow";
    public const string TokenBucket = "TokenBucket";
    public const string Concurrency = "Concurrency";

    public static bool IsSupported(string algorithm) => algorithm is
        FixedWindow or SlidingWindow or TokenBucket or Concurrency;
}

public interface IRateLimitStore
{
    Task<RateLimitLease> AcquireAsync(
        string key,
        string algorithm,
        int permitLimit,
        int windowSeconds,
        int burstLimit,
        CancellationToken cancellationToken);

    Task ReleaseAsync(RateLimitLease lease, CancellationToken cancellationToken);
}

public sealed class InMemoryRateLimitStore : IRateLimitStore
{
    private sealed class CounterState
    {
        public required string Algorithm { get; init; }
        public long FixedWindowStartedAt { get; set; }
        public int FixedWindowCount { get; set; }
        public Queue<long> SlidingHits { get; } = new();
        public double Tokens { get; set; }
        public long LastRefillAtMilliseconds { get; set; }
        public Dictionary<string, long> ConcurrencyLeases { get; } = new();
    }

    private readonly ConcurrentDictionary<string, CounterState> _states = new();

    public Task<RateLimitLease> AcquireAsync(
        string key,
        string algorithm,
        int permitLimit,
        int windowSeconds,
        int burstLimit,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!RateLimitAlgorithms.IsSupported(algorithm))
            throw new ArgumentException($"Unsupported rate limit algorithm '{algorithm}'.", nameof(algorithm));

        var capacity = checked(permitLimit + burstLimit);
        var now = DateTimeOffset.UtcNow;
        var state = _states.AddOrUpdate(
            key,
            _ => CreateState(algorithm, capacity, now),
            (_, current) => current.Algorithm == algorithm ? current : CreateState(algorithm, capacity, now));

        lock (state)
        {
            var lease = algorithm switch
            {
                RateLimitAlgorithms.TokenBucket => HitTokenBucket(key, state, permitLimit, capacity, windowSeconds, now),
                RateLimitAlgorithms.SlidingWindow => HitSlidingWindow(key, state, capacity, windowSeconds, now),
                RateLimitAlgorithms.Concurrency => AcquireConcurrency(key, state, capacity, windowSeconds, now),
                _ => HitFixedWindow(key, state, capacity, windowSeconds, now)
            };
            return Task.FromResult(lease);
        }
    }

    public Task ReleaseAsync(RateLimitLease lease, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (lease.RequiresRelease && _states.TryGetValue(lease.Key, out var state))
        {
            lock (state) state.ConcurrencyLeases.Remove(lease.LeaseId!);
        }
        return Task.CompletedTask;
    }

    private static CounterState CreateState(string algorithm, int capacity, DateTimeOffset now) => new()
    {
        Algorithm = algorithm,
        FixedWindowStartedAt = now.ToUnixTimeSeconds(),
        Tokens = capacity,
        LastRefillAtMilliseconds = now.ToUnixTimeMilliseconds()
    };

    private static RateLimitLease HitFixedWindow(string key, CounterState state, int capacity, int windowSeconds, DateTimeOffset now)
    {
        var nowSeconds = now.ToUnixTimeSeconds();
        if (nowSeconds - state.FixedWindowStartedAt >= windowSeconds)
        {
            state.FixedWindowStartedAt = nowSeconds;
            state.FixedWindowCount = 0;
        }
        state.FixedWindowCount++;
        return NewLease(state.FixedWindowCount <= capacity, key, RateLimitAlgorithms.FixedWindow, null,
            capacity, capacity - state.FixedWindowCount, state.FixedWindowStartedAt + windowSeconds);
    }

    private static RateLimitLease HitSlidingWindow(string key, CounterState state, int capacity, int windowSeconds, DateTimeOffset now)
    {
        var nowSeconds = now.ToUnixTimeSeconds();
        while (state.SlidingHits.TryPeek(out var hit) && hit <= nowSeconds - windowSeconds)
            state.SlidingHits.Dequeue();
        var allowed = state.SlidingHits.Count < capacity;
        if (allowed) state.SlidingHits.Enqueue(nowSeconds);
        var resetAt = state.SlidingHits.TryPeek(out var oldest) ? oldest + windowSeconds : nowSeconds + windowSeconds;
        return NewLease(allowed, key, RateLimitAlgorithms.SlidingWindow, null,
            capacity, capacity - state.SlidingHits.Count, resetAt);
    }

    private static RateLimitLease HitTokenBucket(
        string key, CounterState state, int permitLimit, int capacity, int windowSeconds, DateTimeOffset now)
    {
        var nowMilliseconds = now.ToUnixTimeMilliseconds();
        var elapsedSeconds = Math.Max(0, nowMilliseconds - state.LastRefillAtMilliseconds) / 1000d;
        state.Tokens = Math.Min(capacity, state.Tokens + elapsedSeconds * permitLimit / windowSeconds);
        state.LastRefillAtMilliseconds = nowMilliseconds;
        var allowed = state.Tokens >= 1d;
        if (allowed) state.Tokens -= 1d;
        var secondsPerToken = (double)windowSeconds / permitLimit;
        var resetAt = now.AddSeconds(allowed ? secondsPerToken : Math.Max(0, (1d - state.Tokens) * secondsPerToken)).ToUnixTimeSeconds();
        return NewLease(allowed, key, RateLimitAlgorithms.TokenBucket, null,
            capacity, (int)Math.Floor(state.Tokens), resetAt);
    }

    private static RateLimitLease AcquireConcurrency(
        string key, CounterState state, int capacity, int leaseTimeoutSeconds, DateTimeOffset now)
    {
        var nowMilliseconds = now.ToUnixTimeMilliseconds();
        foreach (var expired in state.ConcurrencyLeases.Where(x => x.Value <= nowMilliseconds).Select(x => x.Key).ToArray())
            state.ConcurrencyLeases.Remove(expired);
        if (state.ConcurrencyLeases.Count >= capacity)
        {
            var resetAt = state.ConcurrencyLeases.Values.Min() / 1000;
            return NewLease(false, key, RateLimitAlgorithms.Concurrency, null,
                capacity, 0, resetAt);
        }
        var leaseId = Guid.NewGuid().ToString("N");
        state.ConcurrencyLeases[leaseId] = now.AddSeconds(leaseTimeoutSeconds).ToUnixTimeMilliseconds();
        return NewLease(true, key, RateLimitAlgorithms.Concurrency, leaseId,
            capacity, capacity - state.ConcurrencyLeases.Count, now.AddSeconds(leaseTimeoutSeconds).ToUnixTimeSeconds());
    }

    private static RateLimitLease NewLease(
        bool allowed, string key, string algorithm, string? leaseId, int limit, int remaining, long resetAt) =>
        new(allowed, key, algorithm, leaseId, limit, Math.Max(0, remaining), resetAt);
}

public sealed class RedisRateLimitStore(IConnectionMultiplexer connection) : IRateLimitStore
{
    private const string AcquireScript = """
        local key = KEYS[1]
        local algorithm = ARGV[1]
        local permit = tonumber(ARGV[2])
        local window = tonumber(ARGV[3])
        local burst = tonumber(ARGV[4])
        local now = tonumber(ARGV[5])
        local leaseId = ARGV[6]
        local capacity = permit + burst

        if algorithm == 'FixedWindow' then
          local count = redis.call('INCR', key)
          if count == 1 then redis.call('PEXPIRE', key, window) end
          local ttl = redis.call('PTTL', key)
          return {count <= capacity and 1 or 0, math.max(0, capacity - count), now + ttl, ''}
        end

        if algorithm == 'SlidingWindow' then
          redis.call('ZREMRANGEBYSCORE', key, '-inf', now - window)
          local count = redis.call('ZCARD', key)
          local allowed = 0
          if count < capacity then
            redis.call('ZADD', key, now, leaseId)
            count = count + 1
            allowed = 1
          end
          redis.call('PEXPIRE', key, window)
          local oldest = redis.call('ZRANGE', key, 0, 0, 'WITHSCORES')
          local reset = now + window
          if #oldest > 0 then reset = tonumber(oldest[2]) + window end
          return {allowed, math.max(0, capacity - count), reset, ''}
        end

        if algorithm == 'TokenBucket' then
          local values = redis.call('HMGET', key, 'tokens', 'last')
          local tokens = tonumber(values[1]) or capacity
          local last = tonumber(values[2]) or now
          tokens = math.min(capacity, tokens + math.max(0, now - last) * permit / window)
          local allowed = 0
          if tokens >= 1 then tokens = tokens - 1; allowed = 1 end
          redis.call('HSET', key, 'tokens', tokens, 'last', now)
          redis.call('PEXPIRE', key, window * 2)
          local wait = allowed == 1 and (window / permit) or ((1 - tokens) * window / permit)
          return {allowed, math.floor(tokens), now + math.max(0, wait), ''}
        end

        if algorithm == 'Concurrency' then
          redis.call('ZREMRANGEBYSCORE', key, '-inf', now)
          local count = redis.call('ZCARD', key)
          if count >= capacity then
            local oldest = redis.call('ZRANGE', key, 0, 0, 'WITHSCORES')
            return {0, 0, tonumber(oldest[2]), ''}
          end
          local expires = now + window
          redis.call('ZADD', key, expires, leaseId)
          redis.call('PEXPIRE', key, window)
          return {1, capacity - count - 1, expires, leaseId}
        end

        return redis.error_reply('Unsupported rate limit algorithm: ' .. algorithm)
        """;

    private const string ReleaseScript = """
        return redis.call('ZREM', KEYS[1], ARGV[1])
        """;

    public async Task<RateLimitLease> AcquireAsync(
        string key,
        string algorithm,
        int permitLimit,
        int windowSeconds,
        int burstLimit,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!RateLimitAlgorithms.IsSupported(algorithm))
            throw new ArgumentException($"Unsupported rate limit algorithm '{algorithm}'.", nameof(algorithm));

        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var requestId = Guid.NewGuid().ToString("N");
        var result = (RedisResult[]?)await connection.GetDatabase().ScriptEvaluateAsync(
            AcquireScript,
            [(RedisKey)key],
            [algorithm, permitLimit, checked(windowSeconds * 1000), burstLimit, now, requestId]);

        if (result is null || result.Length != 4)
            throw new RedisException("The rate limit script returned an invalid result.");

        var acquired = (long)result[0] == 1;
        var remaining = checked((int)(long)result[1]);
        var resetAt = (long)result[2] / 1000;
        var leaseId = result[3].ToString();
        return new RateLimitLease(
            acquired,
            key,
            algorithm,
            string.IsNullOrEmpty(leaseId) ? null : leaseId,
            checked(permitLimit + burstLimit),
            remaining,
            resetAt);
    }

    public async Task ReleaseAsync(RateLimitLease lease, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!lease.RequiresRelease) return;
        await connection.GetDatabase().ScriptEvaluateAsync(
            ReleaseScript,
            [(RedisKey)lease.Key],
            [lease.LeaseId!]);
    }
}
