# US-RBAC-003: Admin Deletes Inventory Item

**Type**: Functional
**Priority**: Critical
**Story Points**: 3
**Source**: EPIC-RBAC / PRD Section 6
**Status**: Ready

---

## User Story

**As an** Admin user
**I want to** soft-delete an inventory item
**So that** obsolete or discontinued items are hidden from users without losing historical data

---

## Acceptance Criteria

- [ ] **AC-001**: Given an authenticated user with the `Admin` role and a valid inventory item ID, when they call `DELETE /api/v1/inventory/{id}`, then the system sets `IsActive = false` and returns HTTP 204 No Content.
- [ ] **AC-002**: Given an `id` that does not exist or is already soft-deleted, when the Admin calls DELETE, then the system returns HTTP 404 Not Found with a ProblemDetails response.
- [ ] **AC-003**: Given a soft-deleted inventory item, when any user calls `GET /api/v1/inventory`, then the deleted item is excluded from the response.
- [ ] **AC-004**: Given a request from an unauthenticated user (no JWT or expired JWT), when they call `DELETE /api/v1/inventory/{id}`, then the system returns HTTP 401 Unauthorized.
- [ ] **AC-005**: Given a request from an authenticated user with the `User` role (not `Admin`), when they call `DELETE /api/v1/inventory/{id}`, then the system returns HTTP 403 Forbidden.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | Delete endpoint responds < 500ms (p95) |
| Security | Only `Admin` role can invoke; enforced via ASP.NET Core authorization policy |
| Security | Soft-delete preserves data for audit/recovery; no hard delete |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-LOGIN-001 — JWT authentication middleware configured | Cross-Epic |
| DEP-002 | US-RBAC-006 — Admin and User roles seeded in Identity | Story |
| DEP-003 | US-RBAC-001 — Inventory items must exist to be deleted | Story |

---

## .NET Implementation Notes

### API Endpoint

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| DELETE | `/api/v1/inventory/{id:guid}` | `Admin` role required | Soft-delete an inventory item |

### Authorization Change

```csharp
group.MapDelete("/{id:guid}", Delete)
    .RequireAuthorization(policy => policy.RequireRole("Admin"));
```

### Linked Artifacts

- Design: ADR-RBAC-001
- Tests: TC-RBAC-011 – TC-RBAC-015
- Implementation: —
