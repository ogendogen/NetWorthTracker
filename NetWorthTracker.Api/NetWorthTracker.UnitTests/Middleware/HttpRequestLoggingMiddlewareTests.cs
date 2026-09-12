using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetWorthTracker.Api.Middleware;

namespace NetWorthTracker.UnitTests.Middleware;

public class HttpRequestLoggingMiddlewareTests
{
    [Test]
    public async Task GivenCompletedRequest_WhenInvokingMiddleware_ThenLogsResponseDetails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/auth/login";
        var logger = new TestLogger<HttpRequestLoggingMiddleware>();
        var middleware = new HttpRequestLoggingMiddleware(
            httpContext =>
            {
                httpContext.Response.StatusCode = StatusCodes.Status201Created;
                return Task.CompletedTask;
            },
            logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var entry = logger.Entries.Single();
        await Assert.That(entry.LogLevel).IsEqualTo(LogLevel.Information);
        await Assert.That(entry.Exception).IsNull();
        await Assert.That(entry.Properties["RequestMethod"]).IsEqualTo(HttpMethods.Post);
        await Assert.That(entry.Properties["RequestPath"]).IsEqualTo("/auth/login");
        await Assert.That(entry.Properties["StatusCode"]).IsEqualTo(StatusCodes.Status201Created);
        await Assert.That((double)entry.Properties["ElapsedMilliseconds"]!).IsGreaterThanOrEqualTo(0);
    }

    [Test]
    public async Task GivenUnhandledException_WhenInvokingMiddleware_ThenLogsFailureAndRethrows()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/data";
        var expectedException = new InvalidOperationException("Request failed.");
        var logger = new TestLogger<HttpRequestLoggingMiddleware>();
        var middleware = new HttpRequestLoggingMiddleware(
            _ => throw expectedException,
            logger);
        Exception? thrownException = null;

        // Act
        try
        {
            await middleware.InvokeAsync(context);
        }
        catch (Exception exception)
        {
            thrownException = exception;
        }

        // Assert
        await Assert.That(thrownException).IsEqualTo(expectedException);
        var entry = logger.Entries.Single();
        await Assert.That(entry.LogLevel).IsEqualTo(LogLevel.Error);
        await Assert.That(entry.Exception).IsEqualTo(expectedException);
        await Assert.That(entry.Properties["RequestMethod"]).IsEqualTo(HttpMethods.Get);
        await Assert.That(entry.Properties["RequestPath"]).IsEqualTo("/data");
        await Assert.That(entry.Properties["StatusCode"]).IsEqualTo(StatusCodes.Status500InternalServerError);
        await Assert.That((double)entry.Properties["ElapsedMilliseconds"]!).IsGreaterThanOrEqualTo(0);
    }

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

            Entries.Add(new LogEntry(logLevel, exception, properties));
        }
    }

    private sealed record LogEntry(
        LogLevel LogLevel,
        Exception? Exception,
        IReadOnlyDictionary<string, object?> Properties);
}
