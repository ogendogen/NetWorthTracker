using System;

namespace NetWorthTracker.Domain.User.Models;

public class User
{
    public Guid UserId { get; set; }

    public string Login { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsEmailConfirmed { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
