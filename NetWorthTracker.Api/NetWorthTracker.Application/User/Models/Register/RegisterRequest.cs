namespace NetWorthTracker.Application.User.Models.Register;

public sealed record RegisterRequest(string Username, string Password, string Email);