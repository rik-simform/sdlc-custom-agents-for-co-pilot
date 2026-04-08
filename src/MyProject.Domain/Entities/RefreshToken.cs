#nullable enable

namespace MyProject.Domain.Entities;

/// <summary>
/// Represents a refresh token used for JWT token rotation.
/// Stored as a SHA-256 hash in the database for security.
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Gets or sets the unique identifier for the refresh token.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the user ID associated with this refresh token.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SHA-256 hash of the refresh token value.
    /// The plaintext token is never stored.
    /// </summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when this token expires.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets when this token was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the device information from the request.
    /// </summary>
    public string? DeviceInfo { get; set; }

    /// <summary>
    /// Gets or sets whether this token has been revoked.
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// Gets or sets when this token was revoked.
    /// </summary>
    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>
    /// Navigation property to the associated user.
    /// </summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// Determines whether this token is still valid (not expired and not revoked).
    /// </summary>
    public bool IsActive => !IsRevoked && ExpiresAt > DateTimeOffset.UtcNow;
}

/// <summary>
/// Represents a user's order for inventory items.
/// Users can place orders; admins view all orders and manage fulfillment.
/// </summary>
public class Order
{
    /// <summary>
    /// Gets or sets the unique order identifier.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the user ID who placed the order.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the inventory item being ordered.
    /// </summary>
    public Guid InventoryItemId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of items requested in this order.
    /// </summary>
    public int QuantityRequested { get; set; }

    /// <summary>
    /// Gets or sets the order status (Pending, Approved, Fulfilled, Cancelled).
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Gets or sets when the order was placed.
    /// </summary>
    public DateTimeOffset OrderedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets when the order was fulfilled (if applicable).
    /// </summary>
    public DateTimeOffset? FulfilledAt { get; set; }

    /// <summary>
    /// Gets or sets optional notes about the order.
    /// </summary>
    public string? Notes { get; set; }

    // Navigation properties
    /// <summary>
    /// The user who placed the order.
    /// </summary>
    public ApplicationUser User { get; set; } = null!;

    /// <summary>
    /// The inventory item that was ordered.
    /// </summary>
    public InventoryItem InventoryItem { get; set; } = null!;
}
