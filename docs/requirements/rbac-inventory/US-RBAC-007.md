# US-RBAC-007: Unauthorized and Forbidden Responses

**Type**: Functional
**Priority**: High
**Story Points**: 3
**Source**: EPIC-RBAC / PRD Section 6
**Status**: Ready

---

## User Story

**As a** developer integrating with the API
**I want to** receive clear, consistent HTTP 401 and 403 responses when access is denied
**So that** my client application can distinguish between "not logged in" and "insufficient permissions" and present appropriate UI feedback

---

## Acceptance Criteria

- [ ] **AC-001**: Given a request to any inventory endpoint without a JWT (or with an expired JWT), when the request is processed, then the system returns HTTP 401 with a ProblemDetails response body containing `title: "Unauthorized"` and `detail: "Authentication is required to access this resource."`.
- [ ] **AC-002**: Given a request from an authenticated `User` to a write endpoint (`POST`, `PUT`, `DELETE` on `/api/v1/inventory`), when the request is processed, then the system returns HTTP 403 with a ProblemDetails response body containing `title: "Forbidden"` and `detail: "You do not have permission to perform this action."`.
- [ ] **AC-003**: Given a 401 or 403 response, then the `Content-Type` header is `application/problem+json`.
- [ ] **AC-004**: Given a 401 or 403 response, then no stack trace, internal exception details, or server-side path information is exposed in the response body.
- [ ] **AC-005**: Given a request to `POST /api/v1/auth/roles/assign` from a non-Admin user, then the system returns HTTP 403 with the same ProblemDetails format.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Security | Error responses never leak internal implementation details |
| Security | Timing of 401/403 responses is consistent — no timing-based role enumeration |
| Standards | All error responses follow RFC 9457 (Problem Details for HTTP APIs) |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-LOGIN-001 — JWT authentication middleware configured | Cross-Epic |
| DEP-002 | US-RBAC-001, 002, 003 — Write endpoints restricted to Admin | Story |

---

## .NET Implementation Notes

### Custom Problem Details Handler

```csharp
// In Program.cs — configure authorization failure responses
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationResultHandler>();
```

```csharp
public class CustomAuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
{
    public async Task HandleAsync(RequestDelegate next, HttpContext context,
        AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Challenged)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = "Authentication is required to access this resource.",
                Status = 401
            });
            return;
        }

        if (authorizeResult.Forbidden)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "Forbidden",
                Detail = "You do not have permission to perform this action.",
                Status = 403
            });
            return;
        }

        await next(context);
    }
}
```

### Linked Artifacts

- Design: ADR-RBAC-001
- Tests: TC-RBAC-035 – TC-RBAC-039
- Implementation: —
