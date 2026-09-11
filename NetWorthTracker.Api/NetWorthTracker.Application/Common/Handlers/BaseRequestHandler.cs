using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentResults;
using FluentValidation;
using MediatR;
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

    public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling request of type {RequestType}", typeof(TRequest).Name);
        _logger.LogInformation("Request data: {@RequestData}", HideSensitiveData(request));
        var validators = _services?.GetServices<IValidator<TRequest>>() ?? Array.Empty<IValidator<TRequest>>();

        await Task.WhenAll(
            validators.Select(validator =>
                validator.ValidateAndThrowAsync(request, cancellationToken)));

        return await Handler(request, cancellationToken);
    }

    protected abstract Task<Result<TResponse>> Handler(
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