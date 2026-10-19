using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetWorthTracker.Api.Middleware;
using NetWorthTracker.Api.Services;

namespace NetWorthTracker.UnitTests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    [Test]
    public async Task GivenArgumentException_WhenInvokingMiddleware_ThenReturnsGenericInternalServerError()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var logger = new TestLogger<ExceptionHandlingMiddleware>();
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new ArgumentException("The username is invalid.", "username"),
            logger,
            new ExceptionResponseMapper());

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var problemDetails = await ReadProblemDetails(context.Response.Body);
        await Assert.That(context.Response.StatusCode).IsEqualTo(StatusCodes.Status500InternalServerError);
        await Assert.That(context.Response.ContentType).IsEqualTo("application/problem+json");
        await Assert.That(problemDetails.GetProperty("status").GetInt32()).IsEqualTo(StatusCodes.Status500InternalServerError);
        await Assert.That(problemDetails.GetProperty("title").GetString())
            .IsEqualTo("An unexpected error occurred while processing the request.");
        await Assert.That(problemDetails.GetProperty("detail").GetString())
            .IsEqualTo("An unexpected error occurred while processing the request.");
        await Assert.That(logger.Entries.Single().Exception).IsTypeOf<ArgumentException>();
    }

    [Test]
    public async Task GivenValidationException_WhenInvokingMiddleware_ThenReturnsGenericInternalServerError()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var logger = new TestLogger<ExceptionHandlingMiddleware>();
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new ValidationException("The request is invalid."),
            logger,
            new ExceptionResponseMapper());

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        await Assert.That(context.Response.StatusCode).IsEqualTo(StatusCodes.Status500InternalServerError);
        var problemDetails = await ReadProblemDetails(context.Response.Body);
        await Assert.That(problemDetails.GetProperty("title").GetString())
            .IsEqualTo("An unexpected error occurred while processing the request.");
    }

    [Test]
    public async Task GivenUnknownException_WhenInvokingMiddleware_ThenReturnsInternalServerErrorWithoutExceptionDetails()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var logger = new TestLogger<ExceptionHandlingMiddleware>();
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new InvalidOperationException("Sensitive implementation detail."),
            logger,
            new ExceptionResponseMapper());

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var problemDetails = await ReadProblemDetails(context.Response.Body);
        await Assert.That(context.Response.StatusCode).IsEqualTo(StatusCodes.Status500InternalServerError);
        await Assert.That(problemDetails.GetProperty("detail").GetString()).DoesNotContain("Sensitive implementation detail.");
        await Assert.That(logger.Entries.Single().Exception).IsTypeOf<InvalidOperationException>();
    }

    private static async Task<JsonElement> ReadProblemDetails(Stream body)
    {
        body.Position = 0;
        using var document = await JsonDocument.ParseAsync(body);
        return document.RootElement.Clone();
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
            Entries.Add(new LogEntry(logLevel, exception));
        }
    }

    private sealed record LogEntry(LogLevel LogLevel, Exception? Exception);
}
