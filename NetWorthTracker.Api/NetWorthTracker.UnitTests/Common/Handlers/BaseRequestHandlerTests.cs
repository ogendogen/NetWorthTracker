using System.Text.Json.Nodes;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetWorthTracker.Application.Common.Handlers;

namespace NetWorthTracker.UnitTests.Common.Handlers;

public class BaseRequestHandlerTests
{
    [Test]
    public async Task GivenNoValidators_WhenHandlingRequest_ThenInvokesHandlerAndReturnsItsResult()
    {
        // Arrange
        var request = CreateRequest();
        var cancellationToken = new CancellationTokenSource().Token;
        var expectedResult = Result.Ok("response data");
        var logger = new TestLogger<BaseRequestHandler<TestRequest, string>>();
        var handler = new TestRequestHandler(null, logger, expectedResult);

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        await Assert.That(result).IsEqualTo(expectedResult);
        await Assert.That(handler.CallCount).IsEqualTo(1);
        await Assert.That(handler.HandledRequest).IsEqualTo(request);
        await Assert.That(handler.CancellationToken).IsEqualTo(cancellationToken);
    }

    [Test]
    public async Task GivenValidRequest_WhenHandlingRequest_ThenRunsValidatorAndInvokesHandler()
    {
        // Arrange
        var request = CreateRequest();
        var cancellationToken = new CancellationTokenSource().Token;
        var validator = new TrackingValidator();
        using var services = new ServiceCollection()
            .AddSingleton<IValidator<TestRequest>>(validator)
            .BuildServiceProvider();
        var logger = new TestLogger<BaseRequestHandler<TestRequest, string>>();
        var handler = new TestRequestHandler(services, logger, Result.Ok("response data"));

        // Act
        var result = await handler.Handle(request, cancellationToken);

        // Assert
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(validator.CallCount).IsEqualTo(1);
        await Assert.That(validator.CancellationToken).IsEqualTo(cancellationToken);
        await Assert.That(handler.CallCount).IsEqualTo(1);
    }

    [Test]
    public async Task GivenInvalidRequest_WhenHandlingRequest_ThenReturnsValidationErrorsWithoutInvokingHandler()
    {
        // Arrange
        const string firstError = "Data is required.";
        const string secondError = "Data is invalid.";
        var request = CreateRequest(data: string.Empty);
        var firstValidator = new InlineValidator<TestRequest>();
        firstValidator.RuleFor(candidate => candidate.Data).NotEmpty().WithMessage(firstError);
        var secondValidator = new InlineValidator<TestRequest>();
        secondValidator.RuleFor(candidate => candidate.Data).NotEmpty().WithMessage(secondError);
        using var services = new ServiceCollection()
            .AddSingleton<IValidator<TestRequest>>(firstValidator)
            .AddSingleton<IValidator<TestRequest>>(secondValidator)
            .BuildServiceProvider();
        var logger = new TestLogger<BaseRequestHandler<TestRequest, string>>();
        var handler = new TestRequestHandler(services, logger, Result.Ok("response data"));

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        var errorMessages = result.Errors.Select(error => error.Message).ToArray();
        await Assert.That(result.IsFailed).IsTrue();
        await Assert.That(errorMessages.Contains("Validation failed")).IsTrue();
        await Assert.That(errorMessages.Contains(firstError)).IsTrue();
        await Assert.That(errorMessages.Contains(secondError)).IsTrue();
        await Assert.That(handler.CallCount).IsEqualTo(0);

        var warningEntry = logger.Entries.Single(entry => entry.LogLevel == LogLevel.Warning);
        await Assert.That(warningEntry.Properties["RequestType"]).IsEqualTo(typeof(TestRequest).Name);
    }

    [Test]
    public async Task GivenSensitiveProperties_WhenHandlingRequest_ThenRedactsThemRecursivelyInLogData()
    {
        // Arrange
        var request = CreateRequest();
        var logger = new TestLogger<BaseRequestHandler<TestRequest, string>>();
        var handler = new TestRequestHandler(null, logger, Result.Ok("response data"));

        // Act
        await handler.Handle(request, CancellationToken.None);

        // Assert
        var informationEntry = logger.Entries.Single();
        var requestData = JsonNode.Parse((string)informationEntry.Properties["@RequestData"]!)!.AsObject();
        await Assert.That(informationEntry.LogLevel).IsEqualTo(LogLevel.Information);
        await Assert.That(informationEntry.Properties["RequestType"]).IsEqualTo(typeof(TestRequest).Name);
        await Assert.That(requestData["Data"]!.GetValue<string>()).IsEqualTo(request.Data);
        await Assert.That(requestData["Password"]!.GetValue<string>()).IsEqualTo("***");
        await Assert.That(requestData["Details"]!["Password"]!.GetValue<string>()).IsEqualTo("***");
        await Assert.That(requestData["Items"]![0]!["Password"]!.GetValue<string>()).IsEqualTo("***");
        await Assert.That(requestData["Details"]!["VisibleValue"]!.GetValue<string>()).IsEqualTo(request.Details.VisibleValue);
    }

    private static TestRequest CreateRequest(string data = "request data") =>
        new(
            data,
            "top-level-secret",
            new SensitivePayload("nested-secret", "visible data"),
            [new SensitivePayload("array-secret", "array data")]);

    private sealed record TestRequest(
        string Data,
        string Password,
        SensitivePayload Details,
        IReadOnlyList<SensitivePayload> Items) : Mediator.IRequest<Result<string>>;

    private sealed record SensitivePayload(string Password, string VisibleValue);

    private sealed class TrackingValidator : AbstractValidator<TestRequest>
    {
        public TrackingValidator()
        {
            RuleFor(request => request.Data)
                .MustAsync((_, cancellationToken) =>
                {
                    CallCount++;
                    CancellationToken = cancellationToken;
                    return Task.FromResult(true);
                });
        }

        public int CallCount { get; private set; }

        public CancellationToken CancellationToken { get; private set; }
    }

    private sealed class TestRequestHandler : BaseRequestHandler<TestRequest, string>
    {
        private readonly Result<string> _result;

        public TestRequestHandler(
            IServiceProvider? services,
            ILogger<BaseRequestHandler<TestRequest, string>> logger,
            Result<string> result)
            : base(services, logger)
        {
            _result = result;
        }

        public int CallCount { get; private set; }

        public TestRequest? HandledRequest { get; private set; }

        public CancellationToken CancellationToken { get; private set; }

        protected override ValueTask<Result<string>> Handler(
            TestRequest request,
            CancellationToken cancellationToken)
        {
            CallCount++;
            HandledRequest = request;
            CancellationToken = cancellationToken;
            return ValueTask.FromResult(_result);
        }
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

            Entries.Add(new LogEntry(logLevel, properties));
        }
    }

    private sealed record LogEntry(
        LogLevel LogLevel,
        IReadOnlyDictionary<string, object?> Properties);
}
