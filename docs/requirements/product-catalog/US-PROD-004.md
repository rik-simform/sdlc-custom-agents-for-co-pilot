# US-PROD-004: Get Product by ID

**Type**: Functional
**Priority**: Critical
**Story Points**: 3
**Source**: EPIC-PROD / PRD Section 6
**Status**: Ready

---

## User Story

**As a** Customer
**I want to** view a product's full details by its ID
**So that** I can make an informed decision about the product

---

## Acceptance Criteria

- [ ] **AC-001**: Given a valid product ID for an existing non-deleted product, when any user calls `GET /api/v1/products/{id}`, then the system returns HTTP 200 with the full product details including `Id`, `Name`, `Description`, `Price`, `CategoryId`, `CategoryName`, `ImageUrl`, and `CreatedAt`.
- [ ] **AC-002**: Given a non-existent or soft-deleted product ID, when any user calls `GET /api/v1/products/{id}`, then the system returns HTTP 404 Not Found with a ProblemDetails response.
- [ ] **AC-003**: Given a malformed GUID as the product ID, when any user calls `GET /api/v1/products/{id}`, then the system returns HTTP 400 Bad Request.
- [ ] **AC-004**: Given a product that has been fetched within the cache duration (60 seconds), when the same request is made, then the response is served from the output cache (verified by `Age` or cache header).
- [ ] **AC-005**: Given a product that was recently updated, when the cache is still active, then the cache is invalidated and the fresh data is returned on the next request.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | < 200ms (p95) with cache hit; < 500ms without cache |
| Security | Endpoint is publicly accessible (no authentication required for read) |
| Security | No sensitive fields exposed in response (no `IsDeleted`, `DeletedBy`, `RowVersion`) |
| Caching | Output cache with 60s sliding expiration; cache-busted on product write |
| Scalability | Supports 1000 concurrent readers without degradation |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-PROD-001 — Product entity and seed data exist | Story |
| DEP-002 | Category entity loaded via `Include` or projection | Technical |

---

## .NET Implementation Notes

### API Endpoint

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/api/v1/products/{id:guid}` | Anonymous | Get product details by ID |

### Output Caching

```csharp
app.MapGet("/api/v1/products/{id:guid}", GetProductById)
    .CacheOutput(policy => policy
        .Expire(TimeSpan.FromSeconds(60))
        .Tag("product-details"));
```

### Cache Invalidation on Write

```csharp
// In CreateProduct / UpdateProduct / DeleteProduct handlers:
await outputCacheStore.EvictByTagAsync("product-details", cancellationToken);
await outputCacheStore.EvictByTagAsync("product-list", cancellationToken);
```

### Response DTO

```csharp
public record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    Guid CategoryId,
    string CategoryName,
    string? ImageUrl,
    DateTime CreatedAt);
```

### Vertical Slice Path

`Features/Products/GetProduct/`

---

## Linked Artifacts

| Type | ID | Description |
|------|-----|------------|
| Tests | TC-PROD-019 | Valid ID returns 200 with full product |
| Tests | TC-PROD-020 | Non-existent ID returns 404 |
| Tests | TC-PROD-021 | Soft-deleted product returns 404 |
| Tests | TC-PROD-022 | Malformed GUID returns 400 |
| Tests | TC-PROD-023 | Second request within 60s served from cache |
| Tests | TC-PROD-024 | Cache invalidated after product update |

---

## Definition of Ready Checklist

- [x] Has unique ID (US-PROD-004)
- [x] Has ≥ 3 acceptance criteria (5 ACs)
- [x] Each AC is testable / automatable
- [x] Priority assigned (Critical)
- [x] Dependencies identified (2)
- [x] NFRs specified (5)
- [x] Story points estimated (3)
- [ ] Reviewed by Product Owner
