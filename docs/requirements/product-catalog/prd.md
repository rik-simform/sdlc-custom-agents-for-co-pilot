# Product Requirements Document: Product Catalog

**Epic**: EPIC-PROD — Product Catalog Management
**Version**: 1.0
**Date**: 2026-03-26
**Author**: SDLC Requirements Engineer (Agent)
**Status**: Draft — Pending Product Owner Review

---

## 1. Executive Summary

This epic delivers a full-featured Product Catalog system for an ASP.NET Core Web API using Vertical Slice Architecture and Minimal API patterns. The system enables customers to browse, search, filter, and sort products, while admin users manage the catalog with full CRUD capabilities, category management, and a complete audit trail.

## 2. Business Objectives

| Objective | Success Metric |
|-----------|---------------|
| Discoverable product catalog | Customers can find any product in < 3 interactions |
| Admin catalog management | Admin can create/update/delete products in < 30 seconds per operation |
| Search performance | Search results returned in < 500ms (p95) for catalogs up to 100k products |
| Data integrity | Zero orphaned or corrupted product records; all mutations audited |
| Access control | Zero unauthorized catalog mutations in production |
| Availability | 99.9% uptime for read endpoints; < 1 minute recovery for write endpoints |

## 3. Scope

### In Scope

| # | Capability | Priority |
|---|-----------|----------|
| 1 | Create product (Admin) — name, description, price, category, image URL | Critical |
| 2 | Update product (Admin) — with optimistic concurrency | Critical |
| 3 | Delete product (Admin) — soft delete with audit trail | High |
| 4 | Get product by ID — public read with response caching | Critical |
| 5 | List/search products — paginated full-text search | Critical |
| 6 | Filter products by category | High |
| 7 | Sort products by price (asc/desc) | Medium |
| 8 | Product image URL validation | Medium |
| 9 | Product category management (Admin CRUD) | High |
| 10 | Product audit trail — all mutations tracked | High |

### Out of Scope

- Product reviews and ratings (separate epic)
- Inventory / stock management (separate epic)
- Product variants (size, color) — future enhancement
- Product import/export (CSV, Excel) — future enhancement
- Product recommendations / ML-driven suggestions
- Shopping cart and checkout integration (separate epic)
- Product pricing tiers or discounts
- Image upload and storage (only URL reference in V1)

## 4. Target Users

| Role | Description |
|------|------------|
| Customer | Unauthenticated or authenticated user who browses and searches for products |
| Admin | Authenticated user with `Admin` role who manages the catalog |
| System Administrator | Ops user who monitors catalog health and audit trails |

## 5. Architecture Context

```
┌─────────────┐     ┌──────────────────┐     ┌─────────────────────────┐
│   Client     │────▶│  API Gateway /   │────▶│  Product Endpoints       │
│  (Browser /  │     │  Minimal API     │     │  (CRUD, Search, Filter)  │
│   Mobile)    │◀────│  Middleware       │◀────│                         │
└─────────────┘     └──────────────────┘     └────────┬────────────────┘
                                                       │
                              ┌─────────────────────────┤
                              │                         │
                    ┌─────────▼────────┐     ┌─────────▼─────────┐
                    │  EF Core         │     │  Audit Trail      │
                    │  DbContext        │     │  Service           │
                    │  (SQL Server)    │     │  (Structured Log)  │
                    └─────────┬────────┘     └───────────────────┘
                              │
                    ┌─────────▼────────┐
                    │  SQL Server      │
                    │  (Products,      │
                    │   Categories,    │
                    │   AuditEntries)  │
                    └──────────────────┘
```

### Vertical Slice Structure

```
Features/
├── Products/
│   ├── CreateProduct/
│   │   ├── CreateProductEndpoint.cs
│   │   ├── CreateProductRequest.cs
│   │   ├── CreateProductValidator.cs
│   │   └── CreateProductHandler.cs
│   ├── UpdateProduct/
│   ├── DeleteProduct/
│   ├── GetProduct/
│   ├── ListProducts/
│   └── Shared/
│       ├── Product.cs (Entity)
│       ├── ProductResponse.cs (DTO)
│       └── IProductRepository.cs
├── Categories/
│   ├── CreateCategory/
│   ├── UpdateCategory/
│   ├── DeleteCategory/
│   ├── ListCategories/
│   └── Shared/
│       ├── Category.cs (Entity)
│       └── CategoryResponse.cs (DTO)
└── Audit/
    └── Shared/
        ├── AuditEntry.cs (Entity)
        └── IAuditService.cs
```

### Technology Decisions

| Concern | Decision |
|---------|----------|
| Architecture | Vertical Slices — each feature self-contained |
| API Style | Minimal API with `MapGroup` endpoint grouping |
| ORM | Entity Framework Core 8 with SQL Server |
| Auth | Azure AD + JWT bearer tokens, role-based (Customer, Admin) |
| Validation | FluentValidation with endpoint filters |
| Concurrency | EF Core optimistic concurrency via `RowVersion` / `ConcurrencyToken` |
| Caching | Response caching middleware + output cache for GET endpoints |
| Search | EF Core LIKE / `CONTAINS` queries (SQL Server full-text index for scale) |
| Soft Delete | Global query filter with `IsDeleted` flag |
| Audit | Entity change interceptor writing to `AuditEntries` table |
| DTOs | C# `record` types for immutable request/response models |
| DI | Primary constructors for dependency injection |
| Error Handling | Result pattern via `Result<T>` with `ProblemDetails` (RFC 7807) |

## 6. User Stories Summary

| ID | Title | Priority | Story Points |
|----|-------|----------|-------------|
| US-PROD-001 | Create Product (Admin) | Critical | 5 |
| US-PROD-002 | Update Product (Admin) | Critical | 5 |
| US-PROD-003 | Delete Product (Admin) | High | 3 |
| US-PROD-004 | Get Product by ID | Critical | 3 |
| US-PROD-005 | List/Search Products | Critical | 8 |
| US-PROD-006 | Filter Products by Category | High | 3 |
| US-PROD-007 | Sort Products by Price | Medium | 2 |
| US-PROD-008 | Product Image URL Validation | Medium | 2 |
| US-PROD-009 | Product Category Management (Admin) | High | 5 |
| US-PROD-010 | Product Audit Trail | High | 5 |
| | **Total** | | **41** |

## 7. Non-Functional Requirements (Epic-Level)

### NFR-PROD-PERF: Performance

- GET single product: < 200ms (p95) with caching, < 500ms without
- Search/list/filter: < 500ms (p95) for catalogs up to 100k products, page size ≤ 50
- Create/update/delete: < 1s (p95) including audit write
- Category list: < 100ms (p95) — cacheable and small dataset

### NFR-PROD-SEC: Security

- OWASP A01 (Broken Access Control): Admin-only write endpoints enforced via `[Authorize(Policy = "AdminOnly")]`
- OWASP A02 (Crypto Failures): HTTPS-only, HSTS header, no sensitive data in URLs
- OWASP A03 (Injection): Parameterized EF Core queries only; FluentValidation on all input
- OWASP A04 (Insecure Design): Rate limiting on write endpoints; input size limits
- OWASP A05 (Security Misconfiguration): CORS restricted; no stack traces in production errors
- OWASP A08 (Software Integrity): `RowVersion` concurrency token prevents lost updates
- OWASP A09 (Logging Failures): All write operations produce structured audit log entries

### NFR-PROD-SCALE: Scalability

- Stateless API — horizontally scalable behind load balancer
- Pagination mandatory — no unbounded result sets
- Database indexes on: `CategoryId`, `Name` (full-text), `Price`, `IsDeleted`

### NFR-PROD-MAINTAIN: Maintainability

- Vertical slice isolation — changes to one feature do not affect others
- ≥ 80% code coverage on product domain
- All public endpoints documented in OpenAPI / Swagger

## 8. Constraints

| Constraint | Rationale |
|-----------|-----------|
| .NET 8 + Minimal API | Project standard |
| SQL Server | Existing infrastructure; EF Core provider |
| Azure AD JWT | Auth already implemented per Login epic |
| No image upload | V1 stores URL only; image upload is a separate epic |
| Max page size 50 | Prevent excessive memory/bandwidth consumption |
| Product name max 200 chars | Database column constraint, UX readability |
| Description max 4000 chars | SQL Server `NVARCHAR(4000)` — avoids MAX column |
| Price > 0 and ≤ 999999.99 | Business rule; decimal(18,2) precision |

## 9. Risks

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|-----------|
| Full-text search performance degrades at scale | Medium | High | SQL Server full-text index; consider Elasticsearch for future |
| Concurrent edits cause lost updates | Medium | High | Optimistic concurrency via `RowVersion` (US-PROD-002) |
| Soft-deleted products leak into search results | Medium | Medium | Global query filter `IsDeleted == false`; integration tests |
| Image URL points to unavailable resource | High | Low | Validate URL format only; health check for critical images |
| Admin token compromise leads to catalog tampering | Low | Critical | Audit trail (US-PROD-010), Azure AD conditional access |
| Category deletion orphans products | Medium | High | Prevent delete if products reference category (US-PROD-009) |

## 10. Dependencies on Login Epic

| Dependency | Login Story | Product Story | Relationship |
|-----------|------------|---------------|-------------|
| JWT authentication middleware | US-LOGIN-001 | All US-PROD-* | Auth middleware must be configured |
| Role-based authorization | US-LOGIN-001 | US-PROD-001, 002, 003, 009 | Admin role claim in JWT |
| Audit user identification | US-LOGIN-001 | US-PROD-010 | `sub` claim used for audit `ModifiedBy` |
| Input validation patterns | US-LOGIN-002 | All US-PROD-* | FluentValidation + ProblemDetails pattern |

## 11. Success Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| API response time (reads) | < 500ms p95 | Application Insights |
| API response time (writes) | < 1s p95 | Application Insights |
| Search relevance | Top result matches intent 90% of cases | Manual QA sample |
| Code coverage | ≥ 80% on product domain | CI pipeline report |
| Security findings | Zero critical/high at release | CodeQL + Dependabot |
| Audit completeness | 100% of write ops have audit entries | Integration test suite |
