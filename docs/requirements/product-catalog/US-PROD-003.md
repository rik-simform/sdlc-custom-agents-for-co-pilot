# US-PROD-003: Delete Product (Admin Only)

**Type**: Functional
**Priority**: High
**Story Points**: 3
**Source**: EPIC-PROD / PRD Section 6
**Status**: Ready

---

## User Story

**As an** Admin user
**I want to** soft-delete a product so it is hidden from customers but retained for auditing
**So that** the catalog stays clean while maintaining data integrity and a complete audit history

---

## Acceptance Criteria

- [ ] **AC-001**: Given an authenticated Admin user and an existing product ID, when they call `DELETE /api/v1/products/{id}`, then the system sets `IsDeleted = true` and `DeletedAt` / `DeletedBy` on the product entity, and returns HTTP 204 No Content.
- [ ] **AC-002**: Given a soft-deleted product, when any user calls `GET /api/v1/products/{id}`, then the system returns HTTP 404 Not Found (global query filter excludes soft-deleted records).
- [ ] **AC-003**: Given a soft-deleted product, when any user performs a search or list, then the deleted product does NOT appear in results.
- [ ] **AC-004**: Given a request with a non-existent or already-deleted product ID, when the Admin submits, then the system returns HTTP 404 Not Found.
- [ ] **AC-005**: Given a request from a non-admin user, when they call `DELETE /api/v1/products/{id}`, then the system returns HTTP 403 Forbidden.
- [ ] **AC-006**: Given a successful soft-delete, when the operation completes, then an audit entry is written with `Action = "ProductDeleted"`, the product ID, and the Admin user ID.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | Delete endpoint responds < 500ms (p95) |
| Security | Only `Admin` role can invoke; enforced via authorization policy |
| Security | Soft delete preserves data for compliance and audit (no data loss) |
| Data Integrity | Global query filter `HasQueryFilter(p => !p.IsDeleted)` prevents leakage |
| Recoverability | Soft-deleted records can be restored via direct DB intervention (ops runbook) |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-PROD-001 — Product entity and table must exist | Story |
| DEP-002 | US-LOGIN-001 — JWT authentication middleware | Cross-Epic |
| DEP-003 | US-PROD-010 — Audit trail service available | Story |

---

## .NET Implementation Notes

### API Endpoint

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| DELETE | `/api/v1/products/{id:guid}` | `Admin` | Soft-delete a product |

### Entity Additions

```csharp
// Added to Product entity
public bool IsDeleted { get; set; }
public DateTime? DeletedAt { get; set; }
public string? DeletedBy { get; set; }
```

### EF Core Global Query Filter

```csharp
modelBuilder.Entity<Product>()
    .HasQueryFilter(p => !p.IsDeleted);
```

### Handler Logic

```csharp
// Soft delete — no physical removal
var product = await dbContext.Products.FindAsync(id);
if (product is null) return Result.NotFound();

product.IsDeleted = true;
product.DeletedAt = DateTime.UtcNow;
product.DeletedBy = currentUser.Id;

await dbContext.SaveChangesAsync(cancellationToken);
```

### Vertical Slice Path

`Features/Products/DeleteProduct/`

---

## Linked Artifacts

| Type | ID | Description |
|------|-----|------------|
| Design | ADR-PROD-002 | Soft delete vs hard delete decision |
| Tests | TC-PROD-013 | Valid delete sets IsDeleted and returns 204 |
| Tests | TC-PROD-014 | Deleted product returns 404 on GET |
| Tests | TC-PROD-015 | Deleted product excluded from search results |
| Tests | TC-PROD-016 | Already-deleted product returns 404 |
| Tests | TC-PROD-017 | Non-admin returns 403 |
| Tests | TC-PROD-018 | Audit entry written on soft-delete |

---

## Definition of Ready Checklist

- [x] Has unique ID (US-PROD-003)
- [x] Has ≥ 3 acceptance criteria (6 ACs)
- [x] Each AC is testable / automatable
- [x] Priority assigned (High)
- [x] Dependencies identified (3)
- [x] NFRs specified (5)
- [x] Story points estimated (3)
- [ ] Reviewed by Product Owner
