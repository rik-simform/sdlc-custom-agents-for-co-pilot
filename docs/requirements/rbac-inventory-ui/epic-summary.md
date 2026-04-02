# EPIC-RBAC-UI: Inventory UI Role Enforcement — Epic Summary

**Date**: 2026-04-02
**REQ-CLASS**: EXTEND
**Status**: Ready for Sprint Planning
**Epic Owner**: Product Owner (pending assignment)
**Prerequisite Epic**: EPIC-RBAC (US-RBAC-001 – US-RBAC-007) — API authorization layer

---

## Context

The EPIC-RBAC stories (US-RBAC-001 to US-RBAC-007) established correct authorization at the API layer.
`POST`, `PUT`, and `DELETE` inventory endpoints already require the `Admin` role.
`GET` endpoints are accessible to all authenticated users.

However, the **Web UI layer** (`MyProject.Web`) has not yet been updated to reflect these rules.
As of 2026-04-02 the following gaps exist:

| Gap | File | Risk |
|---|---|---|
| "Add Item" button shown to all authenticated users | `Index.cshtml` | High — User role user sees a button that will error on click |
| Edit and Delete buttons visible in every table row for all users | `Index.cshtml` | High — Same as above |
| `Create.cshtml.cs` only checks `IsAuthenticated()`, not role | `Create.cshtml.cs` | High — User role user can load the form page |
| `Edit.cshtml.cs` only checks `IsAuthenticated()`, not role | `Edit.cshtml.cs` | High — User role user can load the edit form for any item |
| `TokenService` has no `IsInRole()` method | `TokenService.cs` | High — Blocks implementation of all UI stories above |

This epic closes all four gaps.

---

## Story Inventory

| ID | Title | Priority | SP | Sprint | INVEST I | Dependencies |
|----|-------|----------|----|--------|----------|-------------|
| US-RBAC-008 | Extend TokenService with Role Awareness | Critical | 2 | Sprint 2 | pass | None |
| US-RBAC-009 | Hide Admin-Only Controls on Inventory Index Page | Critical | 2 | Sprint 2 | fail* | US-RBAC-008 |
| US-RBAC-010 | Guard Inventory Create Page from User Role | Critical | 1 | Sprint 2 | fail* | US-RBAC-008 |
| US-RBAC-011 | Guard Inventory Edit Page from User Role | Critical | 1 | Sprint 2 | fail* | US-RBAC-008, US-RBAC-010 |

> \* INVEST "Independent" fails for US-RBAC-009, 010, 011 because all depend on US-RBAC-008. This is an acceptable technical dependency within the same sprint — all four stories should be taken together.

---

## Totals

| Metric | Value |
|--------|-------|
| **Total Stories** | 4 |
| **Total Story Points** | 6 |
| **Total Acceptance Criteria** | 21 |
| **Total Test Cases** | 21 (TC-RBAC-040 – TC-RBAC-060) |
| **Critical Stories** | 4 (6 SP) |

---

## Recommended Sprint Allocation

### Sprint 2 — UI Role Enforcement (6 SP)

| Story | SP | Reason |
|-------|----|--------|
| US-RBAC-008 | 2 | Foundation — must be done first; enables all other stories |
| US-RBAC-009 | 2 | UI visibility — highest user-facing impact |
| US-RBAC-010 | 1 | Page guard — direct URL defense |
| US-RBAC-011 | 1 | Page guard — mirrors US-RBAC-010 on Edit page |

**Sprint Goal**: A `User` role user sees only the read-only inventory list with no Add, Edit, or Delete controls. Direct navigation to Create or Edit pages redirects back to the list with an informative message. Admin users experience no change in behaviour.

**Prerequisite**: EPIC-RBAC Sprint 1 (US-RBAC-001 to US-RBAC-007) must be complete — role seeding, JWT authentication, and API authorization required.

---

## Files to Modify

| File | Change Summary |
|------|---------------|
| `src/MyProject.Web/Services/TokenService.cs` | Add `UserRolesKey`, extend `StoreTokens` to accept roles, add `IsInRole(string role)` |
| `src/MyProject.Web/Services/AuthApiService.cs` | Extract role from JWT at login and pass to updated `StoreTokens` |
| `src/MyProject.Web/Pages/Inventory/Index.cshtml` | Wrap "Add Item" button, Edit buttons, Delete buttons, and delete modal in `@if (Model.IsAdmin)` |
| `src/MyProject.Web/Pages/Inventory/Index.cshtml.cs` | Add `IsAdmin` property; add role guard in `OnPostDeleteAsync` |
| `src/MyProject.Web/Pages/Inventory/Create.cshtml.cs` | Add `IsInRole("Admin")` guard in `OnGet` and `OnPostAsync` |
| `src/MyProject.Web/Pages/Inventory/Edit.cshtml.cs` | Add `IsInRole("Admin")` guard in `OnGetAsync` and `OnPostAsync` |

### No Changes to API Layer

The following files require **no modification**:

- `src/MyProject.Api/Endpoints/InventoryEndpoints.cs` — Already correct
- `src/MyProject.Application/` — No change
- `src/MyProject.Domain/` — No change
- `src/MyProject.Infrastructure/` — No change

---

## Security Defence-in-Depth Summary

| Layer | Control | Covers |
|---|---|---|
| API — `InventoryEndpoints.cs` | `.RequireAuthorization(policy => policy.RequireRole("Admin"))` on POST/PUT/DELETE | Prevents direct API calls from User role (already implemented) |
| Web UI — View | `@if (Model.IsAdmin)` conditional rendering | Hides controls from User role in the browser |
| Web UI — PageModel | `IsInRole("Admin")` check on every write handler | Prevents crafted HTTP requests from bypassing the UI controls |

All three layers together satisfy the defence-in-depth principle from the security instructions.

---

## Definition of Done

- [ ] All 4 stories implemented and code-reviewed
- [ ] All 21 acceptance criteria verified
- [ ] `User` role user sees only the inventory table — no Add/Edit/Delete controls
- [ ] `Admin` role user sees all controls and all pages unchanged
- [ ] Direct GET to `/Inventory/Create` and `/Inventory/Edit/{id}` by `User` role redirects to Index
- [ ] Direct POST to any write path by `User` role is blocked server-side
- [ ] `TokenService.IsInRole` returns correct results for both roles
- [ ] Zero security findings from CodeQL scan
- [ ] Product Owner review completed
