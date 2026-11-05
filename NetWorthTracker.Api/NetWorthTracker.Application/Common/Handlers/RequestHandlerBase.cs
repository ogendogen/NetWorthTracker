using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetWorthTracker.Application.Common.Constants;

namespace NetWorthTracker.Application.Common.Handlers;

public abstract class RequestHandlerBase<TRequest>
    where TRequest : notnull
{
    private const string RedactedValue = "***";

    private static readonly HashSet<string> SensitivePropertyNames = typeof(SensitiveFields)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(field => field.FieldType == typeof(string))
        .Select(field => (string)field.GetRawConstantValue()!)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

    private readonly IServiceProvider? _services;
    private readonly ILogger _logger;

    protected RequestHandlerBase(IServiceProvider? services, ILogger logger)
    {
        _services = services;
        _logger = logger;
    }

    protected void LogHandling(TRequest request) =>
        _logger.LogInformation("Handling request of type {RequestType}. Data: {@RequestData}", typeof(TRequest).Name,
            HideSensitiveData(request));

    protected async ValueTask<string[]> ValidateRequest(TRequest request, CancellationToken cancellationToken)
    {
        var validators = _services?.GetServices<IValidator<TRequest>>() ?? Array.Empty<IValidator<TRequest>>();
        var validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(request, cancellationToken)));

        if (validationResults.Length == 0 || validationResults.All(result => result.IsValid))
        {
            return [];
        }

        var errors = validationResults.Where(result => !result.IsValid).SelectMany(result => result.Errors)
            .Select(error => error.ErrorMessage)
            .ToArray();

        _logger.LogWarning("Validation failed for request of type {RequestType}. Errors: {Errors}",
            typeof(TRequest).Name, string.Join(", ", errors));

        return errors;
    }

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
