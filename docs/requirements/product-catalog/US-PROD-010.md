# US-PROD-010: Product Audit Trail

**Type**: Functional
**Priority**: High
**Story Points**: 5
**Source**: EPIC-PROD / PRD Section 6
**Status**: Ready

---

## User Story

**As a** System Administrator
**I want to** have all product and category changes recorded in a tamper-evident audit trail
**So that** I can investigate issues, satisfy compliance requirements, and detect unauthorized modifications

---

## Acceptance Criteria

- [ ] **AC-001**: Given a successful product creation, when the operation completes, then an `AuditEntry` is written with `EntityType = "Product"`, `Action = "Created"`, `EntityId = {productId}`, `UserId = {adminId}`, `Timestamp = {utcNow}`, and `NewValues = {JSON snapshot of created entity}`.
- [ ] **AC-002**: Given a successful product update, when the operation completes, then an `AuditEntry` is written with `Action = "Updated"`, `OldValues = {JSON of previous state}`, and `NewValues = {JSON of new state}`.
- [ ] **AC-003**: Given a successful product soft-delete, when the operation completes, then an `AuditEntry` is written with `Action = "Deleted"` and the product ID.
- [ ] **AC-004**: Given a successful category create/update/delete, when the operation completes, then a corresponding `AuditEntry` is written with `EntityType = "Category"`.
- [ ] **AC-005**: Given the audit trail service is unavailable (e.g., database write fails), when a product mutation occurs, then the mutation itself is NOT rolled back — the audit write failure is logged at `Error` level and a background retry is scheduled, but the user operation succeeds.
- [ ] **AC-006**: Given an authenticated Admin user, when they call `GET /api/v1/admin/audit?entityType=Product&entityId={id}`, then the system returns all audit entries for that entity in reverse chronological order, paginated.
- [ ] **AC-007**: Given audit entries, the `UserId` field always corresponds to the authenticated user's `sub` claim from the JWT — it is never null for write operations.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | Audit write adds < 50ms overhead to the parent operation |
| Security | Audit entries are append-only; no update or delete API exposed (OWASP A09) |
| Security | Audit endpoint requires `Admin` role |
| Security | Sensitive data (e.g., internal IDs) not logged beyond what's necessary |
| Reliability | Audit write failure does not block the business operation |
| Compliance | Audit entries retained for minimum 1 year (configurable) |
| Data Integrity | `AuditEntry` uses server-generated `Id` and `Timestamp` — not client-supplied |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-PROD-001 / US-PROD-002 / US-PROD-003 — Product write operations | Story |
| DEP-002 | US-PROD-009 — Category write operations | Story |
| DEP-003 | US-LOGIN-001 — JWT `sub` claim for user identification | Cross-Epic |

---

## .NET Implementation Notes

### API Endpoint

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/api/v1/admin/audit` | `Admin` | Query audit entries with filters |

### Query Parameters

| Parameter | Type | Default | Constraints |
|-----------|------|---------|------------|
| `entityType` | string? | null | `Product` or `Category` |
| `entityId` | Guid? | null | Specific entity |
| `action` | string? | null | `Created`, `Updated`, `Deleted` |
| `page` | int | 1 | Min 1 |
| `pageSize` | int | 20 | Min 1, Max 100 |

### Key Entity

```csharp
public class AuditEntry
{
    public Guid Id { get; set; }
    public string EntityType { get; set; } = string.Empty;   // "Product" | "Category"
    public Guid EntityId { get; set; }
    public string Action { get; set; } = string.Empty;       // "Created" | "Updated" | "Deleted"
    public string UserId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? OldValues { get; set; }                    // JSON snapshot
    public string? NewValues { get; set; }                    // JSON snapshot
}
```

### EF Core SaveChanges Interceptor

```csharp
public class AuditInterceptor(IHttpContextAccessor httpContextAccessor)
    : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context is null) return result;

        var userId = httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";

        var auditEntries = context.ChangeTracker.Entries()
            .Where(e => e.Entity is Product or Category)
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(e => CreateAuditEntry(e, userId))
            .ToList();

        context.Set<AuditEntry>().AddRange(auditEntries);

        return result;
    }
}
```

### Vertical Slice Path

```
Features/Audit/
├── QueryAuditEntries/
│   ├── QueryAuditEntriesEndpoint.cs
│   ├── QueryAuditEntriesRequest.cs
│   └── QueryAuditEntriesHandler.cs
└── Shared/
    ├── AuditEntry.cs
    ├── AuditInterceptor.cs
    └── IAuditService.cs
```

---

## Linked Artifacts

| Type | ID | Description |
|------|-----|------------|
| Design | ADR-PROD-006 | Audit trail strategy (interceptor vs explicit service) |
| Tests | TC-PROD-057 | Product create writes audit entry with correct fields |
| Tests | TC-PROD-058 | Product update writes audit with old and new values |
| Tests | TC-PROD-059 | Product delete writes audit entry |
| Tests | TC-PROD-060 | Category mutations write audit entries |
| Tests | TC-PROD-061 | Audit failure does not block product write |
| Tests | TC-PROD-062 | Audit query endpoint returns filtered, paginated results |
| Tests | TC-PROD-063 | Audit entries always have non-null UserId |

---

## Definition of Ready Checklist

- [x] Has unique ID (US-PROD-010)
- [x] Has ≥ 3 acceptance criteria (7 ACs)
- [x] Each AC is testable / automatable
- [x] Priority assigned (High)
- [x] Dependencies identified (3)
- [x] NFRs specified (7)
- [x] Story points estimated (5)
- [ ] Reviewed by Product Owner
