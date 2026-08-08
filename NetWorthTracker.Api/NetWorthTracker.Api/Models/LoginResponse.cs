namespace NetWorthTracker.Api.Models;

public sealed record LoginResponse(string AccessToken, DateTimeOffset ExpiresAt, string UserName);