# US-ORD-001: User Places Order

**Type**: Functional
**Priority**: Critical
**Story Points**: 5
**Source**: EPIC-ORD / PRD Section 6
**Status**: Ready

---

## User Story

**As a** Customer (User role)
**I want to** browse available inventory items and place an order specifying quantity and optional notes
**So that** I can purchase items from the platform and track my purchase

---

## Acceptance Criteria

- [ ] **AC-001**: Given an authenticated user with a valid `CreateOrderRequest` (InventoryItemId, Quantity, Notes), when they call `POST /api/v1/orders/`, then the system returns HTTP 201 Created with `OrderResponse` including the server-generated Order `Id`, and a `Location` header pointing to the order details.

- [ ] **AC-002**: Given a request with invalid quantity (0, negative, or > 999), when the user submits, then the system returns HTTP 400 with `ValidationProblemDetails` listing the quantity error.

- [ ] **AC-003**: Given a request with a non-existent `InventoryItemId`, when the user submits, then the system returns HTTP 400 with error message "Inventory item not found."

- [ ] **AC-004**: Given a successful order creation, then the Order entity is created with `Status = Pending`, `OrderedAt = UtcNow`, `UserId = authenticated user ID`, and `FulfilledAt = null`.

- [ ] **AC-005**: Given a request from an unauthenticated user (no JWT or expired JWT), when they call `POST /api/v1/orders/`, then the system returns HTTP 401 Unauthorized.

- [ ] **AC-006**: Given an order request for quantity greater than available inventory, when the user submits, the system validates inventory availability and returns HTTP 400 if insufficient stock.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | Create order endpoint responds < 1s (p95) |
| Security | User can only create orders for themselves; system extracts UserId from JWT `sub` claim |
| Security | InventoryItemId is validated to exist in InventoryItems table |
| Security | All input validated server-side via FluentValidation before DB write |
| Security | SQL injection prevented by EF Core parameterized queries |
| Scalability | Support 1000+ concurrent order creations |
| Transactionality | Order creation is atomic — all-or-nothing (single transaction) |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-LOGIN-001 — JWT authentication middleware configured | Cross-Epic |
| DEP-002 | US-RBAC-004 — User role can access inventory | Cross-Epic |
| DEP-003 | SQL Server database with Orders and InventoryItems tables | Infrastructure |

---

## Implementation Notes

### API Endpoint

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/v1/orders/` | Required (`User` role recommended but `[Authorize]` sufficient) | Create a new order |

### Request/Response Models

**CreateOrderRequest**
```csharp
public record CreateOrderRequest(
    int InventoryItemId,
    int Quantity,
    string? Notes
);
```

**OrderResponse**
```csharp
public record OrderResponse(
    int Id,
    string ItemName,
    int Quantity,
    string Status,
    DateTimeOffset OrderedAt,
    DateTimeOffset? FulfilledAt,
    string? Notes
);
```

### Handler Logic (Pseudocode)

```csharp
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, ...)
    {
        // 1. Validate input (FluentValidation)
        // 2. Extract UserId from JWT (HttpContext)
        // 3. Fetch InventoryItem — if not found, return 400
        // 4. Check inventory quantity — if insufficient, return 400
        // 5. Create Order entity with Status=Pending, OrderedAt=now
        // 6. Save Order to database (EF Core)
        // 7. Return 201 Created with OrderResponse + Location header
    }
}
```

### Validation Rules

```csharp
public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.InventoryItemId).GreaterThan(0)
            .WithMessage("InventoryItemId must be greater than 0");
        
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(999).WithMessage("Quantity cannot exceed 999");
        
        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters");
    }
}
```

### Linked Artifacts

- Design: ADR-ORDERS-001
- Tests: TC-ORD-001 – TC-ORD-007 (unit + integration)
- Implementation: CreateOrderCommand, CreateOrderCommandHandler, OrderEndpoints (POST handler)
