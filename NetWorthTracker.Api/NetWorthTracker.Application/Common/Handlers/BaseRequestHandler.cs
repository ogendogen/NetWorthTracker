using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentResults;
using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetWorthTracker.Application.Common.Constants;

namespace NetWorthTracker.Application.Common.Handlers;

public abstract class BaseRequestHandler<TRequest, TResponse>
    : IRequestHandler<TRequest, Result<TResponse>>
    where TRequest : IRequest<Result<TResponse>>
{
    private const string RedactedValue = "***";
    private static readonly HashSet<string> SensitivePropertyNames = typeof(SensitiveFields)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(field => field.FieldType == typeof(string))
        .Select(field => (string)field.GetRawConstantValue()!)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

    private readonly IServiceProvider? _services;
    private readonly ILogger<BaseRequestHandler<TRequest, TResponse>> _logger;

    protected BaseRequestHandler(IServiceProvider? services, ILogger<BaseRequestHandler<TRequest, TResponse>> logger)
    {
        _services = services;
        _logger = logger;
    }

    public async ValueTask<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling request of type {RequestType}. Data: {@RequestData}", typeof(TRequest).Name, HideSensitiveData(request));
        var validators = _services?.GetServices<IValidator<TRequest>>() ?? Array.Empty<IValidator<TRequest>>();

        var validationResults = await Task.WhenAll(
            validators.Select(validator =>
                validator.ValidateAsync(request, cancellationToken)));

        if (validationResults.Any(x => !x.IsValid))
        {
            var errors = validationResults.Where(x => !x.IsValid).SelectMany(x => x.Errors).Select(e => e.ErrorMessage);
            _logger.LogWarning("Validation failed for request of type {RequestType}. Errors: {Errors}", typeof(TRequest).Name, string.Join(", ", errors));
            return Result.Fail<TResponse>("Validation failed").WithErrors(errors);
        }

        return await Handler(request, cancellationToken);
    }

    protected abstract ValueTask<Result<TResponse>> Handler(
        TRequest request,
        CancellationToken cancellationToken);

    private static string HideSensitiveData(TRequest request)
    {
        var requestData = JsonSerializer.SerializeToNode(request);

        RedactSensitiveProperties(requestData);

        return requestData?.ToJsonString() ?? string.Empty;
    }

    private static void RedactSensitiveProperties(JsonNode? node)
    {
        switch (node)
        {
            case JsonObject jsonObject:
                foreach (var property in jsonObject)
                {
                    if (SensitivePropertyNames.Contains(property.Key))
                    {
                        jsonObject[property.Key] = RedactedValue;
                        continue;
                    }

                    RedactSensitiveProperties(property.Value);
                }

                break;
            case JsonArray jsonArray:
                foreach (var item in jsonArray)
                {
                    RedactSensitiveProperties(item);
                }

                break;
        }
    }
}