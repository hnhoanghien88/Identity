using Identity.Api.Middleware;

namespace Identity.Api.IntegrationTests;

public sealed class RateLimitCounterStoreTests
{
    [Theory]
    [InlineData("FixedWindow")]
    [InlineData("SlidingWindow")]
    [InlineData("TokenBucket")]
    public async Task Acquire_rejects_requests_after_capacity_is_exhausted(string algorithm)
    {
        var store = new InMemoryRateLimitStore();
        var key = $"test:{algorithm}";

        Assert.True((await Acquire(store, key, algorithm, 2)).Acquired);
        Assert.True((await Acquire(store, key, algorithm, 2)).Acquired);
        var rejected = await Acquire(store, key, algorithm, 2);

        Assert.False(rejected.Acquired);
        Assert.Equal(0, rejected.Remaining);
        Assert.True(rejected.ResetAt >= DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }

    [Fact]
    public async Task Burst_limit_increases_initial_capacity()
    {
        var store = new InMemoryRateLimitStore();

        Assert.True((await Acquire(store, "burst", "TokenBucket", 1, 2)).Acquired);
        Assert.True((await Acquire(store, "burst", "TokenBucket", 1, 2)).Acquired);
        Assert.True((await Acquire(store, "burst", "TokenBucket", 1, 2)).Acquired);
        Assert.False((await Acquire(store, "burst", "TokenBucket", 1, 2)).Acquired);
    }

    [Fact]
    public async Task Concurrency_releases_capacity_when_request_finishes()
    {
        var store = new InMemoryRateLimitStore();
        var first = await Acquire(store, "concurrency", "Concurrency", 1);

        Assert.True(first.Acquired);
        Assert.False((await Acquire(store, "concurrency", "Concurrency", 1)).Acquired);

        await store.ReleaseAsync(first, CancellationToken.None);

        Assert.True((await Acquire(store, "concurrency", "Concurrency", 1)).Acquired);
    }

    [Fact]
    public async Task Unsupported_algorithm_is_rejected_instead_of_falling_back()
    {
        var store = new InMemoryRateLimitStore();

        await Assert.ThrowsAsync<ArgumentException>(() => Acquire(store, "invalid", "Unknown", 1));
    }

    private static Task<RateLimitLease> Acquire(
        IRateLimitStore store,
        string key,
        string algorithm,
        int permitLimit,
        int burstLimit = 0) =>
        store.AcquireAsync(key, algorithm, permitLimit, 60, burstLimit, CancellationToken.None);
}
