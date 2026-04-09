# ADR-ORDERS-001: Order Management System — Architecture Decision Record

**Date**: 2026-04-08  
**Status**: Accepted  
**Context**: Order Management Feature Implementation  
**Decision**: Implement Order Management using Clean Architecture with vertical slices per feature  

---

## The Question

How should we design and implement the Order Management System to meet requirements for user order placement, user order history, admin order visibility, and filtering while maintaining clean architecture principles and scalability?

---

## Environment

- **Framework**: ASP.NET Core 8.0+
- **Architecture**: Clean Architecture with Vertical Slices
- **Data Access**: Entity Framework Core with SQL Server
- **API**: Minimal APIs with MediatR (CQRS)
- **Authentication**: JWT with ASP.NET Identity
- **Authorization**: Role-based (User, Admin)

---

## Decision

We will implement the Order Management System using the following architecture:

### 1. **Domain Layer: Order Entity**

```csharp
public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; } // FK to AspNetUsers
    public int InventoryItemId { get; set; } // FK to InventoryItems
    public int Quantity { get; set; }
    public OrderStatus Status { get; set; } // Enum: Pending, Processing, Fulfilled, Cancelled
    public DateTimeOffset OrderedAt { get; set; }
    public DateTimeOffset? FulfilledAt { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; }
    public virtual InventoryItem InventoryItem { get; set; }
}

public enum OrderStatus
{
    Pending = 1,
    Processing = 2,
    Fulfilled = 3,
    Cancelled = 4
}
```

**Rationale**: 
- Auditability via `CreatedAt` / `UpdatedAt` timestamps
- Status enum enables type-safe workflows
- Nullable `FulfilledAt` indicates pending vs. completed orders
- Navigation properties enable eager loading for query performance

### 2. **Database Schema: Orders Table**

```sql
CREATE TABLE [dbo].[Orders] (
    [Id] INT PRIMARY KEY IDENTITY(1,1),
    [UserId] NVARCHAR(450) NOT NULL,
    [InventoryItemId] INT NOT NULL,
    [Quantity] INT NOT NULL,
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    [OrderedAt] DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    [FulfilledAt] DATETIMEOFFSET NULL,
    [Notes] NVARCHAR(500) NULL,
    [CreatedAt] DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIMEOFFSET NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [FK_Orders_AspNetUsers_UserId] 
        FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Orders_InventoryItems_InventoryItemId] 
        FOREIGN KEY ([InventoryItemId]) REFERENCES [dbo].[InventoryItems]([Id]) ON DELETE CASCADE,
    
    CREATE INDEX [IX_Orders_UserId] ON [dbo].[Orders]([UserId]);
    CREATE INDEX [IX_Orders_Status] ON [dbo].[Orders]([Status]);
    CREATE INDEX [IX_Orders_OrderedAt] ON [dbo].[Orders]([OrderedAt] DESC);
);
```

**Rationale**:
- Indices on UserId enable fast user-specific queries
- Status index accelerates filtering by order state
- OrderedAt index enables efficient "recent orders" queries
- Cascading deletes maintain referential integrity

### 3. **Repository Pattern: IOrderRepository**

```csharp
public interface IOrderRepository
{
    Task<Order> GetByIdAsync(int id);
    Task<IEnumerable<Order>> GetByUserIdAsync(string userId, int skip = 0, int take = 10);
    Task<IEnumerable<Order>> GetAllAsync(int skip = 0, int take = 10, 
                                          OrderStatus? status = null, string? userFilter = null);
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task DeleteAsync(int id);
}
```

**Rationale**:
- Abstract data access from business logic
- Enable easy repository swapping for testing (Moq)
- Methods include pagination parameters for scalability
- Optional filters support future enhancements (status, user filtering)

### 4. **CQRS Pattern: Commands and Queries**

**Commands** (state-changing operations):
- `CreateOrderCommand` / `CreateOrderCommandHandler` — Place new order
- `CancelOrderCommand` / `CancelOrderCommandHandler` — Cancel pending order

**Queries** (read-only operations):
- `GetUserOrdersQuery` / `GetUserOrdersQueryHandler` — Get user's order history
- `GetAllOrdersQuery` / `GetAllOrdersQueryHandler` — Get all orders (admin only)

**Rationale**:
- Separation of concerns: commands modify state, queries only read
- Enables future scaling (event sourcing, command auditing)
- Queries can be independently optimized (e.g., separate read model)

### 5. **Data Transfer Objects (DTOs)**

```csharp
// Requests
public record CreateOrderRequest(
    int InventoryItemId,
    int Quantity,
    string? Notes
);

// Responses
public record OrderResponse(
    int Id,
    string ItemName,
    int Quantity,
    string Status,
    DateTimeOffset OrderedAt,
    DateTimeOffset? FulfilledAt,
    string? Notes
);

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

**Rationale**:
- User-facing models expose only necessary data (security)
- Admin models include PII (username, email) for identification
- Separate request/response models enable versioning

### 6. **API Endpoints: REST Design**

| Method | Route | Auth | Description |
|--------|-------|------|------------|
| POST | `/api/v1/orders/` | User | Create order |
| GET | `/api/v1/orders/my-orders` | User | Get user's orders |
| GET | `/api/v1/orders/` | Admin | Get all orders (admin only) |
| PUT | `/api/v1/orders/{id}/cancel` | User | Cancel pending order |

**Rationale**:
- RESTful semantics: POST for create, GET for read, PUT for state update
- Separate `/my-orders` endpoint clarifies user vs. admin scope
- Minimal APIs with MapGroup for consistent routing

### 7. **Authorization Strategy**

```csharp
// User endpoints
group.MapPost("/", CreateOrder)
    .RequireAuthorization();  // Any authenticated user

group.MapGet("/my-orders", GetUserOrders)
    .RequireAuthorization();  // Any authenticated user, filtered by JWT sub

group.MapPut("/{id}/cancel", CancelOrder)
    .RequireAuthorization();  // Any authenticated user, validated ownership

// Admin endpoints
group.MapGet("/", GetAllOrders)
    .RequireAuthorization(policy => policy.RequireRole("Admin"));

```

**Rationale**:
- Authorization enforced at endpoint (Minimal API `.RequireAuthorization()`)
- User queries filtered by JWT `sub` claim (UserId)
- Admin role validated via ASP.NET Identity
- Command handlers validate ownership (business logic security)

### 8. **Validation Strategy**

All inputs validated using **FluentValidation**:

```csharp
public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.InventoryItemId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0).LessThanOrEqualTo(999);
        RuleFor(x => x.Notes).MaximumLength(500).When(x => x.Notes != null);
    }
}

public class GetAllOrdersQueryValidator : AbstractValidator<GetAllOrdersQuery>
{
    public GetAllOrdersQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
        RuleFor(x => x.Status)
            .Must(s => string.IsNullOrEmpty(s) || 
                       Enum.TryParse<OrderStatus>(s, ignoreCase: true, out _));
    }
}
```

**Rationale**:
- Declarative, readable validation rules
- Reusable validators for commands and queries
- Prevents invalid state transitions and data corruption

### 9. **Pagination & Scalability**

All list endpoints implement **offset-based pagination**:

```csharp
// Query parameters
page=1, pageSize=10  // Default: page 1, 10 items

// Response includes metadata
{
    "items": [...],
    "currentPage": 1,
    "pageSize": 10,
    "totalCount": 42,
    "totalPages": 5
}
```

**Rationale**:
- Prevents unbounded queries on large datasets
- MaxPageSize enforced (100) to prevent DOS
- Metadata enables client-side pagination UI

### 10. **Error Handling & HTTP Status Codes**

| Status | Scenario | Example |
|--------|----------|---------|
| 201 | Resource created successfully | Create order |
| 200 | Successful read/update | Retrieve orders, cancel order |
| 400 | Validation failure or business logic violation | Invalid quantity, insufficient inventory |
| 401 | Missing/invalid JWT | Unauthenticated request |
| 403 | Insufficient permissions | Non-admin accessing admin endpoint, user viewing another's order |
| 404 | Resource not found | Order ID doesn't exist |

**Rationale**:
- Standard HTTP semantics enable client error handling
- Consistent error responses via Problem Details RFC 7807

### 11. **Testing Strategy**

| Test Level | Coverage | Tools |
|------------|----------|-------|
| Unit | Business logic (validators, command handlers) | MSTest, FluentAssertions, Moq |
| Integration | End-to-end API flows | WebApplicationFactory, in-memory DB |
| Acceptance | User stories verification | Manual + future Playwright E2E |

**Test File Structure**:
```
tests/MyProject.UnitTests/Features/Orders/
├── Commands/
│   ├── CreateOrderCommandHandlerTests.cs
│   └── CancelOrderCommandHandlerTests.cs
├── Queries/
│   ├── GetUserOrdersQueryHandlerTests.cs
│   └── GetAllOrdersQueryHandlerTests.cs
├── Validators/
│   └── OrderValidatorsTests.cs
└── Repository/
    └── OrderRepositoryTests.cs
```

---

## Alternatives Considered

### Alternative 1: Single Service Class (No CQRS)

```csharp
public class OrderService
{
    public async Task<OrderResponse> CreateOrder(CreateOrderRequest request);
    public async Task<List<OrderResponse>> GetUserOrders(string userId, ...);
    // ... mix of commands and queries
}
```

**Pros**: Simpler for small features  
**Cons**: Violates Single Responsibility; hard to scale independent command/query optimization  
**Rejected**: CQRS enables future event sourcing and independent scaling

### Alternative 2: Table-Per-Tenant (Multi-Tenancy)

**Rationale for Rejection**: Currently single-tenant; premature optimization; future ADR if needed

### Alternative 3: Soft Delete vs. Hard Delete

**Decision**: Hard delete with cascading referential integrity  
**Rationale**: Orders are historical records; no regulatory retention requirements currently; future ADR if audit mandates soft deletes

---

## Consequences

### Positive

✅ **Clean Architecture** separates concerns (domain, application, infrastructure, presentation)  
✅ **CQRS** enables independent optimization of reads/writes  
✅ **Repository Pattern** decouples data access; testable  
✅ **Validation** at multiple levels (FluentValidation, Business Logic)  
✅ **Pagination** prevents DOS and scales to thousands of orders  
✅ **Role-Based Authorization** enforces security at endpoint level  
✅ **DTOs** prevent unintended data leakage  

### Challenges

⚠️ **Complexity**: More classes/files (Commands, Queries, Handlers, Validators)  
⚠️ **Learning Curve**: Team must understand CQRS, MediatR if unfamiliar  
⚠️ **Performance**: N+1 queries possible without careful eager loading (mitigated by EF `.Include()`)  

### Mitigation

- Comprehensive documentation (this ADR)
- Code review checklist ensures eager loading practices
- Monitoring/APM to catch performance regressions

---

## Implementation Checklist

- [ ] Create Order entity in Domain layer
- [ ] Create IOrderRepository interface
- [ ] Create OrderRepository implementation
- [ ] Create EF Core OrderConfiguration
- [ ] Create database migration
- [ ] Create CQRS commands/queries and handlers
- [ ] Create DTOs and validators
- [ ] Create OrderEndpoints with routes
- [ ] Register dependencies (DI)
- [ ] Wire endpoints in Program.cs
- [ ] Create Razor Pages UI (Orders list, Admin dashboard)
- [ ] Implement unit tests (40+ test cases)
- [ ] Implement integration tests (API + auth validation)
- [ ] Manual E2E test of user order placement → admin view flow
- [ ] Documentation review and sign-off

---

## Related Decisions

- **ADR-LOGIN-001**: JWT authentication and ASP.NET Identity roles
- **ADR-RBAC-001**: Role-based authorization for inventory management
- **ADR-ARCHITECTURE-001**: Clean Architecture and Vertical Slices (project baseline)

---

## References

- Clean Architecture: Robert C. Martin
- CQRS Pattern: https://martinfowler.com/bliki/CQRS.html
- MediatR Documentation: https://github.com/jbogard/MediatR
- FluentValidation: https://docs.fluentvalidation.net
- Entity Framework Core: https://docs.microsoft.com/en-us/ef/core/
- REST API Design Best Practices: https://restfulapi.net/

