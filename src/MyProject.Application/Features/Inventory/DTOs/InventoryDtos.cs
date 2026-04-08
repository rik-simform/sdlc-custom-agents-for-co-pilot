#nullable enable

namespace MyProject.Application.Features.Inventory.DTOs;

/// <summary>Request model for creating a new inventory item.</summary>
public record CreateInventoryItemRequest(
    string Sku,
    string Name,
    string? Description,
    string Category,
    int QuantityInStock,
    int ReorderLevel,
    decimal UnitPrice,
    string? Location);

/// <summary>Request model for updating an existing inventory item.</summary>
public record UpdateInventoryItemRequest(
    string Name,
    string? Description,
    string Category,
    int QuantityInStock,
    int ReorderLevel,
    decimal UnitPrice,
    string? Location,
    bool IsActive);

/// <summary>Response model returned for every inventory item operation.</summary>
public record InventoryItemResponse(
    Guid Id,
    string Sku,
    string Name,
    string? Description,
    string Category,
    int QuantityInStock,
    int ReorderLevel,
    decimal UnitPrice,
    string? Location,
    bool IsActive,
    bool NeedsReorder,
    string CreatedBy,
    DateTimeOffset CreatedAt,
    string? UpdatedBy,
    DateTimeOffset? UpdatedAt);

// ── Order DTOs ─────────────────────────────────────────────────────────

/// <summary>Request model for placing a new order.</summary>
/// <param name="InventoryItemId">The inventory item to order.</param>
/// <param name="QuantityRequested">How many units are being ordered.</param>
/// <param name="Notes">Optional notes about the order.</param>
public record CreateOrderRequest(Guid InventoryItemId, int QuantityRequested, string? Notes = null);

/// <summary>Request model for updating an order's status (Admin only).</summary>
/// <param name="Status">The new status: Pending, Processing, Shipped, Delivered, or Cancelled.</param>
/// <param name="Notes">Optional admin notes about the status update.</param>
public record UpdateOrderStatusRequest(string Status, string? Notes = null);

/// <summary>Response model for an order.</summary>
public record OrderResponse(
    Guid Id,
    string UserId,
    Guid InventoryItemId,
    string ItemName,
    string ItemSku,
    int QuantityRequested,
    string Status,
    DateTimeOffset OrderedAt,
    DateTimeOffset? FulfilledAt,
    string? Notes);

/// <summary>Response model for viewing all orders (Admin only).</summary>
public record OrderDetailResponse(
    Guid Id,
    string UserId,
    string UserEmail,
    string UserName,
    Guid InventoryItemId,
    string ItemName,
    string ItemSku,
    decimal UnitPrice,
    int QuantityRequested,
    string Status,
    DateTimeOffset OrderedAt,
    DateTimeOffset? FulfilledAt,
    string? Notes);
