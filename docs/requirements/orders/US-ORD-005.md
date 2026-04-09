# US-ORD-005: Admin Filters Orders by Status and User

**Type**: Functional
**Priority**: High
**Story Points**: 3
**Source**: EPIC-ORD / PRD Section 6
**Status**: Ready

---

## User Story

**As an** Administrator (Admin role)
**I want to** filter displayed orders by status (Pending, Processing, Fulfilled, Cancelled) and by user (username/email)
**So that** I can quickly locate specific orders and manage fulfillment workflows

---

## Acceptance Criteria

- [ ] **AC-022**: Given an admin on the order dashboard, when they select a status from the Status filter dropdown (Pending, Processing, Fulfilled, Cancelled), then the system filters results to show only orders with that status.

- [ ] **AC-023**: Given an admin, when they enter a partial username or email in the User search box, then the system returns orders matching that user (case-insensitive).

- [ ] **AC-024**: Given an admin applying both Status and User filters simultaneously, when they call `GET /api/v1/orders/?status=Pending&user=john`, then the system returns orders matching **both** criteria (AND logic).

- [ ] **AC-025**: Given filter criteria that match no orders, when the system executes the query, then it returns HTTP 200 OK with an empty `Items` list (not a 404).

- [ ] **AC-026**: All filter comparisons are case-insensitive (e.g., "PENDING" = "pending", "John" = "john").

- [ ] **AC-027**: Filtered results respect pagination rules (max 100 per page); when 150 filtered results exist and user requests page 1, they get 100 items; page 2 returns remaining 50.

- [ ] **AC-028**: Given a non-admin user attempting to call `GET /api/v1/orders/?status=Pending`, then the system returns HTTP 403 Forbidden regardless of query parameters.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | Filtered query completes < 1s (p95) even with 10,000+ orders |
| Security | Only authenticated Admin role users can filter; query parameter validation on backend |
| Security | Status filter only accepts valid enum values (Pending, Processing, Fulfilled, Cancelled) |
| Security | User filter accepts any string; backend performs case-insensitive LIKE search on Username or Email |
| Scalability | Indices on Status and UserId enable efficient filtering |
| Usability | Filters are optional (no required params); calling without filters returns all orders |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-ORD-004 — Admin view all orders | Story |
| DEP-002 | US-LOGIN-001 — JWT authentication | Cross-Epic |
| DEP-003 | US-RBAC-006 — Admin role | Cross-Epic |

---

## Implementation Notes

### API Endpoint

| Method | Route | Auth | Query Params | Description |
|--------|-------|------|-----|-------------|
| GET | `/api/v1/orders/` | Required (`Admin` role) | page, pageSize, status, user | List orders with optional filters |

### Query Parameters

| Param | Type | Example | Behavior |
|-------|------|---------|----------|
| `page` | int | `page=1` | Current page (default 1) |
| `pageSize` | int | `pageSize=20` | Items per page (default 10, max 100) |
| `status` | string | `status=Pending` | Filter by order status (case-insensitive) |
| `user` | string | `user=john` | Partial match on username or email (case-insensitive) |

### Example Calls

```
GET /api/v1/orders/?status=Pending
  → All pending orders, page 1, 10 per page

GET /api/v1/orders/?user=john&page=2&pageSize=20
  → Orders for users matching "john" (case-insensitive), page 2, 20 per page

GET /api/v1/orders/?status=Fulfilled&user=admin@example.com&page=1&pageSize=50
  → Fulfilled orders from user containing "admin@example.com", 50 per page

GET /api/v1/orders/
  → No filters, all orders, page 1, 10 per page (default pagination)
```

### Handler Logic (Pseudocode)

```csharp
public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<PaginatedResponse<AdminOrderResponse>>>
{
    public async Task<Result<PaginatedResponse<AdminOrderResponse>>> Handle(GetAllOrdersQuery request, ...)
    {
        // 1. Validate Admin role (middleware)
        // 2. Start with IQueryable<Order>
        // 3. If request.Status is provided:
        //    - Parse status string to enum (case-insensitive)
        //    - Filter: where status == target
        // 4. If request.User is provided:
        //    - Filter: where User.UserName.Contains(request.User, case-insensitive)
        //           OR User.Email.Contains(request.User, case-insensitive)
        // 5. Apply sorting: OrderBy OrderedAt descending
        // 6. Apply pagination: Skip((page-1) * pageSize).Take(pageSize)
        // 7. Eager load User and InventoryItem
        // 8. Map to AdminOrderResponse
        // 9. Return paginated results
    }
}
```

### Validation Rules

```csharp
public class GetAllOrdersQueryValidator : AbstractValidator<GetAllOrdersQuery>
{
    public GetAllOrdersQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        
        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);
        
        RuleFor(x => x.Status)
            .Must(s => string.IsNullOrEmpty(s) || 
                       Enum.TryParse<OrderStatus>(s, ignoreCase: true, out _))
            .WithMessage("Status must be a valid order status (Pending, Processing, Fulfilled, Cancelled)");
        
        RuleFor(x => x.User)
            .MaximumLength(255).When(x => !string.IsNullOrEmpty(x.User))
            .WithMessage("User filter cannot exceed 255 characters");
    }
}
```

### Web UI (Razor Page)

**Pages/Orders/Admin/Index.cshtml.cs**
```csharp
public class IndexModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? UserFilter { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public int Page { get; set; } = 1;
    
    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;
    
    public PaginatedResponse<AdminOrderResponse>? Orders { get; set; }
    
    public async Task OnGetAsync()
    {
        var query = $"?page={Page}&pageSize={PageSize}";
        if (!string.IsNullOrEmpty(StatusFilter))
            query += $"&status={Uri.EscapeDataString(StatusFilter)}";
        if (!string.IsNullOrEmpty(UserFilter))
            query += $"&user={Uri.EscapeDataString(UserFilter)}";
        
        Orders = await _orderApiService.GetAllOrdersAsync(query);
    }
}
```

**Pages/Orders/Admin/Index.cshtml**
```html
<!-- Filter Form -->
<form method="get" class="mb-4">
    <div class="row">
        <div class="col-md-4">
            <label for="statusFilter">Status:</label>
            <select id="statusFilter" name="statusFilter" class="form-control">
                <option value="">All Statuses</option>
                <option value="Pending" selected="@(StatusFilter == "Pending")">Pending</option>
                <option value="Processing" selected="@(StatusFilter == "Processing")">Processing</option>
                <option value="Fulfilled" selected="@(StatusFilter == "Fulfilled")">Fulfilled</option>
                <option value="Cancelled" selected="@(StatusFilter == "Cancelled")">Cancelled</option>
            </select>
        </div>
        <div class="col-md-4">
            <label for="userFilter">User (Name/Email):</label>
            <input type="text" id="userFilter" name="userFilter" 
                   class="form-control" value="@UserFilter" placeholder="e.g., john or john@example.com">
        </div>
        <div class="col-md-4">
            <label>&nbsp;</label>
            <button type="submit" class="btn btn-primary form-control">Apply Filters</button>
        </div>
    </div>
</form>

<!-- Orders Table -->
<table class="table table-striped">
    <thead>
        <tr>
            <th>User</th>
            <th>Email</th>
            <th>Item</th>
            <th>Quantity</th>
            <th>Status</th>
            <th>Ordered At</th>
            <th>Fulfilled At</th>
            <th>Actions</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var order in Orders?.Items ?? new())
        {
            <tr>
                <td>@order.Username</td>
                <td>@order.Email</td>
                <td>@order.ItemName</td>
                <td>@order.Quantity</td>
                <td>
                    <span class="badge" style="@GetStatusBadgeStyle(order.Status)">@order.Status</span>
                </td>
                <td>@order.OrderedAt:g</td>
                <td>@(order.FulfilledAt?.ToString("g") ?? "—")</td>
                <td><a href="details/@order.Id">View</a></td>
            </tr>
        }
    </tbody>
</table>

<!-- Pagination -->
@if (Orders?.TotalPages > 1)
{
    <!-- Pagination controls -->
}
```

### Linked Artifacts

- Design: ADR-ORDERS-001
- Tests: TC-ORD-025 – TC-ORD-028 (integration + filtering logic)
- Implementation: GetAllOrdersQuery enhancements, GetAllOrdersQueryHandler (filter logic)
