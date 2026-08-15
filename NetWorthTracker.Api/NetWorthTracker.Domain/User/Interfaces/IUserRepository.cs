using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Domain.User.Interfaces;

public interface IUserRepository
{
    bool? LoginAsync(
        string login,
        string password,
        CancellationToken cancellationToken = default);
}
