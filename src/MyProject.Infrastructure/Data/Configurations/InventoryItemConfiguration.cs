#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyProject.Domain.Entities;

namespace MyProject.Infrastructure.Data.Configurations;

/// <summary>EF Core table/column/index configuration for <see cref="InventoryItem"/>.</summary>
public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Sku).IsRequired().HasMaxLength(50);
        builder.Property(i => i.Name).IsRequired().HasMaxLength(200);
        builder.Property(i => i.Description).HasMaxLength(1000);
        builder.Property(i => i.Category).IsRequired().HasMaxLength(100);
        builder.Property(i => i.UnitPrice).HasPrecision(18, 2).IsRequired();
        builder.Property(i => i.Location).HasMaxLength(200);
        builder.Property(i => i.CreatedBy).IsRequired().HasMaxLength(450);
        builder.Property(i => i.UpdatedBy).HasMaxLength(450);
        builder.Property(i => i.IsActive).IsRequired().HasDefaultValue(true);

        builder.HasIndex(i => i.Sku).IsUnique();
        builder.HasIndex(i => i.Category);
        builder.HasIndex(i => new { i.IsActive, i.QuantityInStock, i.ReorderLevel });

        // Restrict so deleting a user does not cascade-delete inventory items
        builder.HasOne(i => i.Creator)
            .WithMany()
            .HasForeignKey(i => i.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // NeedsReorder is a computed property, not a column
        builder.Ignore(i => i.NeedsReorder);
    }
}
