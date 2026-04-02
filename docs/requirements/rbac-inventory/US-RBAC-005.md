# US-RBAC-005: User Views Inventory Item Detail

**Type**: Functional
**Priority**: Critical
**Story Points**: 2
**Source**: EPIC-RBAC / PRD Section 6
**Status**: Ready

---

## User Story

**As a** User (with `User` role)
**I want to** view the full details of a specific inventory item by its ID
**So that** I can see complete stock information including quantity, price, and location

---

## Acceptance Criteria

- [ ] **AC-001**: Given an authenticated user with the `User` role and a valid inventory item ID, when they call `GET /api/v1/inventory/{id}`, then the system returns HTTP 200 with the full inventory item details (Id, SKU, Name, Description, Category, QuantityInStock, ReorderLevel, UnitPrice, Location, NeedsReorder, CreatedAt).
- [ ] **AC-002**: Given an authenticated user with the `Admin` role, when they call `GET /api/v1/inventory/{id}`, then the system also returns HTTP 200 (Admins retain read access).
- [ ] **AC-003**: Given an `id` that does not exist or refers to a soft-deleted item (`IsActive = false`), when any authenticated user calls GET, then the system returns HTTP 404 Not Found with a ProblemDetails response.
- [ ] **AC-004**: Given a malformed (non-GUID) `id` parameter, when any user calls `GET /api/v1/inventory/{id}`, then the system returns HTTP 400 Bad Request.
- [ ] **AC-005**: Given a request from an unauthenticated user, when they call `GET /api/v1/inventory/{id}`, then the system returns HTTP 401 Unauthorized.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | Detail endpoint responds < 200ms (p95) |
| Security | Read-only access; no data mutation possible via GET |
| Security | Soft-deleted items return 404 — no data leakage of deleted records |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-LOGIN-001 — JWT authentication middleware configured | Cross-Epic |
| DEP-002 | US-RBAC-001 — Inventory items exist in the database | Story |

---

## .NET Implementation Notes

### API Endpoint

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/api/v1/inventory/{id:guid}` | Any authenticated user | Get inventory item by ID |

### Authorization (No Change Required)

This GET endpoint already uses `.RequireAuthorization()`. Both Admin and User roles have read access.

### Linked Artifacts

- Design: ADR-RBAC-001
- Tests: TC-RBAC-022 – TC-RBAC-026
- Implementation: —
