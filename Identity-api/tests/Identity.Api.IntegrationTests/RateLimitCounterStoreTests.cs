using Identity.Api.Middleware;

namespace Identity.Api.IntegrationTests;

public sealed class RateLimitCounterStoreTests
{
    [Theory]
    [InlineData("FixedWindow")]
    [InlineData("SlidingWindow")]
    [InlineData("TokenBucket")]
    public void Hit_rejects_requests_after_capacity_is_exhausted(string algorithm)
    {
        var store = new RateLimitCounterStore();
        var key = $"test:{algorithm}";

        Assert.True(store.Hit(key, algorithm, 2, 60, 0).Allowed);
        Assert.True(store.Hit(key, algorithm, 2, 60, 0).Allowed);
        var rejected = store.Hit(key, algorithm, 2, 60, 0);

        Assert.False(rejected.Allowed);
        Assert.Equal(0, rejected.Remaining);
        Assert.True(rejected.ResetAt >= DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }

    [Fact]
    public void Burst_limit_increases_initial_capacity()
    {
        var store = new RateLimitCounterStore();

        Assert.True(store.Hit("burst", "TokenBucket", 1, 60, 2).Allowed);
        Assert.True(store.Hit("burst", "TokenBucket", 1, 60, 2).Allowed);
        Assert.True(store.Hit("burst", "TokenBucket", 1, 60, 2).Allowed);
        Assert.False(store.Hit("burst", "TokenBucket", 1, 60, 2).Allowed);
    }
}