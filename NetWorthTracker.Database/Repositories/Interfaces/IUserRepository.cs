using FluentResults;
using NetWorthTracker.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Repositories.Interfaces;

public interface IUserRepository
{
    Task<Result<IEnumerable<User>>> GetAllUsers(CancellationToken cancellationToken = default);
    Task<Result<User>> CreateUser(User user, bool withDefaultDefinitions = true, CancellationToken cancellationToken = default);
}
