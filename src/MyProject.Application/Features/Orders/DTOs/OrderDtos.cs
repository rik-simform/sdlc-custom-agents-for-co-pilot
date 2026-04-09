#nullable enable

namespace MyProject.Application.Features.Orders.DTOs;

/// <summary>Request to create a new order.</summary>
public record CreateOrderRequest(Guid InventoryItemId, int QuantityRequested, string? Notes = null);

/// <summary>Response for order information.</summary>
public record OrderResponse(
    Guid Id,
    string UserId,
    string ItemName,
    int QuantityRequested,
    string Status,
    DateTimeOffset OrderedAt,
    DateTimeOffset? FulfilledAt,
    string? Notes);

/// <summary>Response for admin view of an order with user details.</summary>
public record AdminOrderResponse(
    Guid Id,
    string UserName,
    string UserEmail,
    string ItemName,
    int QuantityRequested,
    string Status,
    DateTimeOffset OrderedAt,
    DateTimeOffset? FulfilledAt);

/// <summary>Paginated response wrapper.</summary>
public record PaginatedResponse<T>(
    List<T> Items,
    int CurrentPage,
    int PageSize,
    int TotalCount,
    int TotalPages);
