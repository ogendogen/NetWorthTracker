namespace NetWorthTracker.Application.User.Models.Login;

public sealed record LoginResponse(string AccessToken, DateTimeOffset ExpiresAt, string UserName);