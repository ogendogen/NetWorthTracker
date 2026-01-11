using NetWorthTracker.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Repositories.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsers(CancellationToken cancellationToken = default);
}
