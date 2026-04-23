#nullable enable

using MediatR;
using MyProject.Application.Features.Orders.DTOs;
using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;

namespace MyProject.Application.Features.Orders.Commands;

/// <summary>Command to place a new order.</summary>
public record CreateOrderCommand(string UserId, Guid InventoryItemId, int QuantityRequested, string? Notes = null)
    : IRequest<Result<OrderResponse>>;

/// <summary>Handler for CreateOrderCommand.</summary>
public class CreateOrderCommandHandler(IOrderRepository orderRepository, IInventoryRepository inventoryRepository)
    : IRequestHandler<CreateOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        // Verify inventory item exists
        var item = await inventoryRepository.GetByIdAsync(request.InventoryItemId, ct);
        if (item is null)
            return Result<OrderResponse>.Fail("Inventory item not found");

        // Verify sufficient quantity
        if (request.QuantityRequested <= 0 || request.QuantityRequested > 999)
            return Result<OrderResponse>.Fail("Quantity must be between 1 and 999");

        if (item.QuantityInStock < request.QuantityRequested)
            return Result<OrderResponse>.Fail("Insufficient inventory available");

        // Create order
        var order = new Order
        {
            UserId = request.UserId,
            InventoryItemId = request.InventoryItemId,
            QuantityRequested = request.QuantityRequested,
            Status = "Pending",
            OrderedAt = DateTimeOffset.UtcNow,
            Notes = request.Notes
        };

        await orderRepository.AddAsync(order, ct);

        return Result<OrderResponse>.Ok(new OrderResponse(
            order.Id,
            order.UserId,
            item.Name,
            order.QuantityRequested,
            order.Status,
            order.OrderedAt,
            order.FulfilledAt,
            order.Notes));
    }
}

/// <summary>Command to cancel a pending order.</summary>
public record CancelOrderCommand(Guid OrderId, string UserId)
    : IRequest<Result<OrderResponse>>;

/// <summary>Handler for CancelOrderCommand.</summary>
public class CancelOrderCommandHandler(IOrderRepository orderRepository)
    : IRequestHandler<CancelOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, ct);
        if (order is null)
            return Result<OrderResponse>.Fail("Order not found");

        if (order.UserId != request.UserId)
            return Result<OrderResponse>.Fail("Unauthorized: you cannot cancel this order");

        if (order.Status != "Pending")
            return Result<OrderResponse>.Fail($"Cannot cancel order with status '{order.Status}'");

        order.Status = "Cancelled";
        order.UpdatedAt = DateTimeOffset.UtcNow;
        await orderRepository.UpdateAsync(order, ct);

        var itemName = order.InventoryItem?.Name ?? "Unknown";
        return Result<OrderResponse>.Ok(new OrderResponse(
            order.Id,
            order.UserId,
            itemName,
            order.QuantityRequested,
            order.Status,
            order.OrderedAt,
            order.FulfilledAt,
            order.Notes));
    }
}

/// <summary>Command for an admin to mark a pending order as fulfilled.</summary>
public record FulfillOrderCommand(Guid OrderId) : IRequest<Result<AdminOrderResponse>>;

/// <summary>Handler for <see cref="FulfillOrderCommand"/>.</summary>
public class FulfillOrderCommandHandler(IOrderRepository orderRepository)
    : IRequestHandler<FulfillOrderCommand, Result<AdminOrderResponse>>
{
    /// <inheritdoc />
    public async Task<Result<AdminOrderResponse>> Handle(FulfillOrderCommand request, CancellationToken ct)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, ct);
        if (order is null)
            return Result<AdminOrderResponse>.Fail("Order not found");

        if (order.Status == "Cancelled")
            return Result<AdminOrderResponse>.Fail("Cannot fulfill a cancelled order");

        if (order.Status == "Fulfilled")
            return Result<AdminOrderResponse>.Fail("Order has already been fulfilled");

        order.Status = "Fulfilled";
        order.FulfilledAt = DateTimeOffset.UtcNow;
        order.UpdatedAt = DateTimeOffset.UtcNow;
        await orderRepository.UpdateAsync(order, ct);

        return Result<AdminOrderResponse>.Ok(new AdminOrderResponse(
            order.Id,
            order.User?.UserName ?? "Unknown",
            order.User?.Email ?? "Unknown",
            order.InventoryItem?.Name ?? "Unknown",
            order.QuantityRequested,
            order.Status,
            order.OrderedAt,
            order.FulfilledAt));
    }
}

/// <summary>Generic Result wrapper for responses.</summary>
public record Result<T>(bool IsSuccess, T? Value, string? Error)
{
    public static Result<T> Ok(T value) => new(true, value, null);
    public static Result<T> Fail(string error) => new(false, default, error);
}
