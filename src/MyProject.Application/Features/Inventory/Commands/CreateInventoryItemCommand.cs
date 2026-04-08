#nullable enable

using MediatR;
using Microsoft.Extensions.Logging;
using MyProject.Application.Common;
using MyProject.Application.Features.Inventory.DTOs;
using MyProject.Application.Features.Inventory.Queries;
using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;

namespace MyProject.Application.Features.Inventory.Commands;

/// <summary>Command to create a new inventory item.</summary>
public record CreateInventoryItemCommand(
    string Sku,
    string Name,
    string? Description,
    string Category,
    int QuantityInStock,
    int ReorderLevel,
    decimal UnitPrice,
    string? Location,
    string UserId)
    : IRequest<Result<InventoryItemResponse>>;

/// <summary>Handles <see cref="CreateInventoryItemCommand"/>.</summary>
public class CreateInventoryItemCommandHandler(
    IInventoryRepository inventoryRepository,
    ILogger<CreateInventoryItemCommandHandler> logger)
    : IRequestHandler<CreateInventoryItemCommand, Result<InventoryItemResponse>>
{
    public async Task<Result<InventoryItemResponse>> Handle(CreateInventoryItemCommand request, CancellationToken ct)
    {
        var existing = await inventoryRepository.GetBySkuAsync(request.Sku, ct).ConfigureAwait(false);
        if (existing is not null)
        {
            return Result<InventoryItemResponse>.Fail($"An item with SKU '{request.Sku}' already exists");
        }

        var item = new InventoryItem
        {
            Sku = request.Sku,
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            QuantityInStock = request.QuantityInStock,
            ReorderLevel = request.ReorderLevel,
            UnitPrice = request.UnitPrice,
            Location = request.Location,
            CreatedBy = request.UserId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await inventoryRepository.AddAsync(item, ct).ConfigureAwait(false);

        logger.LogInformation("Inventory item {Sku} created by user {UserId}", item.Sku, request.UserId);

        return Result<InventoryItemResponse>.Ok(item.ToResponse());
    }
}

/// <summary>Command to place a new order for an inventory item.</summary>
public record CreateOrderCommand(Guid InventoryItemId, int QuantityRequested, string UserId, string? Notes = null)
    : IRequest<Result<OrderResponse>>;

/// <summary>Handles <see cref="CreateOrderCommand"/>.</summary>
public class CreateOrderCommandHandler(
    IInventoryRepository inventoryRepository,
    IOrderRepository orderRepository,
    ILogger<CreateOrderCommandHandler> logger)
    : IRequestHandler<CreateOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        // Verify inventory item exists
        var item = await inventoryRepository.GetByIdAsync(request.InventoryItemId, ct).ConfigureAwait(false);
        if (item is null)
        {
            return Result<OrderResponse>.Fail("Inventory item not found");
        }

        // Validate quantity
        if (request.QuantityRequested <= 0)
        {
            return Result<OrderResponse>.Fail("Quantity must be greater than 0");
        }

        var order = new Order
        {
            UserId = request.UserId,
            InventoryItemId = request.InventoryItemId,
            QuantityRequested = request.QuantityRequested,
            Status = "Pending",
            OrderedAt = DateTimeOffset.UtcNow,
            Notes = request.Notes
        };

        await orderRepository.AddAsync(order, ct).ConfigureAwait(false);

        logger.LogInformation("Order {OrderId} placed by user {UserId} for {ItemName}", 
            order.Id, request.UserId, item.Name);

        // Manually create response to avoid null reference since navigation properties aren't loaded
        var response = new OrderResponse(
            order.Id,
            order.UserId,
            order.InventoryItemId,
            item.Name,
            item.Sku,
            order.QuantityRequested,
            order.Status,
            order.OrderedAt,
            order.FulfilledAt,
            order.Notes
        );

        return Result<OrderResponse>.Ok(response);
    }
}

/// <summary>Command to update an order's fulfillment status (Admin only).</summary>
public record UpdateOrderStatusCommand(Guid OrderId, string NewStatus, string? Notes = null)
    : IRequest<Result<OrderResponse>>;

/// <summary>Handles <see cref="UpdateOrderStatusCommand"/>.</summary>
public class UpdateOrderStatusCommandHandler(
    IOrderRepository orderRepository,
    ILogger<UpdateOrderStatusCommandHandler> logger)
    : IRequestHandler<UpdateOrderStatusCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(UpdateOrderStatusCommand request, CancellationToken ct)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, ct).ConfigureAwait(false);

        if (order is null)
        {
            return Result<OrderResponse>.Fail("Order not found");
        }

        var validStatuses = new[] { "Pending", "Approved", "Fulfilled", "Cancelled" };
        if (!validStatuses.Contains(request.NewStatus))
        {
            return Result<OrderResponse>.Fail($"Invalid status. Allowed: {string.Join(", ", validStatuses)}");
        }

        order.Status = request.NewStatus;
        if (request.NewStatus == "Fulfilled")
        {
            order.FulfilledAt = DateTimeOffset.UtcNow;
        }

        if (!string.IsNullOrEmpty(request.Notes))
        {
            order.Notes = request.Notes;
        }

        await orderRepository.UpdateAsync(order, ct).ConfigureAwait(false);

        logger.LogInformation("Order {OrderId} status updated to {Status}", request.OrderId, request.NewStatus);

        return Result<OrderResponse>.Ok(order.ToResponse());
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
