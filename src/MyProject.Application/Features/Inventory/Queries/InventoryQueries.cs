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

// ─── Order Queries ───────────────────────────────────────────────────────────

/// <summary>Returns all orders placed by a specific user.</summary>
public record GetUserOrdersQuery(string UserId)
    : IRequest<IEnumerable<OrderResponse>>;

/// <summary>Returns all orders in the system (Admin only).</summary>
public record GetAllOrdersQuery
    : IRequest<IEnumerable<OrderDetailResponse>>;

/// <summary>Returns a single order by ID.</summary>
public record GetOrderByIdQuery(Guid OrderId)
    : IRequest<OrderResponse?>;

// ─── Order Query Handlers ────────────────────────────────────────────────────

/// <summary>Handles <see cref="GetUserOrdersQuery"/>.</summary>
public class GetUserOrdersQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetUserOrdersQuery, IEnumerable<OrderResponse>>
{
    public async Task<IEnumerable<OrderResponse>> Handle(GetUserOrdersQuery request, CancellationToken ct)
    {
        var (orders, _) = await orderRepository.GetByUserIdAsync(request.UserId, ct: ct).ConfigureAwait(false);
        return orders.Select(o => o.ToResponse());
    }
}

/// <summary>Handles <see cref="GetAllOrdersQuery"/>.</summary>
public class GetAllOrdersQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderDetailResponse>>
{
    public async Task<IEnumerable<OrderDetailResponse>> Handle(GetAllOrdersQuery request, CancellationToken ct)
    {
        var (orders, _) = await orderRepository.GetAllAsync(ct: ct).ConfigureAwait(false);
        return orders.Select(o => o.ToDetailResponse());
    }
}

/// <summary>Handles <see cref="GetOrderByIdQuery"/>.</summary>
public class GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetOrderByIdQuery, OrderResponse?>
{
    public async Task<OrderResponse?> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, ct).ConfigureAwait(false);
        return order?.ToResponse();
    }
}

// ─── Order mapping extensions ────────────────────────────────────────────────

/// <summary>Mapping extensions for Order entities.</summary>
public static class OrderExtensions
{
    public static OrderResponse ToResponse(this Order order) =>
        new(order.Id, order.UserId, order.InventoryItemId,
            order.InventoryItem.Name, order.InventoryItem.Sku,
            order.QuantityRequested, order.Status, order.OrderedAt,
            order.FulfilledAt, order.Notes);

    public static OrderDetailResponse ToDetailResponse(this Order order) =>
        new(order.Id, order.UserId, order.User.Email ?? "",
            $"{order.User.FirstName} {order.User.LastName}",
            order.InventoryItemId, order.InventoryItem.Name,
            order.InventoryItem.Sku, order.InventoryItem.UnitPrice,
            order.QuantityRequested, order.Status, order.OrderedAt,
            order.FulfilledAt, order.Notes);
}
