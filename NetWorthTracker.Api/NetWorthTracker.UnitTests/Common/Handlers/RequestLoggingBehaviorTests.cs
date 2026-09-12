using FluentResults;
using Mediator;
using Microsoft.Extensions.Logging;
using NetWorthTracker.Application.Common.Handlers;

namespace NetWorthTracker.UnitTests.Common.Handlers;

public class RequestLoggingBehaviorTests
{
    [Test]
    public async Task GivenSuccessfulResult_WhenHandlingMessage_ThenForwardsMessageAndLogsSuccess()
    {
        // Arrange
        var message = new TestMessage<Result<string>>("request data");
        var cancellationToken = new CancellationTokenSource().Token;
        var expectedResponse = Result.Ok("response data");
        var logger = new TestLogger<RequestLoggingBehavior<TestMessage<Result<string>>, Result<string>>>();
        var behavior = new RequestLoggingBehavior<TestMessage<Result<string>>, Result<string>>(logger);
        TestMessage<Result<string>>? forwardedMessage = null;
        CancellationToken forwardedCancellationToken = default;
        MessageHandlerDelegate<TestMessage<Result<string>>, Result<string>> next =
            (handledMessage, handledCancellationToken) =>
            {
                forwardedMessage = handledMessage;
                forwardedCancellationToken = handledCancellationToken;
                return ValueTask.FromResult(expectedResponse);
            };

        // Act
        var response = await behavior.Handle(message, next, cancellationToken);

        // Assert
        await Assert.That(response).IsEqualTo(expectedResponse);
        await Assert.That(forwardedMessage).IsEqualTo(message);
        await Assert.That(forwardedCancellationToken).IsEqualTo(cancellationToken);
        await Assert.That(logger.Entries).Count().IsEqualTo(1);

        var completionEntry = logger.Entries[0];
        await Assert.That(completionEntry.LogLevel).IsEqualTo(LogLevel.Information);
        await Assert.That(completionEntry.Properties["RequestType"]).IsEqualTo(typeof(TestMessage<Result<string>>).Name);
    }

    [Test]
    public async Task GivenFailedResult_WhenHandlingMessage_ThenLogsErrors()
    {
        // Arrange
        var message = new TestMessage<Result<string>>("request data");
        var expectedResponse = Result.Fail<string>("First error").WithError("Second error");
        var logger = new TestLogger<RequestLoggingBehavior<TestMessage<Result<string>>, Result<string>>>();
        var behavior = new RequestLoggingBehavior<TestMessage<Result<string>>, Result<string>>(logger);
        MessageHandlerDelegate<TestMessage<Result<string>>, Result<string>> next =
            (handledMessage, cancellationToken) => ValueTask.FromResult(expectedResponse);

        // Act
        var response = await behavior.Handle(message, next, CancellationToken.None);

        // Assert
        await Assert.That(response).IsEqualTo(expectedResponse);
        await Assert.That(logger.Entries).Count().IsEqualTo(1);

        var failureEntry = logger.Entries[0];
        await Assert.That(failureEntry.LogLevel).IsEqualTo(LogLevel.Error);
        await Assert.That(failureEntry.Properties["RequestType"]).IsEqualTo(typeof(TestMessage<Result<string>>).Name);
        await Assert.That(failureEntry.Properties["Errors"]).IsEqualTo("First error, Second error");
    }

    [Test]
    public async Task GivenNonResultResponse_WhenHandlingMessage_ThenDoesNotLog()
    {
        // Arrange
        var message = new TestMessage<string>("request data");
        const string expectedResponse = "response data";
        var logger = new TestLogger<RequestLoggingBehavior<TestMessage<string>, string>>();
        var behavior = new RequestLoggingBehavior<TestMessage<string>, string>(logger);
        MessageHandlerDelegate<TestMessage<string>, string> next =
            (handledMessage, cancellationToken) => ValueTask.FromResult(expectedResponse);

        // Act
        var response = await behavior.Handle(message, next, CancellationToken.None);

        // Assert
        await Assert.That(response).IsEqualTo(expectedResponse);
        await Assert.That(logger.Entries).IsEmpty();
    }

    private sealed record TestMessage<TResponse>(string Data) : IRequest<TResponse>;

    private sealed class TestLogger<T> : ILogger<T>
    {
        public List<LogEntry> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            var properties = state is IEnumerable<KeyValuePair<string, object?>> values
                ? values.ToDictionary(pair => pair.Key, pair => pair.Value)
                : [];

            Entries.Add(new LogEntry(logLevel, properties));
        }
    }

    private sealed record LogEntry(
        LogLevel LogLevel,
        IReadOnlyDictionary<string, object?> Properties);
}
