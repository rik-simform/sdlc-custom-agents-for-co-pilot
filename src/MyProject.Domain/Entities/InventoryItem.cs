#nullable enable

namespace MyProject.Domain.Entities;

/// <summary>
/// Represents an inventory item managed in the system.
/// </summary>
public class InventoryItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public int QuantityInStock { get; set; }
    public int ReorderLevel { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Location { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>Navigation property to the user who created this item.</summary>
    public ApplicationUser Creator { get; set; } = null!;

    /// <summary>True when quantity is at or below the reorder threshold.</summary>
    public bool NeedsReorder => QuantityInStock <= ReorderLevel;
}
