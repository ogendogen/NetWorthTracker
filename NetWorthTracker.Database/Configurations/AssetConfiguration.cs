using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetWorthTracker.Database.Models;

namespace NetWorthTracker.Database.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.HasKey(e => e.Id);

        builder.ToTable("assets");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(e => e.EntryId)
            .HasColumnName("entry_id");

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(64)
            .IsRequired();

        builder.HasOne(e => e.Entry)
            .WithMany(e => e.Assets)
            .HasForeignKey(e => e.EntryId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
