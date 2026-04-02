# US-RBAC-001: Admin Creates Inventory Item

**Type**: Functional
**Priority**: Critical
**Story Points**: 3
**Source**: EPIC-RBAC / PRD Section 6
**Status**: Ready

---

## User Story

**As an** Admin user
**I want to** create new inventory items with SKU, name, description, category, quantity, reorder level, price, and location
**So that** the inventory catalog is populated and available for all users to browse

---

## Acceptance Criteria

- [ ] **AC-001**: Given an authenticated user with the `Admin` role and a valid request body, when they call `POST /api/v1/inventory`, then the system returns HTTP 201 with the created inventory item including a server-generated `Id` (GUID) and `CreatedAt` timestamp, and a `Location` header pointing to the new resource.
- [ ] **AC-002**: Given a request with any invalid field (missing required fields, negative price, negative quantity), when the Admin submits, then the system returns HTTP 400 with a `ValidationProblemDetails` response listing all validation errors.
- [ ] **AC-003**: Given a request from an unauthenticated user (no JWT or expired JWT), when they call `POST /api/v1/inventory`, then the system returns HTTP 401 Unauthorized.
- [ ] **AC-004**: Given a request from an authenticated user with the `User` role (not `Admin`), when they call `POST /api/v1/inventory`, then the system returns HTTP 403 Forbidden.
- [ ] **AC-005**: Given a successful inventory item creation, then the `CreatedBy` field is set to the Admin's user ID extracted from the JWT `sub` claim.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | Create endpoint responds < 1s (p95) |
| Security | Only `Admin` role can invoke; enforced via ASP.NET Core authorization policy |
| Security | All input validated server-side via FluentValidation before DB write |
| Security | SQL injection prevented by EF Core parameterized queries |
| Security | Mass assignment prevention: only declared DTO properties mapped to entity |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-LOGIN-001 — JWT authentication middleware configured | Cross-Epic |
| DEP-002 | US-RBAC-006 — Admin and User roles seeded in Identity | Story |
| DEP-003 | SQL Server database with InventoryItems table | Infrastructure |

---

## .NET Implementation Notes

### API Endpoint

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/v1/inventory` | `Admin` role required | Create a new inventory item |

### Authorization Change

Current endpoint uses `.RequireAuthorization()` (any authenticated user). Must change to:

```csharp
group.MapPost("/", Create)
    .RequireAuthorization(policy => policy.RequireRole("Admin"));
```

### Linked Artifacts

- Design: ADR-RBAC-001
- Tests: TC-RBAC-001 – TC-RBAC-005
- Implementation: —
