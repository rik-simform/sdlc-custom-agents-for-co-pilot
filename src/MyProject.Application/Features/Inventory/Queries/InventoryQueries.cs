#nullable enable

using MediatR;
using MyProject.Application.Features.Inventory.DTOs;
using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;

namespace MyProject.Application.Features.Inventory.Queries;

// ─── Queries ────────────────────────────────────────────────────────────────

/// <summary>Returns all active items, optionally filtered by category or search term.</summary>
public record GetInventoryItemsQuery(string? Category = null, string? SearchTerm = null)
    : IRequest<IEnumerable<InventoryItemResponse>>;

/// <summary>Returns a single item by its ID, or null if not found.</summary>
public record GetInventoryItemByIdQuery(Guid Id)
    : IRequest<InventoryItemResponse?>;

/// <summary>Returns all active items whose stock is at or below their reorder level.</summary>
public record GetItemsNeedingReorderQuery
    : IRequest<IEnumerable<InventoryItemResponse>>;

// ─── Handlers ────────────────────────────────────────────────────────────────

/// <summary>Handles <see cref="GetInventoryItemsQuery"/>.</summary>
public class GetInventoryItemsQueryHandler(IInventoryRepository inventoryRepository)
    : IRequestHandler<GetInventoryItemsQuery, IEnumerable<InventoryItemResponse>>
{
    public async Task<IEnumerable<InventoryItemResponse>> Handle(
        GetInventoryItemsQuery request, CancellationToken ct)
    {
        var items = await inventoryRepository
            .GetAllAsync(request.Category, request.SearchTerm, ct)
            .ConfigureAwait(false);

        return items.Select(i => i.ToResponse());
    }
}

/// <summary>Handles <see cref="GetInventoryItemByIdQuery"/>.</summary>
public class GetInventoryItemByIdQueryHandler(IInventoryRepository inventoryRepository)
    : IRequestHandler<GetInventoryItemByIdQuery, InventoryItemResponse?>
{
    public async Task<InventoryItemResponse?> Handle(
        GetInventoryItemByIdQuery request, CancellationToken ct)
    {
        var item = await inventoryRepository
            .GetByIdAsync(request.Id, ct)
            .ConfigureAwait(false);

        return item?.ToResponse();
    }
}

/// <summary>Handles <see cref="GetItemsNeedingReorderQuery"/>.</summary>
public class GetItemsNeedingReorderQueryHandler(IInventoryRepository inventoryRepository)
    : IRequestHandler<GetItemsNeedingReorderQuery, IEnumerable<InventoryItemResponse>>
{
    public async Task<IEnumerable<InventoryItemResponse>> Handle(
        GetItemsNeedingReorderQuery request, CancellationToken ct)
    {
        var items = await inventoryRepository
            .GetItemsNeedingReorderAsync(ct)
            .ConfigureAwait(false);

        return items.Select(i => i.ToResponse());
    }
}

// ─── Shared mapping extension ────────────────────────────────────────────────

/// <summary>Provides a single mapping method shared by all command and query handlers.</summary>
public static class InventoryItemExtensions
{
    public static InventoryItemResponse ToResponse(this InventoryItem item) =>
        new(item.Id, item.Sku, item.Name, item.Description, item.Category,
            item.QuantityInStock, item.ReorderLevel, item.UnitPrice, item.Location,
            item.IsActive, item.NeedsReorder, item.CreatedBy, item.CreatedAt,
            item.UpdatedBy, item.UpdatedAt);
}
