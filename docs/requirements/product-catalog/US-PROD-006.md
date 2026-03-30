# US-PROD-006: Filter Products by Category

**Type**: Functional
**Priority**: High
**Story Points**: 3
**Source**: EPIC-PROD / PRD Section 6
**Status**: Ready

---

## User Story

**As a** Customer
**I want to** filter products by category
**So that** I can narrow down results to the product type I'm interested in

---

## Acceptance Criteria

- [ ] **AC-001**: Given a valid `categoryId` query parameter, when the user calls `GET /api/v1/products?categoryId={guid}`, then the system returns only products belonging to that category, paginated.
- [ ] **AC-002**: Given a `categoryId` that does not exist, when the user submits, then the system returns HTTP 200 with an empty `items` array and `totalCount = 0` (no error — the filter simply matches nothing).
- [ ] **AC-003**: Given both `categoryId` and `search` query parameters, when the user submits, then the system applies both filters (AND logic) and returns products matching the search term within the specified category.
- [ ] **AC-004**: Given a `categoryId` filter combined with sort parameters, when the user submits, then the filter, sort, and pagination are all applied correctly together.
- [ ] **AC-005**: Given a malformed GUID as `categoryId`, when the user submits, then the system returns HTTP 400 Bad Request with a validation error.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | < 500ms (p95) — leverages index on `CategoryId` column |
| Security | `categoryId` validated as GUID format; no raw SQL construction |
| Caching | Filtered results cached independently, tagged for invalidation |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-PROD-005 — List/search endpoint exists and supports query parameters | Story |
| DEP-002 | US-PROD-009 — Categories seeded in database | Story |
| DEP-003 | Database index on `Products.CategoryId` | Infrastructure |

---

## .NET Implementation Notes

### API Endpoint

Extends existing `GET /api/v1/products` endpoint with additional query parameter:

| Parameter | Type | Default | Constraints |
|-----------|------|---------|------------|
| `categoryId` | Guid? | null | Valid GUID or omitted |

### Query Extension

```csharp
if (request.CategoryId.HasValue)
{
    query = query.Where(p => p.CategoryId == request.CategoryId.Value);
}
```

### Database Index

```csharp
modelBuilder.Entity<Product>()
    .HasIndex(p => p.CategoryId)
    .HasDatabaseName("IX_Products_CategoryId");
```

### Vertical Slice Path

Integrated into `Features/Products/ListProducts/` (extends the list endpoint)

---

## Linked Artifacts

| Type | ID | Description |
|------|-----|------------|
| Tests | TC-PROD-033 | Valid categoryId returns filtered products |
| Tests | TC-PROD-034 | Non-existent categoryId returns empty results |
| Tests | TC-PROD-035 | Category filter combined with search (AND logic) |
| Tests | TC-PROD-036 | Category filter combined with sort |
| Tests | TC-PROD-037 | Malformed categoryId returns 400 |

---

## Definition of Ready Checklist

- [x] Has unique ID (US-PROD-006)
- [x] Has ≥ 3 acceptance criteria (5 ACs)
- [x] Each AC is testable / automatable
- [x] Priority assigned (High)
- [x] Dependencies identified (3)
- [x] NFRs specified (3)
- [x] Story points estimated (3)
- [ ] Reviewed by Product Owner
