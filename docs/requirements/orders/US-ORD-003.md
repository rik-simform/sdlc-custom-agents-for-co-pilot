# US-ORD-003: User Cancels Pending Order

**Type**: Functional
**Priority**: High
**Story Points**: 3
**Source**: EPIC-ORD / PRD Section 6
**Status**: Ready

---

## User Story

**As a** Customer (User role)
**I want to** cancel my pending orders within a remorse window
**So that** I can avoid unwanted purchases if I change my mind

---

## Acceptance Criteria

- [ ] **AC-012**: Given an authenticated user and a Pending order they own, when they call `PUT /api/v1/orders/{id}/cancel`, then the system returns HTTP 200 OK with the updated `OrderResponse` showing `Status = Cancelled` and `UpdatedAt` set to the current timestamp.

- [ ] **AC-013**: Given an order with status other than Pending (e.g., Processing, Fulfilled, Cancelled), when the user attempts to cancel, then the system returns HTTP 400 with error message "Only pending orders can be cancelled."

- [ ] **AC-014**: Given a successful cancellation, the Order entity's `Status` field is updated to `Cancelled` and `UpdatedAt` is refreshed.

- [ ] **AC-015**: Given a request for a non-existent order ID, when the user calls `PUT /api/v1/orders/{id}/cancel`, then the system returns HTTP 404 Not Found.

- [ ] **AC-016**: Given a user attempting to cancel an order they do not own (different UserId), then the system returns HTTP 403 Forbidden with error message "You do not have permission to cancel this order."

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | Cancel endpoint responds < 500ms (p95) |
| Security | User can only cancel their own orders; ownership validated before update |
| Security | Order ID is validated to exist |
| Security | Status is validated to be Pending before allowing cancellation |
| Transactionality | Status update is atomic (single transaction) |
| Audit | Order modification (Status change) could be logged for audit trail |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-ORD-001 — User can place orders | Story |
| DEP-002 | US-ORD-002 — User can view orders | Story |
| DEP-003 | US-LOGIN-001 — JWT authentication | Cross-Epic |

---

## Implementation Notes

### API Endpoint

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| PUT | `/api/v1/orders/{id}/cancel` | Required | Cancel a pending order |

### Request/Response Models

**CancelOrderRequest** (empty body, ID in URL)
```csharp
// No request body required; ID from URL parameter
```

**OrderResponse** (standard, with updated Status)
```csharp
public record OrderResponse(
    int Id,
    string ItemName,
    int Quantity,
    string Status,           // Will be "Cancelled"
    DateTimeOffset OrderedAt,
    DateTimeOffset? FulfilledAt,
    string? Notes
);
```

### Handler Logic (Pseudocode)

```csharp
public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(CancelOrderCommand request, ...)
    {
        // 1. Extract UserId from JWT
        // 2. Fetch Order by ID
        // 3. If not found, return 404
        // 4. If UserId != Order.UserId, return 403 Forbidden
        // 5. If Status != Pending, return 400 with message
        // 6. Set Status = Cancelled, UpdatedAt = now
        // 7. Save to database
        // 8. Return 200 OK with updated OrderResponse
    }
}
```

### Validation Rules

```csharp
public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).GreaterThan(0)
            .WithMessage("OrderId must be greater than 0");
    }
}
```

### Web UI (Razor Page)

**Pages/Orders/Index.cshtml**
- "Cancel" button appears only for Pending orders
- Button is greyed out / hidden for non-Pending statuses
- Clicking button shows confirmation dialog: "Cancel this order?"
- On confirm, sends PUT request to `/api/v1/orders/{id}/cancel`
- On success, refreshes order list or shows updated status

**Error Handling**
- 400 error shown as alert: "Cannot cancel this order (status is already Processing/Fulfilled/Cancelled)"
- 403 error shown as alert: "You don't have permission to cancel this order"
- 404 error shown as alert: "Order not found"

### Linked Artifacts

- Design: ADR-ORDERS-001
- Tests: TC-ORD-013 – TC-ORD-018 (unit + integration)
- Implementation: CancelOrderCommand, CancelOrderCommandHandler, OrderEndpoints (PUT cancel handler)
