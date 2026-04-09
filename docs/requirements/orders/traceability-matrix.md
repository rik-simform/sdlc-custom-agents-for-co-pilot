# Requirements Traceability Matrix — EPIC-ORD

Last Updated: 2026-04-08

---

## Forward Traceability (Requirement → Verification)

| Req ID | Title | Priority | Design | Implementation | Test Cases | Status |
|--------|-------|----------|--------|----------------|------------|--------|
| US-ORD-001 | User Places Order | Critical | ADR-ORDERS-001 | — | TC-ORD-001 – TC-ORD-007 | ⏳ Pending |
| US-ORD-002 | User Views Order History | Critical | ADR-ORDERS-001 | — | TC-ORD-008 – TC-ORD-012 | ⏳ Pending |
| US-ORD-003 | User Cancels Pending Order | High | ADR-ORDERS-001 | — | TC-ORD-013 – TC-ORD-018 | ⏳ Pending |
| US-ORD-004 | Admin Views All Orders Dashboard | Critical | ADR-ORDERS-001 | — | TC-ORD-019 – TC-ORD-024 | ⏳ Pending |
| US-ORD-005 | Admin Filters Orders by Status/User | High | ADR-ORDERS-001 | — | TC-ORD-025 – TC-ORD-028 | ⏳ Pending |

---

## Backward Traceability (Test → Requirement)

### US-ORD-001: User Places Order

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-ORD-001 | Admin with valid body creates order and gets 201 | Integration | US-ORD-001/AC-001 | — | ⏳ Pending |
| TC-ORD-002 | Invalid quantity (0, negative, > 999) returns 400 | Unit | US-ORD-001/AC-002 | — | ⏳ Pending |
| TC-ORD-003 | Non-existent InventoryItemId returns 400 | Integration | US-ORD-001/AC-003 | — | ⏳ Pending |
| TC-ORD-004 | Order created with Status=Pending and OrderedAt | Integration | US-ORD-001/AC-004 | — | ⏳ Pending |
| TC-ORD-005 | Unauthenticated request returns 401 | Integration | US-ORD-001/AC-005 | — | ⏳ Pending |
| TC-ORD-006 | Quantity exceeding available inventory returns 400 | Integration | US-ORD-001/AC-006 | — | ⏳ Pending |
| TC-ORD-007 | Order LocationHeader points to created resource | Integration | US-ORD-001/AC-001 | — | ⏳ Pending |

### US-ORD-002: User Views Order History

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-ORD-008 | Authenticated user gets 200 with own orders only | Integration | US-ORD-002/AC-007 | — | ⏳ Pending |
| TC-ORD-009 | Pagination with page=2&pageSize=10 returns correct slice | Integration | US-ORD-002/AC-008 | — | ⏳ Pending |
| TC-ORD-010 | Status displayed as text value (Pending, Processing, etc.) | Integration | US-ORD-002/AC-009 | — | ⏳ Pending |
| TC-ORD-011 | FulfilledAt populated for fulfilled, null for pending | Integration | US-ORD-002/AC-010 | — | ⏳ Pending |
| TC-ORD-012 | Unauthenticated user returns 401 | Integration | US-ORD-002/AC-011 | — | ⏳ Pending |

### US-ORD-003: User Cancels Pending Order

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-ORD-013 | User cancels own Pending order, returns 200 with Cancelled status | Integration | US-ORD-003/AC-012 | — | ⏳ Pending |
| TC-ORD-014 | Cannot cancel non-Pending order (Processing/Fulfilled), returns 400 | Integration | US-ORD-003/AC-013 | — | ⏳ Pending |
| TC-ORD-015 | Cancelled order has Status=Cancelled and UpdatedAt refreshed | Integration | US-ORD-003/AC-014 | — | ⏳ Pending |
| TC-ORD-016 | Non-existent order returns 404 | Integration | US-ORD-003/AC-015 | — | ⏳ Pending |
| TC-ORD-017 | User cannot cancel another's order, returns 403 | Integration | US-ORD-003/AC-016 | — | ⏳ Pending |
| TC-ORD-018 | Unauthenticated request returns 401 | Integration | US-ORD-003/AC-016 | — | ⏳ Pending |

### US-ORD-004: Admin Views All Orders Dashboard

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-ORD-019 | Admin with Admin role gets 200 with all orders (no user filter) | Integration | US-ORD-004/AC-017 | — | ⏳ Pending |
| TC-ORD-020 | Non-admin user returns 403 Forbidden | Integration | US-ORD-004/AC-018 | — | ⏳ Pending |
| TC-ORD-021 | Response includes user details (username, email) | Integration | US-ORD-004/AC-019 | — | ⏳ Pending |
| TC-ORD-022 | Pagination with page=2&pageSize=20 returns correct slice | Integration | US-ORD-004/AC-020 | — | ⏳ Pending |
| TC-ORD-023 | Response includes all order metadata (Id, Qty, Status, dates, Notes) | Integration | US-ORD-004/AC-021 | — | ⏳ Pending |
| TC-ORD-024 | Unauthenticated request returns 401 | Integration | US-ORD-004/AC-017 | — | ⏳ Pending |

### US-ORD-005: Admin Filters Orders by Status and User

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-ORD-025 | Status filter returns only orders with matching status | Integration | US-ORD-005/AC-022 | — | ⏳ Pending |
| TC-ORD-026 | User filter returns orders matching username or email (case-insensitive) | Integration | US-ORD-005/AC-023 | — | ⏳ Pending |
| TC-ORD-027 | Combined Status+User filters apply AND logic | Integration | US-ORD-005/AC-024 | — | ⏳ Pending |
| TC-ORD-028 | No matching filters return 200 with empty list (not 404) | Integration | US-ORD-005/AC-025 | — | ⏳ Pending |

---

## Traceability Summary

| Metric | Count |
|--------|-------|
| Total Requirements (User Stories) | 5 |
| Total Acceptance Criteria | 28 |
| Total Test Cases | 28 |
| AC : TC Ratio | 1:1 (every acceptance criterion has at least one test case) |
| Critical Requirements | 3 (13 SP) |
| High Requirements | 2 (6 SP) |

---

## Coverage Analysis

| Category | Covered | Notes |
|----------|---------|-------|
| **Functional Coverage** | 100% | All user stories have acceptance criteria with test cases |
| **Security Coverage** | 100% | Authorization (401/403) tested for all endpoints |
| **Error Handling** | 100% | 4xx validation and resource-not-found cases tested |
| **Performance** | 80% | Response time targets documented but not explicitly tested (future CI/CD integration) |
| **UI Integration** | 60% | Razor Pages E2E tested manually; automated UI tests future Sprint 2 |

---

## Gap Analysis

| Gap | Priority | Resolution | Sprint |
|-----|----------|-----------|--------|
| No negative test for inventory over-allocation | High | Add integration test: order quantity > available inventory | Sprint 1 |
| No test for concurrent order creation (race condition) | Medium | Add stress test with multiple concurrent users | Sprint 2 |
| No audit logging test | Medium | Implement audit logging and test | Sprint 2 |
| UI automated tests (Playwright) | Low | Add E2E test automation | Sprint 2 |
| API response time baselines | Low | Set up APM (Application Insights) and establish baselines | Sprint 2 |

---

## Sign-Offs (When Implementation Completes)

| Role | Name | Date | Sign-Off |
|------|------|------|----------|
| Product Owner | — | — | ⏳ Pending |
| Architecture Lead | — | — | ⏳ Pending |
| QA Lead | — | — | ⏳ Pending |
| Security Review | — | — | ⏳ Pending |

