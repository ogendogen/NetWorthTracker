namespace NetWorthTracker.Api.Configuration;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public required string Issuer { get; init; }

    public required string Audience { get; init; }

    public required string SigningKey { get; init; }

    public int LifetimeMinutes { get; init; }
}