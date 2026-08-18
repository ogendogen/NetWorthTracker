using Microsoft.EntityFrameworkCore;
using NetWorthTracker.Domain.User.Interfaces;

namespace NetWorthTracker.Infrastructure.Repositories;

using BCrypt = BCrypt.Net.BCrypt;

public class UserRepository : IUserRepository
{
    private readonly NetWorthTrackerDbContext _context;

    public UserRepository(NetWorthTrackerDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<bool> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Login == username, cancellationToken);

        return user is not null && BCrypt.Verify(password, user.PasswordHash);
    }
}