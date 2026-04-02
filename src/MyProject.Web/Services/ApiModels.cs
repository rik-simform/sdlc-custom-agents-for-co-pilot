#nullable enable

namespace MyProject.Web.Services;

// ── Auth models ──────────────────────────────────────────────────────────────

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
public record LoginResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);
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
