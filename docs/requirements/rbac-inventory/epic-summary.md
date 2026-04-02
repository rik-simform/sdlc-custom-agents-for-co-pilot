# EPIC-RBAC: Role-Based Access Control for Inventory — Epic Summary

**Date**: 2026-04-02
**Status**: Ready for Sprint Planning
**Epic Owner**: Product Owner (pending assignment)

---

## Story Inventory

| ID | Title | Priority | SP | Sprint | Dependencies |
|----|-------|----------|----|--------|-------------|
| US-RBAC-006 | Role Seeding and Assignment | High | 5 | Sprint 1 | US-LOGIN-001 |
| US-RBAC-007 | Unauthorized & Forbidden Responses | High | 3 | Sprint 1 | US-LOGIN-001 |
| US-RBAC-001 | Admin Creates Inventory Item | Critical | 3 | Sprint 1 | US-LOGIN-001, US-RBAC-006 |
| US-RBAC-002 | Admin Updates Inventory Item | Critical | 3 | Sprint 1 | US-LOGIN-001, US-RBAC-006 |
| US-RBAC-003 | Admin Deletes Inventory Item | Critical | 3 | Sprint 1 | US-LOGIN-001, US-RBAC-006 |
| US-RBAC-004 | User Views Inventory List | Critical | 3 | Sprint 1 | US-LOGIN-001 |
| US-RBAC-005 | User Views Inventory Item Detail | Critical | 2 | Sprint 1 | US-LOGIN-001 |

---

## Totals

| Metric | Value |
|--------|-------|
| **Total Stories** | 7 |
| **Total Story Points** | 22 |
| **Total Acceptance Criteria** | 39 |
| **Total Test Cases** | 39 |
| **Critical Stories** | 5 (14 SP) |
| **High Stories** | 2 (8 SP) |

---

## Recommended Sprint Allocation

### Sprint 1 — Full RBAC Implementation (22 SP)

| Story | SP | Rationale |
|-------|----|-----------|
| US-RBAC-006 | 5 | Foundation — roles must exist before any RBAC enforcement |
| US-RBAC-007 | 3 | Cross-cutting — proper 401/403 responses for all protected endpoints |
| US-RBAC-001 | 3 | Restrict POST /inventory to Admin |
| US-RBAC-002 | 3 | Restrict PUT /inventory/{id} to Admin |
| US-RBAC-003 | 3 | Restrict DELETE /inventory/{id} to Admin |
| US-RBAC-004 | 3 | Validate User role can list inventory (read-only) |
| US-RBAC-005 | 2 | Validate User role can get item by ID (read-only) |

**Sprint Goal**: Complete RBAC enforcement — Admins have full CRUD, Users have read-only access, proper 401/403 responses for all violations.

**Prerequisite**: Login epic (US-LOGIN-001, 002, 003) must be complete — JWT authentication and ASP.NET Identity required.

---

## Implementation Impact Analysis

### Files to Modify

| File | Change |
|------|--------|
| `src/MyProject.Api/Endpoints/InventoryEndpoints.cs` | Add `.RequireAuthorization(p => p.RequireRole("Admin"))` to POST, PUT, DELETE; keep `.RequireAuthorization()` on GET |
| `src/MyProject.Api/DatabaseSeeder.cs` | Add role seeding for `Admin` and `User` roles |
| `src/MyProject.Api/Endpoints/AuthEndpoints.cs` | Add `POST /api/v1/auth/roles/assign` endpoint |
| `src/MyProject.Api/Program.cs` | Register `IAuthorizationMiddlewareResultHandler` for ProblemDetails responses |
| Registration handler | Add `userManager.AddToRoleAsync(user, "User")` after user creation |

### No Breaking Changes to Read Endpoints

The existing `GET /api/v1/inventory` and `GET /api/v1/inventory/{id}` endpoints keep `.RequireAuthorization()` (any authenticated user), so both Admin and User roles retain read access.

### Migration Plan

1. Deploy role seeding first (US-RBAC-006)
2. Assign existing privileged users to `Admin` role
3. Deploy endpoint authorization changes (US-RBAC-001, 002, 003)
4. Verify User role is read-only via integration tests

---

## Velocity & Capacity

| Sprint | Story Points | Team Capacity Needed |
|--------|-------------|---------------------|
| Sprint 1 | 22 | 1 backend developer, ~1 week |

---

## Definition of Done

- [ ] All 7 stories implemented and code-reviewed
- [ ] All 39 acceptance criteria verified with automated tests
- [ ] Authorization policies enforced on all inventory write endpoints
- [ ] Role seeding runs idempotently on startup
- [ ] Default User role assigned on registration
- [ ] 401/403 responses follow ProblemDetails (RFC 9457) format
- [ ] Zero security findings from CodeQL scan
- [ ] Product Owner review completed
