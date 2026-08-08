namespace NetWorthTracker.Api.Models;

public sealed record NetWorthSummary(decimal NetWorth, decimal Assets, decimal Liabilities, DateTimeOffset UpdatedAt);