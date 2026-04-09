#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyProject.Domain.Entities;

namespace MyProject.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the Order entity.
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(o => o.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(o => o.InventoryItemId)
            .IsRequired();

        builder.Property(o => o.QuantityRequested)
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Pending");

        builder.Property(o => o.OrderedAt)
            .IsRequired();

        builder.Property(o => o.FulfilledAt)
            .IsRequired(false);

        builder.Property(o => o.Notes)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt)
            .IsRequired();

        // Foreign key relationships
        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.InventoryItem)
            .WithMany()
            .HasForeignKey(o => o.InventoryItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for query performance
        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.InventoryItemId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => new { o.UserId, o.Status });
        builder.HasIndex(o => o.OrderedAt).IsDescending();
    }
}
