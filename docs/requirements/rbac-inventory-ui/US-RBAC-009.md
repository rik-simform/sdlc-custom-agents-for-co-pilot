# US-RBAC-009: Hide Admin-Only Controls from User Role on Inventory Index Page

**REQ-CLASS**: EXTEND
**INVEST**: I=fail (depends on US-RBAC-008) N=pass V=pass E=pass S=pass T=pass
**Type**: Functional
**Priority**: Critical
**Estimate**: 2 Story Points
**Source**: EPIC-RBAC-UI / User Request 2026-04-02
**Status**: Ready

> **INVEST Note**: This story has a dependency on US-RBAC-008 (TokenService role awareness). It can be
> developed in parallel but not deployed independently. The dependency is technical, not business.

---

## User Story

**As a** User (with the `User` role)
**I want to** see only the inventory list on the Index page, without any Add, Edit, or Delete controls
**So that** the UI clearly reflects my read-only permissions and I am not presented with buttons that would result in an authorization error

---

## Acceptance Criteria

- [ ] **AC-001**: Given a logged-in `User` role user, when they navigate to `/Inventory/Index`, then the **"Add Item"** button is absent from the rendered HTML.
- [ ] **AC-002**: Given a logged-in `User` role user, when they navigate to `/Inventory/Index`, then **no Edit button** (`<a asp-page="/Inventory/Edit">`) is rendered in any table row.
- [ ] **AC-003**: Given a logged-in `User` role user, when they navigate to `/Inventory/Index`, then **no Delete button** is rendered in any table row and the delete confirmation modal markup is not present in the page.
- [ ] **AC-004**: Given a logged-in `Admin` role user, when they navigate to `/Inventory/Index`, then the "Add Item" button, Edit buttons per row, and Delete buttons per row are **all rendered** as before.
- [ ] **AC-005**: Given a logged-in `User` role user, when they craft a direct HTTP POST to the `OnPostDeleteAsync` handler, then the handler returns a redirect to `/Inventory/Index` **without calling the API** and without deleting any item.
- [ ] **AC-006**: Given a logged-in `User` role user, when they view `/Inventory/Index`, then the **read-only inventory table** (SKU, Name, Category, Stock, Reorder Level, Unit Price, Location, Status) is **fully visible** — no data is hidden from them.

---

## Non-Functional Requirements

| Category | Requirement |
|---|---|
| Performance | Conditional rendering is server-side Razor — no extra HTTP calls or JavaScript evaluation |
| Security | Controls are hidden via server-side role check in the PageModel, NOT via `display:none` CSS or client-side JavaScript — cannot be bypassed by DOM manipulation |
| Security | `OnPostDeleteAsync` server-side guard prevents a crafted POST from reaching the API |
| UX | The inventory table layout is identical for both roles — only the action column and "Add Item" button are conditionally shown/hidden |

---

## Affected Modules

| Module | File | Change Type | Risk |
|---|---|---|---|
| Web/Pages/Inventory | `src/MyProject.Web/Pages/Inventory/Index.cshtml` | Modify — wrap "Add Item" button, Edit button, Delete button, and modal in `@if (Model.IsAdmin)` | Low |
| Web/Pages/Inventory | `src/MyProject.Web/Pages/Inventory/Index.cshtml.cs` | Extend — add `public bool IsAdmin { get; private set; }` property populated from `tokenService.IsInRole("Admin")` in `OnGetAsync`; add role guard in `OnPostDeleteAsync` | Low |

---

## .NET Implementation Notes

### IndexModel changes

```csharp
public bool IsAdmin { get; private set; }

public async Task<IActionResult> OnGetAsync(string? filter, CancellationToken ct)
{
    if (!tokenService.IsAuthenticated())
        return RedirectToPage("/Account/Login");

    IsAdmin = tokenService.IsInRole("Admin");  // ← new
    // ... rest of existing logic unchanged
}

public async Task<IActionResult> OnPostDeleteAsync(Guid id, CancellationToken ct)
{
    if (!tokenService.IsAuthenticated())
        return RedirectToPage("/Account/Login");

    if (!tokenService.IsInRole("Admin"))       // ← new guard
    {
        TempData["Error"] = "You do not have permission to delete inventory items.";
        return RedirectToPage();
    }

    // ... rest of existing logic unchanged
}
```

### Index.cshtml conditional rendering

```razor
@* "Add Item" button — Admin only *@
@if (Model.IsAdmin)
{
    <a asp-page="/Inventory/Create" class="btn btn-primary">
        <i class="bi bi-plus-circle me-1"></i>Add Item
    </a>
}

@* Inside table row — action column *@
<td class="text-end text-nowrap">
    @if (Model.IsAdmin)
    {
        <a asp-page="/Inventory/Edit" asp-route-id="@item.Id"
           class="btn btn-sm btn-outline-primary me-1">
            <i class="bi bi-pencil"></i>
        </a>
        <button type="button" class="btn btn-sm btn-outline-danger"
                data-bs-toggle="modal" data-bs-target="#deleteModal"
                data-item-id="@item.Id" data-item-name="@item.Name">
            <i class="bi bi-trash"></i>
        </button>
    }
</td>

@* Delete modal — Admin only *@
@if (Model.IsAdmin)
{
    <!-- existing delete modal markup -->
}
```

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

- Tests: TC-RBAC-045 – TC-RBAC-050
- Implementation: —
- Design: —
