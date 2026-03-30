# EPIC-PROD: Product Catalog Management — Epic Summary

**Date**: 2026-03-26
**Status**: Ready for Sprint Planning
**Epic Owner**: Product Owner (pending assignment)

---

## Story Inventory

| ID | Title | Priority | SP | Sprint | Dependencies |
|----|-------|----------|----|--------|-------------|
| US-PROD-009 | Category Management (Admin) | High | 5 | Sprint 1 | US-LOGIN-001 |
| US-PROD-001 | Create Product (Admin) | Critical | 5 | Sprint 1 | US-LOGIN-001, US-PROD-009 |
| US-PROD-008 | Image URL Validation | Medium | 2 | Sprint 1 | US-PROD-001 |
| US-PROD-004 | Get Product by ID | Critical | 3 | Sprint 1 | US-PROD-001 |
| US-PROD-002 | Update Product (Admin) | Critical | 5 | Sprint 2 | US-PROD-001, US-PROD-009 |
| US-PROD-003 | Delete Product (Admin) | High | 3 | Sprint 2 | US-PROD-001, US-PROD-010 |
| US-PROD-010 | Product Audit Trail | High | 5 | Sprint 2 | US-PROD-001, 002, 003, 009 |
| US-PROD-005 | List/Search Products | Critical | 8 | Sprint 2 | US-PROD-001 |
| US-PROD-006 | Filter by Category | High | 3 | Sprint 3 | US-PROD-005, US-PROD-009 |
| US-PROD-007 | Sort by Price | Medium | 2 | Sprint 3 | US-PROD-005 |

---

## Totals

| Metric | Value |
|--------|-------|
| **Total Stories** | 10 |
| **Total Story Points** | 41 |
| **Total Acceptance Criteria** | 58 |
| **Total Test Cases** | 63 |
| **Critical Stories** | 4 (21 SP) |
| **High Stories** | 4 (16 SP) |
| **Medium Stories** | 2 (4 SP) |

---

## Recommended Sprint Allocation

### Sprint 1 — Foundation: Categories, Create Product, Get Product (15 SP)

| Story | SP | Rationale |
|-------|----|-----------|
| US-PROD-009 | 5 | Foundation — categories must exist before products can reference them |
| US-PROD-001 | 5 | Core write operation — all other product stories depend on products existing |
| US-PROD-008 | 2 | Ships with create — image URL validation is part of create/update validators |
| US-PROD-004 | 3 | Core read — enables immediate product visibility after creation |

**Sprint Goal**: Admins can create categories and products. Customers can view individual product details. Image URL validation enforced.

**Prerequisite**: Login epic Sprint 1 (US-LOGIN-001, 002, 003) must be complete — JWT auth and role-based authorization required.

### Sprint 2 — CRUD Completion: Update, Delete, Search, Audit (21 SP)

| Story | SP | Rationale |
|-------|----|-----------|
| US-PROD-002 | 5 | Complete write operations with concurrency control |
| US-PROD-003 | 3 | Soft delete with audit requirement |
| US-PROD-010 | 5 | Audit trail — cross-cutting, needed before production |
| US-PROD-005 | 8 | Search/list — largest story, core discovery experience |

**Sprint Goal**: Full product CRUD with concurrency control and audit trail. Customers can search the catalog with pagination.

### Sprint 3 — Discovery Polish: Filter & Sort (5 SP)

| Story | SP | Rationale |
|-------|----|-----------|
| US-PROD-006 | 3 | Extends search endpoint with category filter |
| US-PROD-007 | 2 | Extends search endpoint with price sort |

**Sprint Goal**: Customers can filter by category and sort by price. Product catalog is production-ready.

---

## Velocity & Capacity

| Sprint | Story Points | Team Capacity Needed |
|--------|-------------|---------------------|
| Sprint 1 | 15 | Standard (1 senior + 1 mid-level dev) |
| Sprint 2 | 21 | High (requires search + audit parallel tracks) |
| Sprint 3 | 5 | Light (polish sprint — good for tech debt) |
| **Total** | **41** | **3 sprints** |

---

## Critical Path

```
Login EPIC Sprint 1
    └── US-LOGIN-001 (JWT Auth) ────────────────────────────────┐
                                                                 │
Product EPIC Sprint 1                                            ▼
    US-PROD-009 (Categories) ──▶ US-PROD-001 (Create) ──▶ US-PROD-004 (Get)
                                      │                    US-PROD-008 (Image)
                                      │
Product EPIC Sprint 2                 ▼
    US-PROD-002 (Update) ──┐
    US-PROD-003 (Delete) ──┼──▶ US-PROD-010 (Audit)
    US-PROD-005 (Search) ──┘
                             │
Product EPIC Sprint 3        ▼
    US-PROD-006 (Filter) + US-PROD-007 (Sort)
```

**Minimum viable product catalog** requires Sprint 1 + Sprint 2 = **36 SP** (categories, full CRUD, search, audit).

Sprint 3 (filter + sort) is **enhancement** — valuable but not blocking production release.

---

## Cross-Epic Dependency Graph

```
┌─────────────────────────────────────────────────────────┐
│                    LOGIN EPIC                            │
│  US-LOGIN-001 ──▶ JWT Auth + Role Claims                │
│  US-LOGIN-002 ──▶ FluentValidation + ProblemDetails     │
└────────────┬────────────────────────────────────────────┘
             │
             │ Depends on (blocking)
             ▼
┌─────────────────────────────────────────────────────────┐
│               PRODUCT CATALOG EPIC                       │
│                                                          │
│  Sprint 1: US-PROD-009 → US-PROD-001 → US-PROD-004     │
│            US-PROD-008                                   │
│                                                          │
│  Sprint 2: US-PROD-002, 003, 005, 010                   │
│                                                          │
│  Sprint 3: US-PROD-006, 007                              │
└─────────────────────────────────────────────────────────┘
```

**Scheduling constraint**: Product Catalog Sprint 1 cannot begin until Login Sprint 1 stories (US-LOGIN-001, US-LOGIN-002) are accepted and deployed to the development environment.

---

## OWASP Coverage Matrix

| OWASP Top 10 | Addressed By | Status |
|-------------|-------------|--------|
| A01: Broken Access Control | US-PROD-001 (admin auth), 002, 003, 009 | ✅ Covered |
| A02: Cryptographic Failures | US-PROD-008 (HTTPS-only URLs), NFR-PROD-SEC | ✅ Covered |
| A03: Injection | US-PROD-005 (parameterized queries), US-PROD-008 (XSS) | ✅ Covered |
| A04: Insecure Design | NFR-PROD-SEC (rate limiting, page size limits) | ✅ Covered |
| A05: Security Misconfiguration | NFR-PROD-SEC (CORS, error handling) | ✅ Covered |
| A08: Software Integrity | US-PROD-002 (concurrency token) | ✅ Covered |
| A09: Logging Failures | US-PROD-010 (audit trail) | ✅ Covered |
| A10: SSRF | US-PROD-008 NFR (no server-side URL fetch) | ✅ Covered |

---

## Definition of Done (Epic-Level)

- [ ] All 10 stories (US-PROD-001 through 010) accepted by PO
- [ ] 63 test cases implemented and passing
- [ ] ≥ 80% code coverage on Product and Category domains
- [ ] CodeQL scan: zero critical/high findings
- [ ] Performance: reads < 500ms p95, writes < 1s p95
- [ ] OpenAPI documentation published for all endpoints
- [ ] Runbook: product catalog operations documented
- [ ] Deployed to staging and smoke-tested
- [ ] Audit trail verified end-to-end

---

## API Surface Summary

| Method | Route | Auth | Story |
|--------|-------|------|-------|
| POST | `/api/v1/products` | Admin | US-PROD-001 |
| PUT | `/api/v1/products/{id}` | Admin | US-PROD-002 |
| DELETE | `/api/v1/products/{id}` | Admin | US-PROD-003 |
| GET | `/api/v1/products/{id}` | Anonymous | US-PROD-004 |
| GET | `/api/v1/products` | Anonymous | US-PROD-005, 006, 007 |
| POST | `/api/v1/categories` | Admin | US-PROD-009 |
| PUT | `/api/v1/categories/{id}` | Admin | US-PROD-009 |
| DELETE | `/api/v1/categories/{id}` | Admin | US-PROD-009 |
| GET | `/api/v1/categories` | Anonymous | US-PROD-009 |
| GET | `/api/v1/admin/audit` | Admin | US-PROD-010 |

---

## Files in This Epic

| File | Description |
|------|------------|
| [prd.md](prd.md) | Product Requirements Document |
| [US-PROD-001.md](US-PROD-001.md) | Create Product (Admin) |
| [US-PROD-002.md](US-PROD-002.md) | Update Product (Admin) |
| [US-PROD-003.md](US-PROD-003.md) | Delete Product (Admin) |
| [US-PROD-004.md](US-PROD-004.md) | Get Product by ID |
| [US-PROD-005.md](US-PROD-005.md) | List/Search Products |
| [US-PROD-006.md](US-PROD-006.md) | Filter Products by Category |
| [US-PROD-007.md](US-PROD-007.md) | Sort Products by Price |
| [US-PROD-008.md](US-PROD-008.md) | Product Image URL Validation |
| [US-PROD-009.md](US-PROD-009.md) | Product Category Management (Admin) |
| [US-PROD-010.md](US-PROD-010.md) | Product Audit Trail |
| [traceability-matrix.md](traceability-matrix.md) | Requirements Traceability Matrix |
| [epic-summary.md](epic-summary.md) | This file |
