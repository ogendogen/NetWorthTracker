using Microsoft.EntityFrameworkCore;
using NetWorthTracker.Domain.User.Interfaces;

namespace NetWorthTracker.Infrastructure.Repositories;

using BCrypt = BCrypt.Net.BCrypt;

public class UserRepository(NetWorthTrackerDbContext dbContext) : IUserRepository
{
    public async Task<bool> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Login == username, cancellationToken);

        return user is not null && BCrypt.Verify(password, user.PasswordHash);
    }
}