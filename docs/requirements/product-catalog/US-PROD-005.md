# US-PROD-005: List/Search Products

**Type**: Functional
**Priority**: Critical
**Story Points**: 8
**Source**: EPIC-PROD / PRD Section 6
**Status**: Ready

---

## User Story

**As a** Customer
**I want to** search for products by keyword with paginated results
**So that** I can quickly find the products I'm looking for without loading the entire catalog

---

## Acceptance Criteria

- [ ] **AC-001**: Given no search term, when any user calls `GET /api/v1/products`, then the system returns the first page of all non-deleted products, paginated with default `page=1` and `pageSize=20`.
- [ ] **AC-002**: Given a search term (e.g., `?search=laptop`), when the user calls `GET /api/v1/products?search=laptop`, then the system returns products whose `Name` or `Description` contains the search term (case-insensitive), paginated.
- [ ] **AC-003**: Given valid `page` and `pageSize` query parameters, when the user calls `GET /api/v1/products?page=2&pageSize=10`, then the system returns the correct page with up to `pageSize` items, along with pagination metadata: `totalCount`, `page`, `pageSize`, `totalPages`, `hasNextPage`, `hasPreviousPage`.
- [ ] **AC-004**: Given a `pageSize` greater than 50, when the user submits, then the system clamps it to 50 (max page size) and returns results accordingly.
- [ ] **AC-005**: Given a `page` beyond the total pages, when the user submits, then the system returns HTTP 200 with an empty `items` array and correct pagination metadata.
- [ ] **AC-006**: Given special characters in the search term (e.g., `'; DROP TABLE--`), when the user submits, then EF Core parameterized queries prevent SQL injection and the search returns empty or matching results safely.
- [ ] **AC-007**: Given the same search query executed within the output cache window, when the request is repeated, then the result is served from cache.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | < 500ms (p95) for catalogs up to 100k products with a search term |
| Performance | Database index on `Name` and `Description` for text search |
| Security | SQL injection prevention via EF Core parameterized queries (OWASP A03) |
| Security | Search input sanitized; max length 200 characters |
| Scalability | Cursor-based pagination considered for future; offset pagination in V1 |
| Caching | Output cache with 30s expiration, tagged for invalidation on writes |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-PROD-001 — Products exist in database | Story |
| DEP-002 | SQL Server full-text index (optional, performance optimization) | Infrastructure |

---

## .NET Implementation Notes

### API Endpoint

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/api/v1/products` | Anonymous | List/search products with pagination |

### Query Parameters

| Parameter | Type | Default | Constraints |
|-----------|------|---------|------------|
| `search` | string? | null | Max 200 chars |
| `page` | int | 1 | Min 1 |
| `pageSize` | int | 20 | Min 1, Max 50 |

### Response Model

```csharp
public record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage);
```

### Query Construction

```csharp
var query = dbContext.Products
    .AsNoTracking()
    .Include(p => p.Category)
    .AsQueryable();

if (!string.IsNullOrWhiteSpace(request.Search))
{
    var searchTerm = request.Search.Trim();
    query = query.Where(p =>
        p.Name.Contains(searchTerm) ||
        p.Description.Contains(searchTerm));
}

var totalCount = await query.CountAsync(cancellationToken);
var items = await query
    .OrderBy(p => p.Name)
    .Skip((request.Page - 1) * request.PageSize)
    .Take(request.PageSize)
    .Select(p => new ProductResponse(
        p.Id, p.Name, p.Description, p.Price,
        p.CategoryId, p.Category.Name, p.ImageUrl, p.CreatedAt))
    .ToListAsync(cancellationToken);
```

### FluentValidation Rules

```csharp
public class ListProductsRequestValidator : AbstractValidator<ListProductsRequest>
{
    public ListProductsRequestValidator()
    {
        RuleFor(x => x.Search)
            .MaximumLength(200).WithMessage("Search term must not exceed 200 characters");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50).WithMessage("Page size must be between 1 and 50");
    }
}
```

### Vertical Slice Path

`Features/Products/ListProducts/`

---

## Linked Artifacts

| Type | ID | Description |
|------|-----|------------|
| Design | ADR-PROD-004 | Pagination strategy (offset vs cursor) |
| Tests | TC-PROD-025 | No search term returns paginated products |
| Tests | TC-PROD-026 | Search term filters by name match |
| Tests | TC-PROD-027 | Search term filters by description match |
| Tests | TC-PROD-028 | Pagination metadata is correct |
| Tests | TC-PROD-029 | Page beyond total returns empty items |
| Tests | TC-PROD-030 | PageSize clamped to 50 |
| Tests | TC-PROD-031 | SQL injection attempt returns safely |
| Tests | TC-PROD-032 | Cached search served from output cache |

---

## Definition of Ready Checklist

- [x] Has unique ID (US-PROD-005)
- [x] Has ≥ 3 acceptance criteria (7 ACs)
- [x] Each AC is testable / automatable
- [x] Priority assigned (Critical)
- [x] Dependencies identified (2)
- [x] NFRs specified (6)
- [x] Story points estimated (8)
- [ ] Reviewed by Product Owner
