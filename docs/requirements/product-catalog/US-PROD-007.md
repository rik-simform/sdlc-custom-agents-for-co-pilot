# US-PROD-007: Sort Products by Price

**Type**: Functional
**Priority**: Medium
**Story Points**: 2
**Source**: EPIC-PROD / PRD Section 6
**Status**: Ready

---

## User Story

**As a** Customer
**I want to** sort product listings by price in ascending or descending order
**So that** I can find the cheapest or most expensive products easily

---

## Acceptance Criteria

- [ ] **AC-001**: Given `sortBy=price` and `sortDirection=asc`, when the user calls `GET /api/v1/products?sortBy=price&sortDirection=asc`, then the system returns products ordered by price ascending, paginated.
- [ ] **AC-002**: Given `sortBy=price` and `sortDirection=desc`, when the user calls `GET /api/v1/products?sortBy=price&sortDirection=desc`, then the system returns products ordered by price descending, paginated.
- [ ] **AC-003**: Given no `sortBy` parameter, when the user calls `GET /api/v1/products`, then the system defaults to sorting by `Name` ascending.
- [ ] **AC-004**: Given an unsupported `sortBy` value (e.g., `sortBy=rating`), when the user submits, then the system returns HTTP 400 with a validation error listing allowed sort fields.
- [ ] **AC-005**: Given sort combined with search and category filter, when the user submits, then all three operations (filter, search, sort) are applied correctly with pagination.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | Database index on `Price` column ensures sort < 500ms (p95) |
| Security | `sortBy` validated against allowlist; arbitrary column names rejected |
| Maintainability | Sort fields extensible (e.g., `name`, `createdAt`) via enum or allowlist |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-PROD-005 — List endpoint exists with pagination | Story |
| DEP-002 | Database index on `Products.Price` | Infrastructure |

---

## .NET Implementation Notes

### API Endpoint

Extends existing `GET /api/v1/products` endpoint with additional query parameters:

| Parameter | Type | Default | Constraints |
|-----------|------|---------|------------|
| `sortBy` | string? | `"name"` | Allowed: `name`, `price`, `createdAt` |
| `sortDirection` | string? | `"asc"` | Allowed: `asc`, `desc` |

### Sort Implementation

```csharp
query = (request.SortBy?.ToLowerInvariant(), request.SortDirection?.ToLowerInvariant()) switch
{
    ("price", "desc") => query.OrderByDescending(p => p.Price),
    ("price", "asc" or null) => query.OrderBy(p => p.Price),
    ("createdat", "desc") => query.OrderByDescending(p => p.CreatedAt),
    ("createdat", "asc" or null) => query.OrderBy(p => p.CreatedAt),
    ("name", "desc") => query.OrderByDescending(p => p.Name),
    _ => query.OrderBy(p => p.Name) // default
};
```

### FluentValidation Rules (added to ListProductsRequestValidator)

```csharp
RuleFor(x => x.SortBy)
    .Must(s => s is null or "name" or "price" or "createdAt")
    .WithMessage("SortBy must be one of: name, price, createdAt");

RuleFor(x => x.SortDirection)
    .Must(s => s is null or "asc" or "desc")
    .WithMessage("SortDirection must be 'asc' or 'desc'");
```

### Database Index

```csharp
modelBuilder.Entity<Product>()
    .HasIndex(p => p.Price)
    .HasDatabaseName("IX_Products_Price");
```

### Vertical Slice Path

Integrated into `Features/Products/ListProducts/` (extends the list endpoint)

---

## Linked Artifacts

| Type | ID | Description |
|------|-----|------------|
| Tests | TC-PROD-038 | Sort by price ascending returns ordered results |
| Tests | TC-PROD-039 | Sort by price descending returns ordered results |
| Tests | TC-PROD-040 | Default sort is name ascending |
| Tests | TC-PROD-041 | Invalid sortBy returns 400 |
| Tests | TC-PROD-042 | Sort combined with search and filter works correctly |

---

## Definition of Ready Checklist

- [x] Has unique ID (US-PROD-007)
- [x] Has ≥ 3 acceptance criteria (5 ACs)
- [x] Each AC is testable / automatable
- [x] Priority assigned (Medium)
- [x] Dependencies identified (2)
- [x] NFRs specified (3)
- [x] Story points estimated (2)
- [ ] Reviewed by Product Owner
