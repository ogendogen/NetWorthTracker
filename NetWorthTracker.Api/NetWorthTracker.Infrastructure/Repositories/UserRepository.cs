using NetWorthTracker.Domain.User.Interfaces;
using NetWorthTracker.Domain.User.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly NetWorthTrackerDbContext _dbContext;

    public UserRepository(NetWorthTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool? LoginAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        // todo: hash password before comparing with bcrypt
        return _dbContext.Users.SingleOrDefault(u => u.Login == login && u.PasswordHash == password) != null;
    }
}
