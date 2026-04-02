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
