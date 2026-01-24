using FluentResults;
using Microsoft.EntityFrameworkCore;
using NetWorthTracker.Database.Constants;
using NetWorthTracker.Database.Models;

namespace NetWorthTracker.Database.Repositories;

public interface IUserRepository
{
    Task<Result<IEnumerable<User>>> GetAllUsers(CancellationToken cancellationToken = default);
    Task<Result<User>> CreateUser(User user, bool withDefaultDefinitions = true, CancellationToken cancellationToken = default);
}

public class UserRepository : IUserRepository
{
    private readonly NetWorthTrackerDbContext _context;
    public UserRepository(NetWorthTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<Result<User>> CreateUser(User user, bool withDefaultDefinitions = true, CancellationToken cancellationToken = default)
    {
        if (withDefaultDefinitions)
        {
            foreach (var item in Definitions.Assets)
            {
                user.Definitions.Add(item);
            }
        }

        _context.Add(user);
        var affected = await _context.SaveChangesAsync();

        return affected >= 1 ? Result.Ok(user) : Result.Fail("Użytkownik nie dodany");
    }

    public async Task<Result<IEnumerable<User>>> GetAllUsers(CancellationToken cancellationToken = default)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }
}
