# US-RBAC-008: Extend TokenService with Role Awareness

**REQ-CLASS**: EXTEND
**INVEST**: I=pass N=pass V=pass E=pass S=pass T=pass
**Type**: Functional
**Priority**: Critical
**Estimate**: 2 Story Points
**Source**: EPIC-RBAC-UI / User Request 2026-04-02
**Status**: Ready

---

## User Story

**As a** developer building role-aware Razor Pages
**I want to** have a single `IsInRole(string role)` method on `TokenService` backed by session storage
**So that** all Razor Page models can perform consistent role checks without each one duplicating JWT-decoding logic

---

## Acceptance Criteria

- [ ] **AC-001**: Given a successful login where roles are passed to `StoreTokens`, when `StoreTokens("token", "refresh", "email@x.com", "Admin")` is called, then the role string is persisted in the HTTP session under a `UserRoles` key.
- [ ] **AC-002**: Given a session containing `UserRoles = "Admin"`, when `IsInRole("Admin")` is called, then it returns `true`.
- [ ] **AC-003**: Given a session containing `UserRoles = "User"`, when `IsInRole("Admin")` is called, then it returns `false`.
- [ ] **AC-004**: Given no active session, or a session where `UserRoles` is absent, when `IsInRole` is called with any value, then it returns `false` without throwing an exception.
- [ ] **AC-005**: Given a session cleared via `Clear()`, when `IsInRole` is called, then it returns `false`.

---

## Non-Functional Requirements

| Category | Requirement |
|---|---|
| Performance | Role check completes in constant time (single session key lookup); negligible overhead on every page render |
| Security | Roles stored in server-side HTTP session — not readable by client-side JavaScript |
| Security | Role string is never logged or included in error responses |
| Maintainability | `IsInRole` is case-insensitive to tolerate JWT claim casing variations |
| Extendability | Comma-separated role storage format allows multi-role support in the future without a breaking change |

---

## Affected Modules

| Module | File | Change Type | Risk |
|---|---|---|---|
| Web/Services | `src/MyProject.Web/Services/TokenService.cs` | Extend — new overload + new method | Low |
| Web/Services | `src/MyProject.Web/Services/AuthApiService.cs` | Extend — extract role from JWT at login and pass to `StoreTokens` | Low |

---

## .NET Implementation Notes

### TokenService changes

```csharp
private const string UserRolesKey = "UserRoles";

/// <summary>Saves access token, refresh token, user email, and roles to session.</summary>
public void StoreTokens(string accessToken, string refreshToken, string email, string roles)
{
    Session.SetString(AccessTokenKey, accessToken);
    Session.SetString(RefreshTokenKey, refreshToken);
    Session.SetString(UserEmailKey, email);
    Session.SetString(UserRolesKey, roles);
}

/// <summary>Returns true when the current session contains the specified role.</summary>
public bool IsInRole(string role) =>
    Session.GetString(UserRolesKey)
        ?.Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Contains(role, StringComparer.OrdinalIgnoreCase) ?? false;
```

### AuthApiService — extracting roles at login

```csharp
// After successful login, extract role from JWT payload
var handler = new JwtSecurityTokenHandler();
var jwt = handler.ReadJwtToken(loginResponse.AccessToken);
var roles = string.Join(",", jwt.Claims
    .Where(c => c.Type == ClaimTypes.Role)
    .Select(c => c.Value));

tokenService.StoreTokens(loginResponse.AccessToken, loginResponse.RefreshToken, email, roles);
```

### Backward compatibility

The existing three-argument `StoreTokens(string, string, string)` overload should be retained or updated to a four-argument version. All existing callers must be updated to pass roles.

---

## Dependencies

| ID | Dependency | Type |
|---|---|---|
| DEP-001 | `System.IdentityModel.Tokens.Jwt` NuGet package must be referenced in `MyProject.Web.csproj` for `JwtSecurityTokenHandler` | Infrastructure |
| DEP-002 | JWT tokens issued by the API contain `ClaimTypes.Role` claims (confirmed in `JwtTokenService` + tests) | Cross-Project |

---

## Assumptions

- ASM-001: JWT tokens issued by the API already contain the user's role as a `ClaimTypes.Role` claim.
- ASM-003: HTTP Session is available and configured in `MyProject.Web`.

---

## Linked Artifacts

- Tests: TC-RBAC-040 – TC-RBAC-044
- Implementation: —
- Design: —
