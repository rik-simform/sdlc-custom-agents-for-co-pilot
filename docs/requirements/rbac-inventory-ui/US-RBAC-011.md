# US-RBAC-011: Guard Inventory Edit Page from User Role Direct Access

**REQ-CLASS**: EXTEND
**INVEST**: I=fail (depends on US-RBAC-008, US-RBAC-010) N=pass V=pass E=pass S=pass T=pass
**Type**: Functional
**Priority**: Critical
**Estimate**: 1 Story Point
**Source**: EPIC-RBAC-UI / User Request 2026-04-02
**Status**: Ready

> **INVEST Note**: This story mirrors US-RBAC-010 for the Edit page. Technical dependency on US-RBAC-008.
> Can be developed alongside US-RBAC-010 but requires US-RBAC-008 to deploy.

---

## User Story

**As a** User (with the `User` role)
**I want to** be redirected away from `/Inventory/Edit/{id}` if I navigate to it directly
**So that** I am never shown an edit form I am not permitted to submit, and I avoid a confusing 403 error

---

## Acceptance Criteria

- [ ] **AC-001**: Given a logged-in `User` role user, when they perform a `GET` request to `/Inventory/Edit/{valid-guid}`, then the server redirects them to `/Inventory/Index`.
- [ ] **AC-002**: Given the redirect in AC-001, when the `/Inventory/Index` page renders, then a TempData error message **"You do not have permission to access that page."** is displayed as an alert banner.
- [ ] **AC-003**: Given a logged-in `User` role user, when they submit a crafted `POST` to `/Inventory/Edit/{valid-guid}` with valid fields, then the server redirects them to `/Inventory/Index` **without updating the inventory item**.
- [ ] **AC-004**: Given a logged-in `Admin` role user, when they navigate to `/Inventory/Edit/{valid-guid}`, then the page loads normally and the pre-populated edit form is rendered — existing behaviour fully preserved.
- [ ] **AC-005**: Given an unauthenticated user (no session), when they navigate to `/Inventory/Edit/{valid-guid}`, then they are redirected to `/Account/Login` — **this existing behaviour must not be changed**.

---

## Non-Functional Requirements

| Category | Requirement |
|---|---|
| Performance | Role check is a single session read — no measurable latency added to page load |
| Security | Server-side guard in `OnGetAsync` prevents the form HTML from ever reaching the client for non-Admin users |
| Security | Server-side guard in `OnPostAsync` ensures no item is updated even via a crafted form POST |
| UX | Redirect is seamless with an informative banner; no raw HTTP error codes exposed to the user |

---

## Affected Modules

| Module | File | Change Type | Risk |
|---|---|---|---|
| Web/Pages/Inventory | `src/MyProject.Web/Pages/Inventory/Edit.cshtml.cs` | Modify — add `IsInRole("Admin")` guards in `OnGetAsync` and `OnPostAsync` | Low |

---

## .NET Implementation Notes

```csharp
public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct)
{
    if (!tokenService.IsAuthenticated())
        return RedirectToPage("/Account/Login");

    if (!tokenService.IsInRole("Admin"))    // ← new
    {
        TempData["Error"] = "You do not have permission to access that page.";
        return RedirectToPage("/Inventory/Index");
    }

    inventoryApi.SetBearerToken(tokenService.GetAccessToken()!);
    var (item, error) = await inventoryApi.GetByIdAsync(id, ct);
    // ... rest of existing logic unchanged
}

public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken ct)
{
    if (!tokenService.IsAuthenticated())
        return RedirectToPage("/Account/Login");

    if (!tokenService.IsInRole("Admin"))    // ← new
    {
        TempData["Error"] = "You do not have permission to access that page.";
        return RedirectToPage("/Inventory/Index");
    }

    if (!ModelState.IsValid) return Page();
    // ... rest of existing logic unchanged
}
```

---

## Dependencies

| ID | Dependency | Type |
|---|---|---|
| DEP-001 | US-RBAC-008 — `TokenService.IsInRole(string role)` must exist | Story |
| DEP-002 | US-RBAC-010 — Consistent redirect + TempData pattern established for Create — reuse exactly the same pattern here | Story |

---

## Assumptions

- ASM-002: Users have exactly one role at a time (either `Admin` or `User`).

---

## Linked Artifacts

- Tests: TC-RBAC-056 – TC-RBAC-060
- Implementation: —
- Design: —
