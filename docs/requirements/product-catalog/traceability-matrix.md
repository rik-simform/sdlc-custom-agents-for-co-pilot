# Requirements Traceability Matrix — EPIC-PROD

Last Updated: 2026-03-26

---

## Forward Traceability (Requirement → Verification)

| Req ID | Title | Priority | Design | Implementation | Test Cases | Status |
|--------|-------|----------|--------|----------------|------------|--------|
| US-PROD-001 | Create Product (Admin) | Critical | ADR-PROD-001, ADR-PROD-002 | — | TC-PROD-001 – TC-PROD-006 | ⏳ Pending |
| US-PROD-002 | Update Product (Admin) | Critical | ADR-PROD-003 | — | TC-PROD-007 – TC-PROD-012 | ⏳ Pending |
| US-PROD-003 | Delete Product (Admin) | High | ADR-PROD-002 | — | TC-PROD-013 – TC-PROD-018 | ⏳ Pending |
| US-PROD-004 | Get Product by ID | Critical | — | — | TC-PROD-019 – TC-PROD-024 | ⏳ Pending |
| US-PROD-005 | List/Search Products | Critical | ADR-PROD-004 | — | TC-PROD-025 – TC-PROD-032 | ⏳ Pending |
| US-PROD-006 | Filter by Category | High | — | — | TC-PROD-033 – TC-PROD-037 | ⏳ Pending |
| US-PROD-007 | Sort by Price | Medium | — | — | TC-PROD-038 – TC-PROD-042 | ⏳ Pending |
| US-PROD-008 | Image URL Validation | Medium | — | — | TC-PROD-043 – TC-PROD-048 | ⏳ Pending |
| US-PROD-009 | Category Management | High | ADR-PROD-005 | — | TC-PROD-049 – TC-PROD-056 | ⏳ Pending |
| US-PROD-010 | Product Audit Trail | High | ADR-PROD-006 | — | TC-PROD-057 – TC-PROD-063 | ⏳ Pending |

---

## Backward Traceability (Test → Requirement)

### US-PROD-001: Create Product

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-PROD-001 | Valid request creates product and returns 201 | Integration | US-PROD-001/AC-001 | — | ⏳ Pending |
| TC-PROD-002 | Duplicate name+category returns 409 | Integration | US-PROD-001/AC-002 | — | ⏳ Pending |
| TC-PROD-003 | Invalid fields return 400 with ProblemDetails | Unit | US-PROD-001/AC-003 | — | ⏳ Pending |
| TC-PROD-004 | Unauthenticated request returns 401 | Integration | US-PROD-001/AC-004 | — | ⏳ Pending |
| TC-PROD-005 | Non-admin role returns 403 | Integration | US-PROD-001/AC-005 | — | ⏳ Pending |
| TC-PROD-006 | Audit entry written on successful create | Integration | US-PROD-001/AC-006, US-PROD-010/AC-001 | — | ⏳ Pending |

### US-PROD-002: Update Product

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-PROD-007 | Valid update returns 200 with new RowVersion | Integration | US-PROD-002/AC-001 | — | ⏳ Pending |
| TC-PROD-008 | Stale RowVersion returns 409 Conflict | Integration | US-PROD-002/AC-002 | — | ⏳ Pending |
| TC-PROD-009 | Non-existent product returns 404 | Integration | US-PROD-002/AC-003 | — | ⏳ Pending |
| TC-PROD-010 | Invalid fields return 400 | Unit | US-PROD-002/AC-004 | — | ⏳ Pending |
| TC-PROD-011 | Unauthorized/forbidden returns 401/403 | Integration | US-PROD-002/AC-005 | — | ⏳ Pending |
| TC-PROD-012 | Audit entry written on successful update | Integration | US-PROD-002/AC-006, US-PROD-010/AC-002 | — | ⏳ Pending |

### US-PROD-003: Delete Product

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-PROD-013 | Valid delete sets IsDeleted and returns 204 | Integration | US-PROD-003/AC-001 | — | ⏳ Pending |
| TC-PROD-014 | Deleted product returns 404 on GET | Integration | US-PROD-003/AC-002, US-PROD-004/AC-002 | — | ⏳ Pending |
| TC-PROD-015 | Deleted product excluded from search results | Integration | US-PROD-003/AC-003, US-PROD-005/AC-001 | — | ⏳ Pending |
| TC-PROD-016 | Already-deleted product returns 404 | Integration | US-PROD-003/AC-004 | — | ⏳ Pending |
| TC-PROD-017 | Non-admin returns 403 | Integration | US-PROD-003/AC-005 | — | ⏳ Pending |
| TC-PROD-018 | Audit entry written on soft-delete | Integration | US-PROD-003/AC-006, US-PROD-010/AC-003 | — | ⏳ Pending |

### US-PROD-004: Get Product by ID

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-PROD-019 | Valid ID returns 200 with full product | Integration | US-PROD-004/AC-001 | — | ⏳ Pending |
| TC-PROD-020 | Non-existent ID returns 404 | Integration | US-PROD-004/AC-002 | — | ⏳ Pending |
| TC-PROD-021 | Soft-deleted product returns 404 | Integration | US-PROD-004/AC-002 | — | ⏳ Pending |
| TC-PROD-022 | Malformed GUID returns 400 | Unit | US-PROD-004/AC-003 | — | ⏳ Pending |
| TC-PROD-023 | Second request within 60s served from cache | Integration | US-PROD-004/AC-004 | — | ⏳ Pending |
| TC-PROD-024 | Cache invalidated after product update | Integration | US-PROD-004/AC-005 | — | ⏳ Pending |

### US-PROD-005: List/Search Products

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-PROD-025 | No search term returns paginated products | Integration | US-PROD-005/AC-001 | — | ⏳ Pending |
| TC-PROD-026 | Search term filters by name match | Integration | US-PROD-005/AC-002 | — | ⏳ Pending |
| TC-PROD-027 | Search term filters by description match | Integration | US-PROD-005/AC-002 | — | ⏳ Pending |
| TC-PROD-028 | Pagination metadata is correct | Integration | US-PROD-005/AC-003 | — | ⏳ Pending |
| TC-PROD-029 | Page beyond total returns empty items | Integration | US-PROD-005/AC-005 | — | ⏳ Pending |
| TC-PROD-030 | PageSize clamped to 50 | Unit | US-PROD-005/AC-004 | — | ⏳ Pending |
| TC-PROD-031 | SQL injection attempt returns safely | Security | US-PROD-005/AC-006 | — | ⏳ Pending |
| TC-PROD-032 | Cached search served from output cache | Integration | US-PROD-005/AC-007 | — | ⏳ Pending |

### US-PROD-006: Filter by Category

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-PROD-033 | Valid categoryId returns filtered products | Integration | US-PROD-006/AC-001 | — | ⏳ Pending |
| TC-PROD-034 | Non-existent categoryId returns empty results | Integration | US-PROD-006/AC-002 | — | ⏳ Pending |
| TC-PROD-035 | Category filter combined with search (AND logic) | Integration | US-PROD-006/AC-003 | — | ⏳ Pending |
| TC-PROD-036 | Category filter combined with sort | Integration | US-PROD-006/AC-004 | — | ⏳ Pending |
| TC-PROD-037 | Malformed categoryId returns 400 | Unit | US-PROD-006/AC-005 | — | ⏳ Pending |

### US-PROD-007: Sort by Price

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-PROD-038 | Sort by price ascending returns ordered results | Integration | US-PROD-007/AC-001 | — | ⏳ Pending |
| TC-PROD-039 | Sort by price descending returns ordered results | Integration | US-PROD-007/AC-002 | — | ⏳ Pending |
| TC-PROD-040 | Default sort is name ascending | Integration | US-PROD-007/AC-003 | — | ⏳ Pending |
| TC-PROD-041 | Invalid sortBy returns 400 | Unit | US-PROD-007/AC-004 | — | ⏳ Pending |
| TC-PROD-042 | Sort combined with search and filter | Integration | US-PROD-007/AC-005 | — | ⏳ Pending |

### US-PROD-008: Image URL Validation

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-PROD-043 | Valid HTTPS URL accepted | Unit | US-PROD-008/AC-001 | — | ⏳ Pending |
| TC-PROD-044 | HTTP URL rejected with 400 | Unit | US-PROD-008/AC-002 | — | ⏳ Pending |
| TC-PROD-045 | Malformed URL rejected with 400 | Unit | US-PROD-008/AC-003 | — | ⏳ Pending |
| TC-PROD-046 | Null/omitted ImageUrl accepted | Unit | US-PROD-008/AC-004 | — | ⏳ Pending |
| TC-PROD-047 | URL over 2048 chars rejected | Unit | US-PROD-008/AC-005 | — | ⏳ Pending |
| TC-PROD-048 | javascript: URI rejected | Security | US-PROD-008/AC-006 | — | ⏳ Pending |

### US-PROD-009: Category Management

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-PROD-049 | Create category returns 201 | Integration | US-PROD-009/AC-001 | — | ⏳ Pending |
| TC-PROD-050 | Duplicate category name returns 409 | Integration | US-PROD-009/AC-002 | — | ⏳ Pending |
| TC-PROD-051 | Update category returns 200 | Integration | US-PROD-009/AC-003 | — | ⏳ Pending |
| TC-PROD-052 | Update non-existent category returns 404 | Integration | US-PROD-009/AC-004 | — | ⏳ Pending |
| TC-PROD-053 | Delete empty category returns 204 | Integration | US-PROD-009/AC-005 | — | ⏳ Pending |
| TC-PROD-054 | Delete category with products returns 409 | Integration | US-PROD-009/AC-006 | — | ⏳ Pending |
| TC-PROD-055 | List categories returns all, alphabetically | Integration | US-PROD-009/AC-007 | — | ⏳ Pending |
| TC-PROD-056 | Non-admin write returns 403; GET is public | Integration | US-PROD-009/AC-008 | — | ⏳ Pending |

### US-PROD-010: Product Audit Trail

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-PROD-057 | Product create writes audit entry with correct fields | Integration | US-PROD-010/AC-001 | — | ⏳ Pending |
| TC-PROD-058 | Product update writes audit with old and new values | Integration | US-PROD-010/AC-002 | — | ⏳ Pending |
| TC-PROD-059 | Product delete writes audit entry | Integration | US-PROD-010/AC-003 | — | ⏳ Pending |
| TC-PROD-060 | Category mutations write audit entries | Integration | US-PROD-010/AC-004 | — | ⏳ Pending |
| TC-PROD-061 | Audit failure does not block product write | Integration | US-PROD-010/AC-005 | — | ⏳ Pending |
| TC-PROD-062 | Audit query endpoint returns filtered, paginated results | Integration | US-PROD-010/AC-006 | — | ⏳ Pending |
| TC-PROD-063 | Audit entries always have non-null UserId | Unit | US-PROD-010/AC-007 | — | ⏳ Pending |

---

## Cross-Epic Dependencies (Login → Product Catalog)

| Product Story | Login Dependency | Relationship |
|--------------|-----------------|-------------|
| All US-PROD-* | US-LOGIN-001 (JWT Auth) | Auth middleware must be configured and operational |
| US-PROD-001, 002, 003, 009 | US-LOGIN-001 (Role claims) | Admin role claim present in JWT for authorization |
| US-PROD-010 | US-LOGIN-001 (sub claim) | User ID from JWT for audit trail |
| All US-PROD-* | US-LOGIN-002 (Validation patterns) | FluentValidation + ProblemDetails pattern established |

---

## Cross-Cutting Concern Traceability

| Concern | Stories | Implementation Notes |
|---------|---------|---------------------|
| Authentication (JWT) | All US-PROD-* | `AddAuthentication().AddJwtBearer()` from Login epic |
| Authorization (Role-based) | US-PROD-001, 002, 003, 009, 010 | `[Authorize(Policy = "AdminOnly")]` |
| Input Validation | US-PROD-001, 002, 005, 006, 007, 008, 009 | FluentValidation endpoint filters |
| Error Handling | All US-PROD-* | Result pattern → ProblemDetails (RFC 7807) |
| Caching | US-PROD-004, 005, 006, 007, 009 | Output cache middleware with tag-based invalidation |
| Audit Logging | US-PROD-001, 002, 003, 009, 010 | EF Core SaveChanges interceptor |
| Soft Delete | US-PROD-003, 004, 005 | Global query filter on `IsDeleted` |
| Concurrency | US-PROD-002 | EF Core `RowVersion` optimistic concurrency |

---

## OWASP Top 10 Coverage

| OWASP | Addressed By | Test Cases |
|-------|-------------|-----------|
| A01: Broken Access Control | US-PROD-001/AC-004,005; US-PROD-002/AC-005; US-PROD-003/AC-005; US-PROD-009/AC-008 | TC-PROD-004, 005, 011, 017, 056 |
| A02: Cryptographic Failures | US-PROD-008 (HTTPS-only image URLs); NFR-PROD-SEC | TC-PROD-044 |
| A03: Injection | US-PROD-005/AC-006 (SQL injection); US-PROD-008/AC-006 (XSS) | TC-PROD-031, 048 |
| A04: Insecure Design | NFR-PROD-SEC (rate limiting); US-PROD-005/AC-004 (page size limit) | TC-PROD-030 |
| A05: Security Misconfiguration | NFR-PROD-SEC (CORS, no stack traces) | — (infrastructure test) |
| A08: Software Integrity | US-PROD-002/AC-002 (concurrency token) | TC-PROD-008 |
| A09: Logging Failures | US-PROD-010 (full audit trail) | TC-PROD-057 – TC-PROD-063 |
| A10: SSRF | US-PROD-008 NFR (URL not fetched server-side) | TC-PROD-043 – 048 |

---

## Coverage Summary

| Category | Total | Designed | Implemented | Tested | Verified |
|----------|-------|----------|-------------|--------|----------|
| Critical (US-001, 002, 004, 005) | 4 | 3 (75%) | 0 (0%) | 0 (0%) | 0 (0%) |
| High (US-003, 006, 009, 010) | 4 | 3 (75%) | 0 (0%) | 0 (0%) | 0 (0%) |
| Medium (US-007, 008) | 2 | 0 (0%) | 0 (0%) | 0 (0%) | 0 (0%) |
| **Total** | **10** | **6 (60%)** | **0 (0%)** | **0 (0%)** | **0 (0%)** |

### Test Coverage by Type

| Test Type | Count | Stories Covered |
|-----------|-------|----------------|
| Unit | 12 | US-PROD-001, 002, 004, 005, 006, 007, 008, 010 |
| Integration | 46 | US-PROD-001 – US-PROD-010 |
| Security | 3 | US-PROD-005, US-PROD-008 |
| Performance | 0 (planned) | NFR-PROD-PERF |
| **Total** | **63** | **10 stories** |

---

## Gaps

| Gap Type | Count | Details |
|----------|-------|---------|
| Unimplemented stories | 10 | All stories pending implementation |
| Missing ADRs | 4 | US-PROD-004, 006, 007, 008 (low-complexity — no architectural decision needed) |
| Missing performance tests | 1 | NFR-PROD-PERF — benchmark tests to be created |
| Missing security scan | 1 | CodeQL scan to be configured in CI |
| Missing E2E tests | 0 | API-only project; integration tests cover E2E at API level |

---

## Dependency Graph

```
US-LOGIN-001 (JWT Auth) ─────────────────────────────────────────┐
                                                                  │
US-PROD-009 (Categories) ◀── foundation ──┐                      │
    │                                     │                      ▼
    ├── US-PROD-001 (Create Product) ─────┼── depends on ──▶ Auth + Categories
    │       │                             │
    │       ├── US-PROD-002 (Update) ─────┤
    │       ├── US-PROD-003 (Delete) ─────┤
    │       └── US-PROD-008 (Image URL) ──┘ (validation shared)
    │
    └── US-PROD-004 (Get Product) ────┐
        US-PROD-005 (List/Search) ────┤── public read endpoints
        US-PROD-006 (Filter) ─────────┤  (depend on products existing)
        US-PROD-007 (Sort) ───────────┘

US-PROD-010 (Audit Trail) ──── depends on ──▶ US-PROD-001, 002, 003, 009
```
