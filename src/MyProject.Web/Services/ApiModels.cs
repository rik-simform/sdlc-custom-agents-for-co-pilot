#nullable enable

namespace MyProject.Web.Services;

// ── Auth models ──────────────────────────────────────────────────────────────

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
public record LoginResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt, IList<string> Roles);
public record RegisterResponse(string UserId, string Email);
public record RefreshTokenRequest(string RefreshToken);
public record RevokeTokenRequest(string RefreshToken);

// ── Inventory models ─────────────────────────────────────────────────────────

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

public record CreateInventoryItemRequest(
    string Sku,
    string Name,
    string? Description,
    string Category,
    int QuantityInStock,
    int ReorderLevel,
    decimal UnitPrice,
    string? Location);

public record UpdateInventoryItemRequest(
    string Name,
    string? Description,
    string Category,
    int QuantityInStock,
    int ReorderLevel,
    decimal UnitPrice,
    string? Location,
    bool IsActive);

// ── Order models ─────────────────────────────────────────────────────────

public record CreateOrderRequest(Guid InventoryItemId, int QuantityRequested, string? Notes = null);

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

public record UpdateOrderStatusRequest(string Status, string? Notes = null);

// -- Paginated Order DTOs --------------------------------------------------

public record OrderDto(
    Guid Id,
    string UserId,
    string ItemName,
    int QuantityRequested,
    string Status,
    DateTimeOffset OrderedAt,
    DateTimeOffset? FulfilledAt,
    string? Notes);

public record AdminOrderDto(
    Guid Id,
    string UserName,
    string UserEmail,
    string ItemName,
    int QuantityRequested,
    string Status,
    DateTimeOffset OrderedAt,
    DateTimeOffset? FulfilledAt);

public record PaginatedOrderResponse(
    List<OrderDto> Items,
    int CurrentPage,
    int PageSize,
    int TotalCount,
    int TotalPages);

public record PaginatedAdminOrderResponse(
    List<AdminOrderDto> Items,
    int CurrentPage,
    int PageSize,
    int TotalCount,
    int TotalPages);

