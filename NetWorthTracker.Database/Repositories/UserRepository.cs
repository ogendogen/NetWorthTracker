using FluentResults;
using Microsoft.EntityFrameworkCore;
using NetWorthTracker.Database.Models;
using NetWorthTracker.Database.Repositories.Interfaces;

namespace NetWorthTracker.Database.Repositories;

public class UserRepository : IUserRepository
{
    private readonly NetWorthTrackerDbContext _context;
    public UserRepository(NetWorthTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<Result<User>> CreateUser(User user, CancellationToken cancellationToken = default)
    {
        _context.Add(user);
        var affected = await _context.SaveChangesAsync();

        return affected == 1 ? Result.Ok(user) : Result.Fail("Użytkownik nie dodany");
    }

    public async Task<Result<IEnumerable<User>>> GetAllUsers(CancellationToken cancellationToken = default)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }
}
