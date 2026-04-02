# US-RBAC-006: Role Seeding and Assignment

**Type**: Functional
**Priority**: High
**Story Points**: 5
**Source**: EPIC-RBAC / PRD Section 6
**Status**: Ready

---

## User Story

**As a** System Administrator
**I want to** have `Admin` and `User` roles automatically seeded on application startup, and allow Admins to assign roles to users
**So that** role-based access control is available immediately after deployment without manual database intervention

---

## Acceptance Criteria

- [ ] **AC-001**: Given a fresh database, when the application starts, then the `Admin` and `User` roles exist in the `AspNetRoles` table.
- [ ] **AC-002**: Given the roles already exist in the database, when the application starts again, then no duplicate roles are created (idempotent seeding).
- [ ] **AC-003**: Given an authenticated Admin user and a valid target user ID, when they call `POST /api/v1/auth/roles/assign` with `{ "userId": "...", "role": "Admin" }`, then the target user is added to the specified role and the system returns HTTP 200.
- [ ] **AC-004**: Given an authenticated Admin user, when they call `POST /api/v1/auth/roles/assign` with a role name that does not exist, then the system returns HTTP 400 with a ProblemDetails error.
- [ ] **AC-005**: Given an authenticated Admin user, when they call `POST /api/v1/auth/roles/assign` for a user who already has the specified role, then the system returns HTTP 200 (idempotent — no error).
- [ ] **AC-006**: Given a request from a non-Admin user, when they call `POST /api/v1/auth/roles/assign`, then the system returns HTTP 403 Forbidden.
- [ ] **AC-007**: Given a successful role assignment, then an audit log entry is recorded with `Action = "RoleAssigned"`, target user ID, role name, and Admin actor ID.
- [ ] **AC-008**: Given a new user registers via `POST /api/v1/auth/register`, then they are automatically assigned the `User` role by default.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | Role seeding completes in < 2s on application startup |
| Security | Role assignment restricted to Admin role only |
| Security | Cannot self-escalate — assigning Admin role requires existing Admin |
| Security | Role changes trigger re-authentication on next token refresh |
| Availability | Seeding is idempotent; safe for rolling deployments |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-LOGIN-001 — ASP.NET Identity with EF Core stores configured | Cross-Epic |
| DEP-002 | SQL Server with Identity schema (AspNetRoles, AspNetUserRoles) | Infrastructure |

---

## .NET Implementation Notes

### Role Seeding (Startup)

```csharp
// In Program.cs or DatabaseSeeder.cs
using var scope = app.Services.CreateScope();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

string[] roles = ["Admin", "User"];
foreach (var role in roles)
{
    if (!await roleManager.RoleExistsAsync(role))
        await roleManager.CreateAsync(new IdentityRole(role));
}
```

### API Endpoint

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/v1/auth/roles/assign` | `Admin` role required | Assign a role to a user |

### Request Model

```csharp
public record AssignRoleRequest(string UserId, string Role);
```

### Default Role on Registration

Update the existing registration handler to add `await userManager.AddToRoleAsync(user, "User")` after successful user creation.

### Linked Artifacts

- Design: ADR-RBAC-001
- Tests: TC-RBAC-027 – TC-RBAC-034
- Implementation: —
