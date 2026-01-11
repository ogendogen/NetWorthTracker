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

    public async Task<IEnumerable<User>> GetAllUsers(CancellationToken cancellationToken = default)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }
}
