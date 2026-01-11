using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetWorthTracker.Database.Models;

namespace NetWorthTracker.Database.Configurations;

public class EntryConfiguration : IEntityTypeConfiguration<Entry>
{
    public void Configure(EntityTypeBuilder<Entry> builder)
    {
        builder.HasKey(e => e.Id);

        builder.ToTable("entries");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(64);

        builder.Property(e => e.Date)
            .HasColumnName("date");

        builder.Property(e => e.Value)
            .HasColumnName("value")
            .HasPrecision(18, 2);

        builder.Property(e => e.UserId)
            .HasColumnName("user_id");

        builder.HasOne(e => e.User)
            .WithMany(u => u.Entries)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(e => e.Assets)
            .WithOne(e => e.Entry)
            .HasForeignKey(e => e.EntryId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(e => e.Debts)
            .WithOne(e => e.Entry)
            .HasForeignKey(e => e.EntryId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
