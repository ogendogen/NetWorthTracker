using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetWorthTracker.Domain.Common.Interfaces;
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
    public const string TestUserEmail = "integration-user@example.com";
    public const string FrontendUrl = "https://localhost:4200";

    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder("postgres:18").Build();

    public RecordingEmailService EmailService { get; } = new();

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        _ = Server;

        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NetWorthTrackerDbContext>();

        await dbContext.Database.MigrateAsync();
        dbContext.Users.Add(new User
        {
            UserId = Guid.NewGuid(),
            Login = TestUserName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(TestPassword),
            Email = TestUserEmail,
            IsEmailConfirmed = true,
            CreatedAt = DateTimeOffset.UtcNow
        });
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
                ["Jwt:SigningKey"] = JwtSigningKey,
                ["FrontendUrl"] = FrontendUrl,
                ["Seq:ApiUrl"] = "http://localhost:5341",
                ["Seq:ApiKey"] = "integration-tests-only-api-key"
            });
        });
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IEmailService>();
            services.AddSingleton<IEmailService>(EmailService);
        });
    }
}