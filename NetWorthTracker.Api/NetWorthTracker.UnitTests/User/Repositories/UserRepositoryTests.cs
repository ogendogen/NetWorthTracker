using Microsoft.EntityFrameworkCore;
using NetWorthTracker.Infrastructure;
using NetWorthTracker.Infrastructure.Repositories;
using NetWorthTracker.Tests.Testing;
using TUnit.Mocks;

namespace NetWorthTracker.Tests.User.Repositories;

using BCrypt = BCrypt.Net.BCrypt;
using UserModel = NetWorthTracker.Domain.User.Models.User;

public class UserRepositoryTests
{
    [Test]
    public async Task GivenExistingUserAndMatchingPassword_WhenLoggingIn_ThenReturnsTrue()
    {
        // Arrange
        const string username = "test-user";
        const string password = "valid-password";
        var user = CreateUser(username, password);
        var repository = CreateRepository(user);

        // Act
        var result = await repository.LoginAsync(username, password);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task GivenMissingUser_WhenLoggingIn_ThenReturnsFalse()
    {
        // Arrange
        var repository = CreateRepository();

        // Act
        var result = await repository.LoginAsync("missing-user", "password");

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task GivenExistingUserAndWrongPassword_WhenLoggingIn_ThenReturnsFalse()
    {
        // Arrange
        const string username = "test-user";
        var user = CreateUser(username, "valid-password");
        var repository = CreateRepository(user);

        // Act
        var result = await repository.LoginAsync(username, "wrong-password");

        // Assert
        await Assert.That(result).IsFalse();
    }

    private static UserRepository CreateRepository(params UserModel[] users)
    {
        var queryable = users.AsQueryable();
        var usersDbSet = Mock.Of<DbSet<UserModel>, IQueryable>();
        usersDbSet.ElementType.Returns(queryable.ElementType);
        usersDbSet.Expression.Returns(queryable.Expression);
        usersDbSet.Provider.Returns(new AsyncQueryProvider(queryable.Provider));

        return new UserRepository(new TestDbContext(usersDbSet.Object));
    }

    private static UserModel CreateUser(string username, string password)
    {
        return new UserModel(
            Guid.NewGuid(),
            username,
            BCrypt.HashPassword(password, workFactor: 4),
            $"{username}@example.com",
            true,
            DateTimeOffset.UtcNow);
    }

    private sealed class TestDbContext : NetWorthTrackerDbContext
    {
        private readonly DbSet<UserModel> _users;

        public TestDbContext(DbSet<UserModel> users)
            : base(new DbContextOptionsBuilder<NetWorthTrackerDbContext>().Options)
        {
            _users = users;
        }

        public override DbSet<UserModel> Users => _users;
    }
}