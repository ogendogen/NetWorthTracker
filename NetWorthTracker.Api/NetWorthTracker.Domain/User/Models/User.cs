using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Domain.User.Models;

public record User(
    Guid UserId,
    string Login,
    string PasswordHash,
    string Email,
    bool IsEmailConfirmed,
    DateTimeOffset CreatedAt);