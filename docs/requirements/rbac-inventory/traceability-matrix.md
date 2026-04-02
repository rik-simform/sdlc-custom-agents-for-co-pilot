# Requirements Traceability Matrix — EPIC-RBAC

Last Updated: 2026-04-02

---

## Forward Traceability (Requirement → Verification)

| Req ID | Title | Priority | Design | Implementation | Test Cases | Status |
|--------|-------|----------|--------|----------------|------------|--------|
| US-RBAC-001 | Admin Creates Inventory Item | Critical | ADR-RBAC-001 | — | TC-RBAC-001 – TC-RBAC-005 | ⏳ Pending |
| US-RBAC-002 | Admin Updates Inventory Item | Critical | ADR-RBAC-001 | — | TC-RBAC-006 – TC-RBAC-010 | ⏳ Pending |
| US-RBAC-003 | Admin Deletes Inventory Item | Critical | ADR-RBAC-001 | — | TC-RBAC-011 – TC-RBAC-015 | ⏳ Pending |
| US-RBAC-004 | User Views Inventory List | Critical | ADR-RBAC-001 | — | TC-RBAC-016 – TC-RBAC-021 | ⏳ Pending |
| US-RBAC-005 | User Views Inventory Item Detail | Critical | ADR-RBAC-001 | — | TC-RBAC-022 – TC-RBAC-026 | ⏳ Pending |
| US-RBAC-006 | Role Seeding and Assignment | High | ADR-RBAC-001 | — | TC-RBAC-027 – TC-RBAC-034 | ⏳ Pending |
| US-RBAC-007 | Unauthorized & Forbidden Responses | High | ADR-RBAC-001 | — | TC-RBAC-035 – TC-RBAC-039 | ⏳ Pending |

---

## Backward Traceability (Test → Requirement)

### US-RBAC-001: Admin Creates Inventory Item

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-RBAC-001 | Admin with valid body creates item and gets 201 | Integration | US-RBAC-001/AC-001 | — | ⏳ Pending |
| TC-RBAC-002 | Invalid fields return 400 with ValidationProblemDetails | Unit | US-RBAC-001/AC-002 | — | ⏳ Pending |
| TC-RBAC-003 | Unauthenticated request returns 401 | Integration | US-RBAC-001/AC-003 | — | ⏳ Pending |
| TC-RBAC-004 | User role returns 403 on POST | Integration | US-RBAC-001/AC-004 | — | ⏳ Pending |
| TC-RBAC-005 | CreatedBy set to Admin user ID | Integration | US-RBAC-001/AC-005 | — | ⏳ Pending |

### US-RBAC-002: Admin Updates Inventory Item

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-RBAC-006 | Admin with valid body updates item and gets 200 | Integration | US-RBAC-002/AC-001 | — | ⏳ Pending |
| TC-RBAC-007 | Non-existent ID returns 404 | Integration | US-RBAC-002/AC-002 | — | ⏳ Pending |
| TC-RBAC-008 | Invalid fields return 400 | Unit | US-RBAC-002/AC-003 | — | ⏳ Pending |
| TC-RBAC-009 | Unauthenticated request returns 401 | Integration | US-RBAC-002/AC-004 | — | ⏳ Pending |
| TC-RBAC-010 | User role returns 403 on PUT | Integration | US-RBAC-002/AC-005 | — | ⏳ Pending |

### US-RBAC-003: Admin Deletes Inventory Item

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-RBAC-011 | Admin soft-deletes item and gets 204 | Integration | US-RBAC-003/AC-001 | — | ⏳ Pending |
| TC-RBAC-012 | Non-existent or already-deleted ID returns 404 | Integration | US-RBAC-003/AC-002 | — | ⏳ Pending |
| TC-RBAC-013 | Soft-deleted item excluded from GET list | Integration | US-RBAC-003/AC-003 | — | ⏳ Pending |
| TC-RBAC-014 | Unauthenticated request returns 401 | Integration | US-RBAC-003/AC-004 | — | ⏳ Pending |
| TC-RBAC-015 | User role returns 403 on DELETE | Integration | US-RBAC-003/AC-005 | — | ⏳ Pending |

### US-RBAC-004: User Views Inventory List

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-RBAC-016 | User role can list all active items | Integration | US-RBAC-004/AC-001 | — | ⏳ Pending |
| TC-RBAC-017 | Admin role can also list items | Integration | US-RBAC-004/AC-002 | — | ⏳ Pending |
| TC-RBAC-018 | Category filter returns matching items | Integration | US-RBAC-004/AC-003 | — | ⏳ Pending |
| TC-RBAC-019 | Search filter returns matching items | Integration | US-RBAC-004/AC-004 | — | ⏳ Pending |
| TC-RBAC-020 | Unauthenticated request returns 401 | Integration | US-RBAC-004/AC-005 | — | ⏳ Pending |
| TC-RBAC-021 | User role can access reorder endpoint | Integration | US-RBAC-004/AC-006 | — | ⏳ Pending |

### US-RBAC-005: User Views Inventory Item Detail

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-RBAC-022 | User role gets full item detail by valid ID | Integration | US-RBAC-005/AC-001 | — | ⏳ Pending |
| TC-RBAC-023 | Admin role also gets item detail | Integration | US-RBAC-005/AC-002 | — | ⏳ Pending |
| TC-RBAC-024 | Non-existent or soft-deleted ID returns 404 | Integration | US-RBAC-005/AC-003 | — | ⏳ Pending |
| TC-RBAC-025 | Malformed GUID returns 400 | Unit | US-RBAC-005/AC-004 | — | ⏳ Pending |
| TC-RBAC-026 | Unauthenticated request returns 401 | Integration | US-RBAC-005/AC-005 | — | ⏳ Pending |

### US-RBAC-006: Role Seeding and Assignment

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-RBAC-027 | Admin and User roles seeded on fresh DB | Integration | US-RBAC-006/AC-001 | — | ⏳ Pending |
| TC-RBAC-028 | Idempotent seeding — no duplicates on restart | Integration | US-RBAC-006/AC-002 | — | ⏳ Pending |
| TC-RBAC-029 | Admin assigns role to user via POST | Integration | US-RBAC-006/AC-003 | — | ⏳ Pending |
| TC-RBAC-030 | Invalid role name returns 400 | Unit | US-RBAC-006/AC-004 | — | ⏳ Pending |
| TC-RBAC-031 | Duplicate role assignment is idempotent (200) | Integration | US-RBAC-006/AC-005 | — | ⏳ Pending |
| TC-RBAC-032 | Non-Admin cannot assign roles (403) | Integration | US-RBAC-006/AC-006 | — | ⏳ Pending |
| TC-RBAC-033 | Role assignment audit entry recorded | Integration | US-RBAC-006/AC-007 | — | ⏳ Pending |
| TC-RBAC-034 | New user registration gets User role by default | Integration | US-RBAC-006/AC-008 | — | ⏳ Pending |

### US-RBAC-007: Unauthorized and Forbidden Responses

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-RBAC-035 | No JWT returns 401 with ProblemDetails | Integration | US-RBAC-007/AC-001 | — | ⏳ Pending |
| TC-RBAC-036 | User role on write endpoint returns 403 ProblemDetails | Integration | US-RBAC-007/AC-002 | — | ⏳ Pending |
| TC-RBAC-037 | 401/403 responses have application/problem+json content type | Integration | US-RBAC-007/AC-003 | — | ⏳ Pending |
| TC-RBAC-038 | Error responses contain no stack traces or internal details | Security | US-RBAC-007/AC-004 | — | ⏳ Pending |
| TC-RBAC-039 | Non-admin on role assign returns 403 ProblemDetails | Integration | US-RBAC-007/AC-005 | — | ⏳ Pending |

---

## Coverage Summary

| Metric | Value |
|--------|-------|
| **Total Requirements** | 7 |
| **Total Acceptance Criteria** | 39 |
| **Total Test Cases** | 39 |
| **Coverage** | 100% (every AC mapped to at least one test) |
| **Untested ACs** | 0 |

---

## Cross-Epic Dependencies

| This Epic | Depends On | Type | Status |
|-----------|-----------|------|--------|
| US-RBAC-001, 002, 003, 004, 005, 006, 007 | US-LOGIN-001 — JWT Authentication | Cross-Epic | ⏳ Pending |
| US-RBAC-006 | ASP.NET Identity Schema (AspNetRoles, AspNetUserRoles) | Infrastructure | ⏳ Pending |
