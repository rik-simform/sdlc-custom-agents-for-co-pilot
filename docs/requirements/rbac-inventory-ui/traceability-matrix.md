# Requirements Traceability Matrix — EPIC-RBAC-UI

**Last Updated**: 2026-04-02

---

## Forward Traceability (Requirement → Verification)

| Req ID | Title | Priority | SP | REQ-CLASS | Design | Implementation | Test Cases | Status |
|--------|-------|----------|----|-----------|--------|----------------|------------|--------|
| US-RBAC-008 | Extend TokenService with Role Awareness | Critical | 2 | EXTEND | — | — | TC-RBAC-040 – TC-RBAC-044 | ⏳ Pending |
| US-RBAC-009 | Hide Admin-Only Controls on Inventory Index Page | Critical | 2 | EXTEND | — | — | TC-RBAC-045 – TC-RBAC-050 | ⏳ Pending |
| US-RBAC-010 | Guard Inventory Create Page from User Role | Critical | 1 | EXTEND | — | — | TC-RBAC-051 – TC-RBAC-055 | ⏳ Pending |
| US-RBAC-011 | Guard Inventory Edit Page from User Role | Critical | 1 | EXTEND | — | — | TC-RBAC-056 – TC-RBAC-060 | ⏳ Pending |

---

## Backward Traceability (Test Case → Requirement)

### US-RBAC-008: Extend TokenService with Role Awareness

| Test ID | Test Name | Type | Req AC | Status |
|---------|-----------|------|--------|--------|
| TC-RBAC-040 | StoreTokens_WithRoles_PersistsRoleInSession | Unit | AC-001 | ⏳ Pending |
| TC-RBAC-041 | IsInRole_WithMatchingRole_ReturnsTrue | Unit | AC-002 | ⏳ Pending |
| TC-RBAC-042 | IsInRole_WithNonMatchingRole_ReturnsFalse | Unit | AC-003 | ⏳ Pending |
| TC-RBAC-043 | IsInRole_WithNoSessionOrMissingKey_ReturnsFalseWithoutThrowing | Unit | AC-004 | ⏳ Pending |
| TC-RBAC-044 | IsInRole_AfterClear_ReturnsFalse | Unit | AC-005 | ⏳ Pending |

### US-RBAC-009: Hide Admin-Only Controls on Inventory Index Page

| Test ID | Test Name | Type | Req AC | Status |
|---------|-----------|------|--------|--------|
| TC-RBAC-045 | IndexPage_UserRole_AddItemButtonNotRendered | Integration | AC-001 | ⏳ Pending |
| TC-RBAC-046 | IndexPage_UserRole_EditButtonsNotRendered | Integration | AC-002 | ⏳ Pending |
| TC-RBAC-047 | IndexPage_UserRole_DeleteButtonsAndModalNotRendered | Integration | AC-003 | ⏳ Pending |
| TC-RBAC-048 | IndexPage_AdminRole_AllControlsRendered | Integration | AC-004 | ⏳ Pending |
| TC-RBAC-049 | IndexPage_UserRole_CraftedDeletePost_RedirectsWithoutCallingApi | Unit | AC-005 | ⏳ Pending |
| TC-RBAC-050 | IndexPage_UserRole_InventoryTableDataFullyVisible | Integration | AC-006 | ⏳ Pending |

### US-RBAC-010: Guard Inventory Create Page from User Role

| Test ID | Test Name | Type | Req AC | Status |
|---------|-----------|------|--------|--------|
| TC-RBAC-051 | CreatePage_UserRole_GetRedirectsToIndex | Integration | AC-001 | ⏳ Pending |
| TC-RBAC-052 | CreatePage_UserRole_RedirectShowsTempDataError | Integration | AC-002 | ⏳ Pending |
| TC-RBAC-053 | CreatePage_UserRole_CraftedPostRedirectsWithoutCreating | Unit | AC-003 | ⏳ Pending |
| TC-RBAC-054 | CreatePage_AdminRole_GetLoadsFormNormally | Integration | AC-004 | ⏳ Pending |
| TC-RBAC-055 | CreatePage_UnauthenticatedUser_RedirectsToLogin | Integration | AC-005 | ⏳ Pending |

### US-RBAC-011: Guard Inventory Edit Page from User Role

| Test ID | Test Name | Type | Req AC | Status |
|---------|-----------|------|--------|--------|
| TC-RBAC-056 | EditPage_UserRole_GetRedirectsToIndex | Integration | AC-001 | ⏳ Pending |
| TC-RBAC-057 | EditPage_UserRole_RedirectShowsTempDataError | Integration | AC-002 | ⏳ Pending |
| TC-RBAC-058 | EditPage_UserRole_CraftedPostRedirectsWithoutUpdating | Unit | AC-003 | ⏳ Pending |
| TC-RBAC-059 | EditPage_AdminRole_GetLoadsPrePopulatedFormNormally | Integration | AC-004 | ⏳ Pending |
| TC-RBAC-060 | EditPage_UnauthenticatedUser_RedirectsToLogin | Integration | AC-005 | ⏳ Pending |

---

## Dependency Graph

```
US-RBAC-008 (TokenService Role Awareness)
    ├── US-RBAC-009 (Hide Index Page Controls)
    ├── US-RBAC-010 (Guard Create Page)
    │       └── US-RBAC-011 (Guard Edit Page)
    └── US-RBAC-011 (Guard Edit Page)
```

US-RBAC-008 must be completed before any of the other three stories can be deployed.
US-RBAC-009, 010, and 011 can be developed in parallel once US-RBAC-008 is merged.

---

## Cross-Epic Traceability Link

This matrix extends EPIC-RBAC. The full API-layer traceability for roles is in:
`docs/requirements/rbac-inventory/traceability-matrix.md`

| EPIC-RBAC Story | Relationship |
|---|---|
| US-RBAC-001 | API POST restriction → enforced by US-RBAC-009/010 in UI |
| US-RBAC-002 | API PUT restriction → enforced by US-RBAC-009/011 in UI |
| US-RBAC-003 | API DELETE restriction → enforced by US-RBAC-009 in UI |
| US-RBAC-004 | User Views Inventory List → read access preserved in US-RBAC-009 (AC-006) |
| US-RBAC-006 | Role Seeding → provides Admin/User roles consumed by TokenService in US-RBAC-008 |
