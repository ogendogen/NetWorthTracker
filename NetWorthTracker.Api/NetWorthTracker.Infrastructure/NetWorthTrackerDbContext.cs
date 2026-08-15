using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NetWorthTracker.Domain.User.Models;

namespace NetWorthTracker.Infrastructure;

public class NetWorthTrackerDbContext : DbContext
{
    public NetWorthTrackerDbContext(DbContextOptions<NetWorthTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NetWorthTrackerDbContext).Assembly);
    }
}
