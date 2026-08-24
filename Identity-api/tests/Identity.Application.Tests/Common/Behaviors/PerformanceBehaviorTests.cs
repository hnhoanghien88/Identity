using Identity.Application.Common.Behaviors;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Tests.Common.Behaviors;

public sealed class PerformanceBehaviorTests
{
    [Fact]
    public async Task Handle_returns_response_without_warning_when_request_is_fast()
    {
        var logger = new TestLogger<PerformanceBehavior<TestRequest, string>>();
        var behavior = new PerformanceBehavior<TestRequest, string>(
            new PerformanceOptions { SlowRequestThresholdMilliseconds = 10_000 },
            logger);

        var result = await behavior.Handle(
            new TestRequest(),
            _ => Task.FromResult("handled"),
            CancellationToken.None);

        Assert.Equal("handled", result);
        Assert.Empty(logger.Entries);
    }

    [Fact]
    public async Task Handle_logs_warning_when_request_exceeds_threshold()
    {
        var logger = new TestLogger<PerformanceBehavior<TestRequest, string>>();
        var behavior = new PerformanceBehavior<TestRequest, string>(
            new PerformanceOptions { SlowRequestThresholdMilliseconds = 1 },
            logger);

        await behavior.Handle(
            new TestRequest(),
            async cancellationToken =>
            {
                await Task.Delay(25, cancellationToken);
                return "handled";
            },
            CancellationToken.None);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
        Assert.Contains(nameof(TestRequest), entry.Message);
        Assert.Contains("threshold: 1 ms", entry.Message);
    }

    private sealed record TestRequest;

    private sealed class TestLogger<T> : ILogger<T>
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter) =>
            Entries.Add((logLevel, formatter(state, exception)));
    }
}
