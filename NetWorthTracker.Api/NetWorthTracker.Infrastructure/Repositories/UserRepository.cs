using Microsoft.EntityFrameworkCore;
using NetWorthTracker.Domain.User.Interfaces;
using NetWorthTracker.Domain.User.Models;

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

    public async Task<bool> RegisterAsync(string username, string password, string email,
        CancellationToken cancellationToken = default)
    {
        var encryptedPassword = BCrypt.HashPassword(password);

        var newUser = new User(
            UserId: Guid.NewGuid(),
            Login: username,
            PasswordHash: encryptedPassword,
            Email: email,
            IsEmailConfirmed: true, // todo: hardcoded for now, set it to false once we implement email confirmation
            CreatedAt: DateTime.UtcNow
        );

        await _context.Users.AddAsync(newUser, cancellationToken);
        var result = await _context.SaveChangesAsync(cancellationToken);

        // todo: not sure if we want to return some kind of userid after successful registration
        // or just keep it as it is and let exception to response mapper return correct response
        return result > 0;
    }

    public async Task<User?> GetByUsernameAsync(string username, string email,
        CancellationToken cancellationToken = default) =>
        await _context.Users.SingleOrDefaultAsync(u => u.Login == username || u.Email == email, cancellationToken);
}