# US-ORD-002: User Views Order History

**Type**: Functional
**Priority**: Critical
**Story Points**: 3
**Source**: EPIC-ORD / PRD Section 6
**Status**: Ready

---

## User Story

**As a** Customer (User role)
**I want to** view all my past orders with their status and dates
**So that** I can track my purchases and follow up on fulfillment

---

## Acceptance Criteria

- [ ] **AC-007**: Given an authenticated user, when they call `GET /api/v1/orders/my-orders`, then the system returns HTTP 200 with a paginated list of only that user's orders, including `Id`, `ItemName`, `Quantity`, `Status`, `OrderedAt`, and `FulfilledAt`.

- [ ] **AC-008**: Given a request with query parameters `page=2&pageSize=10`, when the user calls `GET /api/v1/orders/my-orders?page=2&pageSize=10`, then the system returns the correct page of results (skip 10, take 10) with pagination metadata (total count, current page).

- [ ] **AC-009**: The `Status` field displays as a text value (`Pending`, `Processing`, `Fulfilled`, or `Cancelled`) with visual formatting (e.g., color badge in UI).

- [ ] **AC-010**: Given an order that has been fulfilled, the `FulfilledAt` timestamp is populated; for pending orders, `FulfilledAt` is null.

- [ ] **AC-011**: Given an unauthenticated user (no valid JWT), when they call `GET /api/v1/orders/my-orders`, then the system returns HTTP 401 Unauthorized.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | List endpoint responds < 500ms (p95) for 100 items per page |
| Security | User can only see their own orders; UserId extracted from JWT `sub` claim |
| Security | Query includes WHERE UserId = @UserId (parameterized) |
| Scalability | Pagination prevents unbounded result sets (max 100 per page) |
| UI | Status badges are color-coded for quick visual scanning |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-ORD-001 — User can place orders | Story |
| DEP-002 | US-LOGIN-001 — JWT authentication | Cross-Epic |

---

## Implementation Notes

### API Endpoint

| Method | Route | Auth | Query Params | Description |
|--------|-------|------|-----|-------------|
| GET | `/api/v1/orders/my-orders` | Required | page, pageSize | List authenticated user's orders |

### Response Model

**OrderResponse (list)**
```csharp
public record OrderResponse(
    int Id,
    string ItemName,
    int Quantity,
    string Status,           // "Pending", "Processing", "Fulfilled", "Cancelled"
    DateTimeOffset OrderedAt,
    DateTimeOffset? FulfilledAt,
    string? Notes
);
```

**PaginatedResponse**
```csharp
public record PaginatedResponse<T>(
    List<T> Items,
    int CurrentPage,
    int PageSize,
    int TotalCount,
    int TotalPages
);
```

### Handler Logic (Pseudocode)

```csharp
public class GetUserOrdersQueryHandler : IRequestHandler<GetUserOrdersQuery, Result<PaginatedResponse<OrderResponse>>>
{
    public async Task<Result<PaginatedResponse<OrderResponse>>> Handle(GetUserOrdersQuery request, ...)
    {
        // 1. Extract UserId from JWT
        // 2. Query Orders WHERE UserId = @UserId, ordered by OrderedAt DESC
        // 3. Apply pagination (skip, take)
        // 4. Eager load InventoryItem (for ItemName)
        // 5. Map to OrderResponse list
        // 6. Return paginated results with metadata
    }
}
```

### Validation Rules

```csharp
public class GetUserOrdersQueryValidator : AbstractValidator<GetUserOrdersQuery>
{
    public GetUserOrdersQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1)
            .WithMessage("Page must be >= 1");
        
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be > 0")
            .LessThanOrEqualTo(100).WithMessage("PageSize cannot exceed 100");
    }
}
```

### Web UI (Razor Page)

**Pages/Orders/Index.cshtml.cs**
- Injects HttpClient via DI
- OnGetAsync() calls GET /api/v1/orders/my-orders with pagination
- Binds results to PagedList<OrderResponse> property
- Supports page navigation

**Pages/Orders/Index.cshtml**
- Table: Id, Item, Quantity, Status (badge), OrderedAt, Actions (View/Cancel)
- Status badge styling: Pending=yellow, Processing=blue, Fulfilled=green, Cancelled=gray
- Pagination controls (Previous/Next/page numbers)

### Linked Artifacts

- Design: ADR-ORDERS-001
- Tests: TC-ORD-008 – TC-ORD-012 (unit + integration)
- Implementation: GetUserOrdersQuery, GetUserOrdersQueryHandler, OrderEndpoints (GET my-orders handler)
