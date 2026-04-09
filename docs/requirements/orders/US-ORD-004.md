# US-ORD-004: Admin Views All Orders Dashboard

**Type**: Functional
**Priority**: Critical
**Story Points**: 5
**Source**: EPIC-ORD / PRD Section 6
**Status**: Ready

---

## User Story

**As an** Administrator (Admin role)
**I want to** view all customer orders in a centralized read-only dashboard
**So that** I can monitor order activity, track fulfillment status, and manage customer inquiries

---

## Acceptance Criteria

- [ ] **AC-017**: Given an authenticated user with the `Admin` role, when they call `GET /api/v1/orders/`, then the system returns HTTP 200 OK with a paginated list of **all orders across all users** (no filtering by user ID), including `Id`, `Username`, `Email`, `ItemName`, `Quantity`, `Status`, `OrderedAt`, and `FulfilledAt`.

- [ ] **AC-018**: Given a user without the `Admin` role (e.g., `User` role), when they call `GET /api/v1/orders/`, then the system returns HTTP 403 Forbidden.

- [ ] **AC-019**: Each order in the response includes the customer's user details (username and email) for identification and follow-up purposes.

- [ ] **AC-020**: Given a request with query parameters `page=2&pageSize=20`, when an admin calls `GET /api/v1/orders/?page=2&pageSize=20`, then the system returns the correct page of results with pagination metadata (total count, current page, total pages).

- [ ] **AC-021**: The response includes complete order metadata: `Id`, `UserId`, `ItemName`, `Quantity`, `Status`, `OrderedAt`, `FulfilledAt`, and optional `Notes`.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | Admin list endpoint responds < 1s (p95) for 100+ orders |
| Security | Only `Admin` role users can access; enforced via `[Authorize(Roles="Admin")]` |
| Security | No user data leakage; only PII necessary for order context (username, email) |
| Scalability | Pagination enforced (max 100 per page); supports 10,000+ orders |
| Audit | Admin dashboard access could be logged for compliance |
| UI | Orders displayed in table format with sortable columns (future enhancement) |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-LOGIN-001 — JWT authentication | Cross-Epic |
| DEP-002 | US-RBAC-006 — Admin role available | Cross-Epic |
| DEP-003 | InventoryItems table with item names | Infrastructure |

---

## Implementation Notes

### API Endpoint

| Method | Route | Auth | Query Params | Description |
|--------|-------|------|-----|-------------|
| GET | `/api/v1/orders/` | Required (`Admin` role) | page, pageSize, status (future), user (future) | List all orders (admin only) |

### Response Model

**AdminOrderResponse**
```csharp
public record AdminOrderResponse(
    int Id,
    string UserId,
    string Username,
    string Email,
    string ItemName,
    int Quantity,
    string Status,
    DateTimeOffset OrderedAt,
    DateTimeOffset? FulfilledAt,
    string? Notes
);
```

**PaginatedResponse<T>**
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
public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<PaginatedResponse<AdminOrderResponse>>>
{
    public async Task<Result<PaginatedResponse<AdminOrderResponse>>> Handle(GetAllOrdersQuery request, ...)
    {
        // 1. Validate Admin role (authorization middleware)
        // 2. Query Orders table (NO UserId filter — get all)
        // 3. Eager load User and InventoryItem entities
        // 4. Apply pagination (skip, take, order by OrderedAt DESC)
        // 5. Map to AdminOrderResponse list (includes Username, Email)
        // 6. Count total records
        // 7. Return paginated results with metadata
    }
}
```

### Validation Rules

```csharp
public class GetAllOrdersQueryValidator : AbstractValidator<GetAllOrdersQuery>
{
    public GetAllOrdersQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1)
            .WithMessage("Page must be >= 1");
        
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be > 0")
            .LessThanOrEqualTo(100).WithMessage("PageSize cannot exceed 100");
        
        // Future: add status and user filter validators
    }
}
```

### Endpoint Authorization

```csharp
group.MapGet("/", GetAllOrders)
    .WithName("GetAllOrders")
    .RequireAuthorization(policy => policy.RequireRole("Admin"))
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status403Forbidden);
```

### Web UI (Razor Page)

**Pages/Orders/Admin/Index.cshtml.cs**
- Injects HttpClient via DI
- OnGetAsync() calls GET /api/v1/orders/ with pagination
- Requires Admin role (enforced by [Authorize(Roles="Admin")] attribute on PageModel)
- Binds results to PagedList<AdminOrderResponse>
- Non-admin redirected to 403 error page

**Pages/Orders/Admin/Index.cshtml**
- Table columns: User (username), Email, Item, Quantity, Status (badge), OrderedAt, FulfilledAt, Actions
- Status badge styling: Pending=yellow, Processing=blue, Fulfilled=green, Cancelled=gray
- Pagination controls (Previous/Next/page numbers)
- Row click could link to order detail page (future)

---

## Future Enhancements

- **US-ORD-005**: Admin filters by status and user (query params)
- **US-ORD-006**: Admin updates order status (Processing, Fulfilled) — restricted to Admin role
- **US-ORD-009**: Order export to CSV/PDF
- Column sorting by Status, OrderedAt, etc.

---

## Linked Artifacts

- Design: ADR-ORDERS-001
- Tests: TC-ORD-019 – TC-ORD-024 (integration + authorization)
- Implementation: GetAllOrdersQuery, GetAllOrdersQueryHandler, OrderEndpoints (GET admin handler)
