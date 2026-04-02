# US-RBAC-004: User Views Inventory List

**Type**: Functional
**Priority**: Critical
**Story Points**: 3
**Source**: EPIC-RBAC / PRD Section 6
**Status**: Ready

---

## User Story

**As a** User (with `User` role)
**I want to** view the list of all active inventory items added by Admin
**So that** I can browse available products and check stock information

---

## Acceptance Criteria

- [ ] **AC-001**: Given an authenticated user with the `User` role, when they call `GET /api/v1/inventory`, then the system returns HTTP 200 with a list of all active (`IsActive = true`) inventory items.
- [ ] **AC-002**: Given an authenticated user with the `Admin` role, when they call `GET /api/v1/inventory`, then the system also returns HTTP 200 with the full list (Admins retain read access).
- [ ] **AC-003**: Given an inventory list with items, when the User provides a `?category=X` query parameter, then the system returns only items matching that category (case-insensitive).
- [ ] **AC-004**: Given an inventory list with items, when the User provides a `?search=Y` query parameter, then the system returns items where Name or Description contains the search term (case-insensitive).
- [ ] **AC-005**: Given a request from an unauthenticated user, when they call `GET /api/v1/inventory`, then the system returns HTTP 401 Unauthorized.
- [ ] **AC-006**: Given an authenticated user with the `User` role, when they call `GET /api/v1/inventory/reorder`, then the system returns HTTP 200 with items where `QuantityInStock ≤ ReorderLevel`.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | List endpoint responds < 500ms (p95) for up to 10,000 items |
| Security | Read-only access for `User` role; no mutation endpoints accessible |
| Security | Soft-deleted items (IsActive = false) never returned in list |
| Security | Search parameters sanitized; SQL injection prevented by EF Core |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-LOGIN-001 — JWT authentication middleware configured | Cross-Epic |
| DEP-002 | US-RBAC-001 — Inventory items exist in the database (created by Admin) | Story |

---

## .NET Implementation Notes

### API Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/api/v1/inventory` | Any authenticated user | List all active inventory items |
| GET | `/api/v1/inventory/reorder` | Any authenticated user | List items needing reorder |

### Authorization (No Change Required)

These GET endpoints already use `.RequireAuthorization()` which permits any authenticated user regardless of role. This remains correct — both Admin and User roles should have read access.

```csharp
group.MapGet("/", GetAll)
    .RequireAuthorization();  // Any authenticated user — Admin or User
```

### Response DTO

```csharp
public record InventoryItemResponse(
    Guid Id,
    string Sku,
    string Name,
    string? Description,
    string Category,
    int QuantityInStock,
    int ReorderLevel,
    decimal UnitPrice,
    string? Location,
    bool NeedsReorder,
    DateTimeOffset CreatedAt);
```

### Linked Artifacts

- Design: ADR-RBAC-001
- Tests: TC-RBAC-016 – TC-RBAC-021
- Implementation: —
