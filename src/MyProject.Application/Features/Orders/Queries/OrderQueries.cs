#nullable enable

using MediatR;
using MyProject.Application.Features.Orders.Commands;
using MyProject.Application.Features.Orders.DTOs;
using MyProject.Domain.Interfaces;

namespace MyProject.Application.Features.Orders.Queries;

/// <summary>Query to get orders for the authenticated user.</summary>
public record GetUserOrdersQuery(string UserId, int Page = 1, int PageSize = 10)
    : IRequest<Result<PaginatedResponse<OrderResponse>>>;

/// <summary>Handler for GetUserOrdersQuery.</summary>
public class GetUserOrdersQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetUserOrdersQuery, Result<PaginatedResponse<OrderResponse>>>
{
    public async Task<Result<PaginatedResponse<OrderResponse>>> Handle(GetUserOrdersQuery request, CancellationToken ct)
    {
        int skip = (request.Page - 1) * request.PageSize;
        var (orders, totalCount) = await orderRepository.GetByUserIdAsync(
            request.UserId, skip, request.PageSize, ct);

        var items = orders
            .Select(o => new OrderResponse(
                o.Id,
                o.UserId,
                o.InventoryItem?.Name ?? "Unknown",
                o.QuantityRequested,
                o.Status,
                o.OrderedAt,
                o.FulfilledAt,
                o.Notes))
            .ToList();

        int totalPages = (totalCount + request.PageSize - 1) / request.PageSize;
        var response = new PaginatedResponse<OrderResponse>(items, request.Page, request.PageSize, totalCount, totalPages);
        return Result<PaginatedResponse<OrderResponse>>.Ok(response);
    }
}

/// <summary>Query to get all orders (admin only).</summary>
public record GetAllOrdersQuery(int Page = 1, int PageSize = 10, string? Status = null, string? UserFilter = null)
    : IRequest<Result<PaginatedResponse<AdminOrderResponse>>>;

/// <summary>Handler for GetAllOrdersQuery.</summary>
public class GetAllOrdersQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetAllOrdersQuery, Result<PaginatedResponse<AdminOrderResponse>>>
{
    public async Task<Result<PaginatedResponse<AdminOrderResponse>>> Handle(GetAllOrdersQuery request, CancellationToken ct)
    {
        int skip = (request.Page - 1) * request.PageSize;
        var (orders, totalCount) = await orderRepository.GetAllAsync(
            skip, request.PageSize, request.Status, request.UserFilter, ct);

        var items = orders
            .Select(o => new AdminOrderResponse(
                o.Id,
                o.User?.UserName ?? "Unknown",
                o.User?.Email ?? "Unknown",
                o.InventoryItem?.Name ?? "Unknown",
                o.QuantityRequested,
                o.Status,
                o.OrderedAt,
                o.FulfilledAt))
            .ToList();

        int totalPages = (totalCount + request.PageSize - 1) / request.PageSize;
        var response = new PaginatedResponse<AdminOrderResponse>(items, request.Page, request.PageSize, totalCount, totalPages);
        return Result<PaginatedResponse<AdminOrderResponse>>.Ok(response);
    }
}

/// <summary>Query to get a specific order by ID.</summary>
public record GetOrderByIdQuery(Guid OrderId)
    : IRequest<Result<OrderResponse?>>;

/// <summary>Handler for GetOrderByIdQuery.</summary>
public class GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetOrderByIdQuery, Result<OrderResponse?>>
{
    public async Task<Result<OrderResponse?>> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId, ct);
        if (order is null)
            return Result<OrderResponse?>.Ok(null);

        var response = new OrderResponse(
            order.Id,
            order.UserId,
            order.InventoryItem?.Name ?? "Unknown",
            order.QuantityRequested,
            order.Status,
            order.OrderedAt,
            order.FulfilledAt,
            order.Notes);

        return Result<OrderResponse?>.Ok(response);
    }
}
