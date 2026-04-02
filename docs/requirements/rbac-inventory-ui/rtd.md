# Requirements Traceability Document — EPIC-RBAC-UI: Inventory UI Role Enforcement

**Generated**: 2026-04-02
**Author**: SDLC Requirements Engineer Agent
**Version**: 1.0
**REQ-CLASS**: EXTEND

---

## Pre-Analysis Summary

**REQ-CLASS**: EXTEND — The API authorization layer is already correct. `POST`, `PUT`, and `DELETE`
inventory endpoints already require the `Admin` role in `InventoryEndpoints.cs`. This epic extends
that security to the Web UI layer to ensure users cannot see or navigate to write operations.

### Decision: PROCEED
No critical or high blockers. API defense-in-depth is already in place. All UI gaps can be resolved
within a single sprint with the proposed approach below.

---

## 1. Summary of Ambiguities

| AMB-ID | Area | Ambiguity Description | Risk Level | Business Impact if Unresolved | Proposed Resolution |
|---|---|---|---|---|---|
| AMB-001 | Architecture | `TokenService` has no role-awareness; stores JWT as opaque string with no `IsInRole()` method | High | All UI pages would need to decode JWT manually — inconsistent, error-prone | Store user roles in HTTP session at login time; add `IsInRole(string role)` to `TokenService` |
| AMB-002 | UX / Navigation | Should a User navigating directly to `/Inventory/Create` or `/Inventory/Edit/{id}` see a 403 page or be silently redirected to `/Inventory/Index`? | Medium | Silent redirect is better UX; 403 page risks confusion for legitimate users who deep-linked | Redirect to `/Inventory/Index` with a TempData error message |
| AMB-003 | Security | The `OnPostDeleteAsync` handler on `Index.cshtml.cs` has no role check — a User who crafts a raw HTTP POST could attempt a delete even if the button is hidden | Medium | The API will still return 403, but the Web UI layer should also block this to prevent error-state confusion | Add `IsInRole("Admin")` guard to `OnPostDeleteAsync` in `IndexModel` |
| AMB-004 | Configuration | Should role be stored in session as a plain string or as a comma-separated list for multi-role support? | Low | Single-role system has no issue now; multi-role could be added later | Store as comma-separated string in session; `IsInRole` checks if string contains the role name |

---

## 2. Impact Analysis on Current Architecture and Flows

### API Layer — No Changes Required

| Area | Affected File | Impact Level | Description |
|---|---|---|---|
| Inventory endpoints | `src/MyProject.Api/Endpoints/InventoryEndpoints.cs` | None | Already requires `Admin` role on POST, PUT, DELETE. GET remains open to all authenticated users. |

### Web UI Layer — Changes Required

| Area | Affected File | Impact Level | Description |
|---|---|---|---|
| TokenService | `src/MyProject.Web/Services/TokenService.cs` | High | No `IsInRole()` or role-retrieval method. Must add role storage + role check to support all UI stories. |
| Index Razor View | `src/MyProject.Web/Pages/Inventory/Index.cshtml` | High | "Add Item" button, Edit button per row, and Delete button per row are all unconditionally rendered. Need conditional rendering gated on Admin role. |
| Index Page Model | `src/MyProject.Web/Pages/Inventory/Index.cshtml.cs` | Medium | Must expose `IsAdmin` bool property to the view. `OnPostDeleteAsync` must also guard against non-Admin role. |
| Create Page Model | `src/MyProject.Web/Pages/Inventory/Create.cshtml.cs` | High | `OnGet` and `OnPost` only check `IsAuthenticated()`. A User-role user can currently load the form and submit (will get 403 back from the API). Must add `IsInRole("Admin")` check with redirect. |
| Edit Page Model | `src/MyProject.Web/Pages/Inventory/Edit.cshtml.cs` | High | Same issue as Create — only checks `IsAuthenticated()`. Must add `IsInRole("Admin")` check with redirect. |
| Auth Flow (Login) | `src/MyProject.Web/Services/AuthApiService.cs` | Medium | Login must extract the role from the JWT response and pass it to `TokenService.StoreTokens()` so it can be stored in session. |

---

## 3. Clarifying Questions (Grouped by Area)

### Architecture
| ID | Question | Risk if Unanswered | Would Change Design | Would Override Current Flow | Suggested Default |
|---|---|---|---|---|---|
| CQ-001 | Should `TokenService` decode the JWT at parse time, or should roles be stored in session separately at login time? | Medium | Yes | Yes | Store in session at login — avoids JWT decoding on every page load |
| CQ-002 | Should multi-role support be designed in now (e.g., user can have both Admin and User roles)? | Low | Yes | No | No — single active role per user for now; use comma-separated string to allow future extension |

### UX / Navigation
| ID | Question | Risk if Unanswered | Would Change Design | Would Override Current Flow | Suggested Default |
|---|---|---|---|---|---|
| CQ-003 | What should a User see when redirected away from a forbidden page — a toast, a banner, or nothing? | Low | No | No | TempData error banner on `/Inventory/Index` saying "You do not have permission to access that page." |

### Security
| ID | Question | Risk if Unanswered | Would Change Design | Would Override Current Flow | Suggested Default |
|---|---|---|---|---|---|
| CQ-004 | Should the Delete form POST also be role-guarded on the server side in `IndexModel.OnPostDeleteAsync`? | Medium | No | Yes — adds a server-side role check that currently doesn't exist | Yes — add role guard to prevent crafted POSTs from non-Admin users reaching the API |

---

## 4. Proposed Resolutions and Design Notes

### Resolution for AMB-001: TokenService Role Awareness

Extend `TokenService` to store roles in session and expose `IsInRole(string role)`:

```csharp
// Extended StoreTokens overload
public void StoreTokens(string accessToken, string refreshToken, string email, string roles)
{
    Session.SetString(AccessTokenKey, accessToken);
    Session.SetString(RefreshTokenKey, refreshToken);
    Session.SetString(UserEmailKey, email);
    Session.SetString(UserRolesKey, roles); // comma-separated roles string
}

public bool IsInRole(string role) =>
    Session.GetString(UserRolesKey)
        ?.Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Contains(role, StringComparer.OrdinalIgnoreCase) ?? false;
```

At login in `AuthApiService`, decode the JWT to extract `ClaimTypes.Role` and pass to `StoreTokens`.

### Resolution for AMB-002: Forbidden Page Redirect

```csharp
// In Create.cshtml.cs and Edit.cshtml.cs OnGet/OnPost
if (!tokenService.IsInRole("Admin"))
{
    TempData["Error"] = "You do not have permission to access that page.";
    return RedirectToPage("/Inventory/Index");
}
```

### Resolution for AMB-003: Server-Side Delete Guard

```csharp
// In Index.cshtml.cs OnPostDeleteAsync
if (!tokenService.IsInRole("Admin"))
{
    TempData["Error"] = "You do not have permission to delete inventory items.";
    return RedirectToPage();
}
```

### Resolution for AMB-004: Role Storage Format

Store as `Session.SetString("UserRoles", "Admin")` or `"User"`. Use comma-separation if a user has multiple roles (e.g., `"Admin,User"`). `IsInRole` checks using `Contains` with `OrdinalIgnoreCase`.

---

## 5. User Stories with Acceptance Criteria

See individual story files:

- [US-RBAC-008.md](US-RBAC-008.md) — Extend TokenService with Role Awareness
- [US-RBAC-009.md](US-RBAC-009.md) — Hide Admin-Only Controls from User Role on Inventory Index Page
- [US-RBAC-010.md](US-RBAC-010.md) — Guard Inventory Create Page from User Role Direct Access
- [US-RBAC-011.md](US-RBAC-011.md) — Guard Inventory Edit Page from User Role Direct Access

---

## 6. Assumptions and Constraints

| ID | Text | Category | Impact if Wrong |
|---|---|---|---|
| ASM-001 | JWT tokens issued by the API already contain the user's role as a `ClaimTypes.Role` claim | Architecture | Role cannot be extracted from JWT at login time without code changes to `JwtTokenService` |
| ASM-002 | Users have exactly one role at a time (either `Admin` or `User`) | Business Rule | Multi-role users could get incorrect visibility if only the first role is checked |
| ASM-003 | HTTP Session is available and configured in `MyProject.Web` | Infrastructure | `TokenService` depends on `IHttpContextAccessor` and session middleware — already confirmed in existing code |
| ASM-004 | The `AuthApiService` parses the JWT login response and has access to the raw token string | Architecture | Role extraction at login time requires either parsing the JWT or the API returning role in the login response body |
| ASM-005 | The existing `LoginResponse` DTO from the API includes roles in the body or they can be read from the JWT | API Contract | If roles are not in the response body, JWT parsing with `JwtSecurityTokenHandler` is needed in the Web project |

---

## 7. Blockers

None. The API correctly enforces all authorization rules. UI gaps are additive and do not require schema or API changes.

---

## 8. Requirements Traceability Matrix

| Req ID | Title | REQ-CLASS | Affected Modules | Design | Implementation | Test Cases | Status |
|---|---|---|---|---|---|---|---|
| US-RBAC-008 | Extend TokenService with Role Awareness | EXTEND | Web/Services | — | — | TC-RBAC-040 – TC-RBAC-044 | ⏳ Pending |
| US-RBAC-009 | Hide Admin-Only Controls on Index Page | EXTEND | Web/Pages/Inventory | — | — | TC-RBAC-045 – TC-RBAC-050 | ⏳ Pending |
| US-RBAC-010 | Guard Inventory Create Page from User Role | EXTEND | Web/Pages/Inventory | — | — | TC-RBAC-051 – TC-RBAC-055 | ⏳ Pending |
| US-RBAC-011 | Guard Inventory Edit Page from User Role | EXTEND | Web/Pages/Inventory | — | — | TC-RBAC-056 – TC-RBAC-060 | ⏳ Pending |
