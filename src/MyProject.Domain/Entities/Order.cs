#nullable enable

namespace MyProject.Domain.Entities;

/// <summary>
/// Represents a customer order for inventory items.
/// </summary>
public class Order
{
    /// <summary>
    /// Gets or sets the unique order identifier.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the user ID who placed the order (foreign key to AspNetUsers).
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the inventory item ID being ordered (foreign key to InventoryItems).
    /// </summary>
    public Guid InventoryItemId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of items requested in this order.
    /// </summary>
    public int QuantityRequested { get; set; }

    /// <summary>
    /// Gets or sets the current status of the order (Pending, Processing, Fulfilled, Cancelled).
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Gets or sets the timestamp when the order was placed.
    /// </summary>
    public DateTimeOffset OrderedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the timestamp when the order was fulfilled (null if not yet fulfilled).
    /// </summary>
    public DateTimeOffset? FulfilledAt { get; set; }

    /// <summary>
    /// Gets or sets optional notes or special instructions for the order.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the order record was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the timestamp when the order record was last updated.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Navigation property to the user who placed this order.
    /// </summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Navigation property to the inventory item being ordered.
    /// </summary>
    public InventoryItem InventoryItem { get; set; } = null!;
}
