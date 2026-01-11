using Microsoft.EntityFrameworkCore;
using NetWorthTracker.Database.Configurations;
using NetWorthTracker.Database.Models;

namespace NetWorthTracker.Database;

public class NetWorthTrackerDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Entry> Entries { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Debt> Debts { get; set; }
    public DbSet<AssetDefinition> AssetsDefinitions { get; set; }
    public DbSet<DebtDefinition> DebtsDefinitions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=networthtracker.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new EntryConfiguration());
        modelBuilder.ApplyConfiguration(new AssetConfiguration());
        modelBuilder.ApplyConfiguration(new DebtConfiguration());
        modelBuilder.ApplyConfiguration(new AssetDefinitionConfiguration());
        modelBuilder.ApplyConfiguration(new DebtDefinitionConfiguration());
    }
}
