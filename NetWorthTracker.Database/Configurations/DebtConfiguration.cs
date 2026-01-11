using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetWorthTracker.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Configurations;

public class DebtConfiguration : IEntityTypeConfiguration<Debt>
{
    public void Configure(EntityTypeBuilder<Debt> builder)
    {
        builder.HasKey(e => e.Id);

        builder.ToTable("debts");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(64);

        builder.Property(e => e.EntryId)
            .HasColumnName("entry_id");

        builder.HasOne(e => e.Entry)
            .WithMany(e => e.Debts)
            .HasForeignKey(e => e.EntryId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
