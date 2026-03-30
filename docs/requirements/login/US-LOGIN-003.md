# US-LOGIN-003: Account Lockout After Failed Attempts

**Type**: Functional
**Priority**: Critical
**Story Points**: 5
**Source**: EPIC-LOGIN / PRD Section 6 — OWASP A07 (Identification and Authentication Failures)
**Status**: Ready

---

## User Story

**As a** system administrator
**I want** user accounts to be temporarily locked after repeated failed login attempts
**So that** the system is protected against brute-force and credential-stuffing attacks

---

## Acceptance Criteria

- [ ] **AC-001**: Given a user who has failed login 5 times consecutively (configurable via `appsettings.json` key `Auth:MaxFailedAttempts`), when they attempt a 6th login, then the system returns HTTP 429 with message `"Account temporarily locked. Try again after {lockoutEnd} minutes"` and the account is locked for 15 minutes (configurable via `Auth:LockoutDurationMinutes`).
- [ ] **AC-002**: Given a locked-out user, when the lockout duration expires, then the user can log in successfully with valid credentials and the failed attempt counter resets to 0.
- [ ] **AC-003**: Given a user who has failed 3 times, when they successfully log in on the 4th attempt, then the failed attempt counter resets to 0.
- [ ] **AC-004**: Given a locked-out account, when the system returns HTTP 429, then the response includes a `Retry-After` header with the remaining lockout duration in seconds.
- [ ] **AC-005**: Given any failed login attempt (locked or not), when the response is returned, then the system does NOT reveal whether the lockout was triggered — the response message is consistent with the generic `"Invalid email or password"` until the lockout threshold is exceeded.
- [ ] **AC-006**: Given a rate limit of 20 login requests per minute per IP (configurable), when a client exceeds this rate, then the system returns HTTP 429 regardless of account lockout status.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Security | Lockout parameters are server-side configurable; never exposed to clients |
| Security | Rate limiting applied per-IP using ASP.NET Core Rate Limiting middleware |
| Security | Lockout events logged with severity Warning, include IP and User-Agent |
| Performance | Lockout check adds < 10ms to login flow |
| Resilience | If lockout state store (database) is temporarily unavailable, fail open with rate limiting as backup |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-LOGIN-001 (login endpoint exists) | Story |
| DEP-002 | ASP.NET Core Identity lockout configured | Technical |

---

## .NET Implementation Notes

### Configuration

```json
{
  "Auth": {
    "MaxFailedAttempts": 5,
    "LockoutDurationMinutes": 15,
    "RateLimitPerMinutePerIp": 20
  }
}
```

### Identity Configuration

```csharp
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
});
```

### Rate Limiting

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("login", limiter =>
    {
        limiter.PermitLimit = 20;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueLimit = 0;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
```

### Project Layer Mapping

| Layer | Artifacts |
|-------|----------|
| Application | Lockout check in `LoginCommandHandler` |
| Infrastructure | Identity lockout config, rate limiting middleware |
| API | Rate limiter policy on login endpoint group |

---

## Linked Artifacts

| Type | ID | Description |
|------|-----|------------|
| Design | ADR-LOGIN-002 | Lockout strategy and rate limiting approach |
| Tests | TC-LOGIN-012 | 5 failures triggers lockout |
| Tests | TC-LOGIN-013 | Lockout expires after configured duration |
| Tests | TC-LOGIN-014 | Successful login resets counter |
| Tests | TC-LOGIN-015 | Retry-After header present on lockout |
| Tests | TC-LOGIN-016 | Rate limit exceeded returns 429 |
| Tests | TC-LOGIN-017 | Locked account returns consistent error sub-threshold |

---

## Definition of Ready Checklist

- [x] Has unique ID (US-LOGIN-003)
- [x] Has user story in standard format
- [x] Has ≥ 3 testable acceptance criteria (6 provided)
- [x] Each AC is automatable as a test case
- [x] Priority assigned (Critical)
- [x] Dependencies identified (2)
- [x] NFRs specified (5)
- [x] Story points estimated (5)
- [x] Linked test cases identified (6)
- [ ] Reviewed by Product Owner
