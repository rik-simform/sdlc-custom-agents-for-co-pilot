# US-RBAC-010: Guard Inventory Create Page from User Role Direct Access

**REQ-CLASS**: EXTEND
**INVEST**: I=fail (depends on US-RBAC-008) N=pass V=pass E=pass S=pass T=pass
**Type**: Functional
**Priority**: Critical
**Estimate**: 1 Story Point
**Source**: EPIC-RBAC-UI / User Request 2026-04-02
**Status**: Ready

> **INVEST Note**: This story has a technical dependency on US-RBAC-008 (TokenService role awareness).
> Development can proceed in parallel but the deployment gate requires US-RBAC-008.

---

## User Story

**As a** User (with the `User` role)
**I want to** be redirected away from `/Inventory/Create` if I navigate to it directly
**So that** I am never shown a form I am not permitted to submit, and I avoid a confusing 403 error

---

## Acceptance Criteria

- [ ] **AC-001**: Given a logged-in `User` role user, when they perform a `GET` request to `/Inventory/Create`, then the server redirects them to `/Inventory/Index`.
- [ ] **AC-002**: Given the redirect in AC-001, when the `/Inventory/Index` page renders, then a TempData error message **"You do not have permission to access that page."** is displayed as an alert banner.
- [ ] **AC-003**: Given a logged-in `User` role user, when they submit a crafted `POST` to `/Inventory/Create` with a valid request body, then the server redirects them to `/Inventory/Index` **without creating any inventory item**.
- [ ] **AC-004**: Given a logged-in `Admin` role user, when they navigate to `/Inventory/Create`, then the **page loads normally** and the Create form is rendered — existing behaviour is fully preserved.
- [ ] **AC-005**: Given an unauthenticated user (no session), when they navigate to `/Inventory/Create`, then they are redirected to `/Account/Login` — **this existing behaviour must not be changed**.

---

## Non-Functional Requirements

| Category | Requirement |
|---|---|
| Performance | Role check is a single session read — no measurable latency added to page load |
| Security | Server-side guard in `OnGet` prevents the form HTML from ever reaching the client for non-Admin users — not bypassable via URL manipulation |
| Security | Server-side guard in `OnPost` ensures no item is created even via a crafted form POST |
| UX | Redirect is seamless with an informative banner; no raw HTTP error codes exposed to the user |

---

## Affected Modules

| Module | File | Change Type | Risk |
|---|---|---|---|
| Web/Pages/Inventory | `src/MyProject.Web/Pages/Inventory/Create.cshtml.cs` | Modify — add `IsInRole("Admin")` guards in `OnGet` and `OnPostAsync` | Low |

---

## .NET Implementation Notes

```csharp
public IActionResult OnGet()
{
    if (!tokenService.IsAuthenticated())
        return RedirectToPage("/Account/Login");

    if (!tokenService.IsInRole("Admin"))   // ← new
    {
        TempData["Error"] = "You do not have permission to access that page.";
        return RedirectToPage("/Inventory/Index");
    }

    return Page();
}

public async Task<IActionResult> OnPostAsync(CancellationToken ct)
{
    if (!tokenService.IsAuthenticated())
        return RedirectToPage("/Account/Login");

    if (!tokenService.IsInRole("Admin"))   // ← new
    {
        TempData["Error"] = "You do not have permission to access that page.";
        return RedirectToPage("/Inventory/Index");
    }

    if (!ModelState.IsValid) return Page();

    // ... rest of existing logic unchanged
}
```

### TempData display in Index.cshtml

```razor
@if (TempData["Error"] is string errorMsg)
{
    <div class="alert alert-danger alert-dismissible fade show" role="alert">
        @errorMsg
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </div>
}
```

> Note: If `Index.cshtml` already displays `Model.ErrorMessage` as an alert, the same visual pattern
> should be used for the TempData message for consistency.

---

## Dependencies

| ID | Dependency | Type |
|---|---|---|
| DEP-001 | US-RBAC-008 — `TokenService.IsInRole(string role)` must exist | Story |

---

## Assumptions

- ASM-002: Users have exactly one role at a time (either `Admin` or `User`).

---

## Linked Artifacts

- Tests: TC-RBAC-051 – TC-RBAC-055
- Implementation: —
- Design: —
