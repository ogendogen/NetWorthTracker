using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetWorthTracker.Domain.User.Models;
using NetWorthTracker.Infrastructure;
using Testcontainers.PostgreSql;
using TUnit.Core.Interfaces;

namespace NetWorthTracker.IntegrationTests;

public sealed class NetWorthTrackerApiFactory : WebApplicationFactory<Program>, IAsyncInitializer
{
    public const string JwtAudience = "NetWorthTracker.IntegrationTests";
    public const string JwtIssuer = "NetWorthTracker.Api.IntegrationTests";
    public const string JwtSigningKey = "integration-tests-only-signing-key-2026";
    public const string TestPassword = "integration-password";
    public const string TestUserName = "integration-user";

    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder("postgres:18").Build();

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        _ = Server;

        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NetWorthTrackerDbContext>();

        await dbContext.Database.MigrateAsync();
        dbContext.Users.Add(new User(
            Guid.NewGuid(),
            TestUserName,
            BCrypt.Net.BCrypt.HashPassword(TestPassword),
            "integration-user@example.com",
            true,
            DateTimeOffset.UtcNow));
        await dbContext.SaveChangesAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _postgreSqlContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _postgreSqlContainer.GetConnectionString(),
                ["Jwt:Audience"] = JwtAudience,
                ["Jwt:Issuer"] = JwtIssuer,
                ["Jwt:LifetimeMinutes"] = "15",
                ["Jwt:SigningKey"] = JwtSigningKey
            });
        });
    }
}