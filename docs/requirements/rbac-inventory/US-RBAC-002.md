# US-RBAC-002: Admin Updates Inventory Item

**Type**: Functional
**Priority**: Critical
**Story Points**: 3
**Source**: EPIC-RBAC / PRD Section 6
**Status**: Ready

---

## User Story

**As an** Admin user
**I want to** update existing inventory items (name, description, category, quantity, reorder level, price, location)
**So that** the inventory catalog stays accurate and up-to-date for all users

---

## Acceptance Criteria

- [ ] **AC-001**: Given an authenticated user with the `Admin` role and a valid request body, when they call `PUT /api/v1/inventory/{id}`, then the system returns HTTP 200 with the updated inventory item including the `UpdatedAt` timestamp and `UpdatedBy` set to the Admin's user ID.
- [ ] **AC-002**: Given an `id` that does not exist or refers to a soft-deleted item, when the Admin submits an update, then the system returns HTTP 404 Not Found with a ProblemDetails response.
- [ ] **AC-003**: Given a request with any invalid field, when the Admin submits, then the system returns HTTP 400 with a `ValidationProblemDetails` response listing all validation errors.
- [ ] **AC-004**: Given a request from an unauthenticated user (no JWT or expired JWT), when they call `PUT /api/v1/inventory/{id}`, then the system returns HTTP 401 Unauthorized.
- [ ] **AC-005**: Given a request from an authenticated user with the `User` role (not `Admin`), when they call `PUT /api/v1/inventory/{id}`, then the system returns HTTP 403 Forbidden.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | Update endpoint responds < 1s (p95) |
| Security | Only `Admin` role can invoke; enforced via ASP.NET Core authorization policy |
| Security | All input validated server-side via FluentValidation |
| Security | Malformed GUID route parameter returns 400 (not 500) |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-LOGIN-001 — JWT authentication middleware configured | Cross-Epic |
| DEP-002 | US-RBAC-006 — Admin and User roles seeded in Identity | Story |
| DEP-003 | US-RBAC-001 — Inventory items must exist to be updated | Story |

---

## .NET Implementation Notes

### API Endpoint

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| PUT | `/api/v1/inventory/{id:guid}` | `Admin` role required | Update an inventory item |

### Authorization Change

```csharp
group.MapPut("/{id:guid}", Update)
    .RequireAuthorization(policy => policy.RequireRole("Admin"));
```

### Linked Artifacts

- Design: ADR-RBAC-001
- Tests: TC-RBAC-006 – TC-RBAC-010
- Implementation: —
