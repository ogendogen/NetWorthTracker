using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace NetWorthTracker.Infrastructure;

public sealed class NetWorthTrackerDbContextFactory : IDesignTimeDbContextFactory<NetWorthTrackerDbContext>
{
    public NetWorthTrackerDbContext CreateDbContext(string[] args)
    {
        var configurationPath = FindApiConfigurationPath();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(configurationPath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                $"Connection string 'DefaultConnection' was not found in '{configurationPath}' or environment variables.");

        var optionsBuilder = new DbContextOptionsBuilder<NetWorthTrackerDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new NetWorthTrackerDbContext(optionsBuilder.Options);
    }

    private static string FindApiConfigurationPath()
    {
        for (var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
             directory is not null;
             directory = directory.Parent)
        {
            var apiConfigurationPath = Path.Combine(directory.FullName, "NetWorthTracker.Api");
            if (File.Exists(Path.Combine(apiConfigurationPath, "appsettings.Development.json"))
                || File.Exists(Path.Combine(apiConfigurationPath, "appsettings.json")))
            {
                return apiConfigurationPath;
            }

            if (File.Exists(Path.Combine(directory.FullName, "appsettings.Development.json"))
                || File.Exists(Path.Combine(directory.FullName, "appsettings.json")))
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException(
            "Could not locate the API project configuration files (appsettings.json or appsettings.Development.json).");
    }
}
