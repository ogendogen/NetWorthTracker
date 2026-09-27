namespace NetWorthTracker.Api.Services;

public sealed record ExceptionResponse(int StatusCode, string Title, string Detail);
