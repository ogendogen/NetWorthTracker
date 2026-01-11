using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetWorthTracker.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Database.Configurations;

public class DebtDefinitionConfiguration : IEntityTypeConfiguration<DebtDefinition>
{
    public void Configure(EntityTypeBuilder<DebtDefinition> builder)
    {
        builder.HasKey(e => e.Id);

        builder.ToTable("debtsdefinitions");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(64);

        builder.Property(e => e.UserId)
            .HasColumnName("user_id");

        builder.HasOne(e => e.User)
            .WithMany(e => e.DebtsDefinitions)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
