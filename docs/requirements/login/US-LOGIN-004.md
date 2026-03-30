# US-LOGIN-004: Remember Me / Persistent Session

**Type**: Functional
**Priority**: Medium
**Story Points**: 3
**Source**: EPIC-LOGIN / PRD Section 6
**Status**: Ready

---

## User Story

**As a** registered user
**I want to** stay logged in across browser sessions when I choose "Remember me"
**So that** I don't have to re-enter my credentials every time I return to the application

---

## Acceptance Criteria

- [ ] **AC-001**: Given the user checks "Remember me" during login (sends `rememberMe: true` in the request body), when the login succeeds, then the refresh token is issued with a 30-day expiry (instead of the default 7-day expiry).
- [ ] **AC-002**: Given the user does NOT check "Remember me" (sends `rememberMe: false` or omits the field), when the login succeeds, then the refresh token is issued with the default 7-day expiry.
- [ ] **AC-003**: Given a persistent refresh token (30-day), when the user explicitly logs out via `POST /api/v1/auth/revoke`, then the refresh token is immediately revoked and cannot be reused.
- [ ] **AC-004**: Given a persistent refresh token, when it is used to refresh the access token, then a new refresh token is issued with the remaining lifetime of the original (sliding expiration is NOT applied — the absolute expiry date is preserved).
- [ ] **AC-005**: Given a user with active refresh tokens on multiple devices, when they revoke one token, then only that specific token is invalidated; other device sessions remain active.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Security | Refresh token stored as HttpOnly, Secure, SameSite=Strict cookie (if cookie-based) or securely in client storage |
| Security | Refresh token value stored as SHA-256 hash in database |
| Security | Maximum 5 concurrent refresh tokens per user (oldest revoked on overflow) |
| Performance | Refresh endpoint responds < 200ms (p95) |
| Privacy | "Remember me" preference is not stored server-side; it only affects token lifetime |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-LOGIN-001 (tokens and refresh flow exist) | Story |

---

## .NET Implementation Notes

### Extended Request Model

```csharp
public record LoginRequest(string Email, string Password, bool RememberMe = false);
```

### Refresh Token Lifetime Logic

```csharp
var refreshTokenExpiry = request.RememberMe
    ? TimeSpan.FromDays(30)
    : TimeSpan.FromDays(7);
```

### Configuration

```json
{
  "Auth": {
    "RefreshTokenExpiryDays": 7,
    "PersistentRefreshTokenExpiryDays": 30,
    "MaxRefreshTokensPerUser": 5
  }
}
```

### Project Layer Mapping

| Layer | Artifacts |
|-------|----------|
| Application | `LoginCommand` updated with `RememberMe`, token expiry logic |
| Infrastructure | `RefreshTokenRepository` — per-user token limit enforcement |

---

## Linked Artifacts

| Type | ID | Description |
|------|-----|------------|
| Tests | TC-LOGIN-018 | Remember me = true issues 30-day refresh token |
| Tests | TC-LOGIN-019 | Remember me = false issues 7-day refresh token |
| Tests | TC-LOGIN-020 | Explicit logout revokes persistent token |
| Tests | TC-LOGIN-021 | Refresh preserves absolute expiry |
| Tests | TC-LOGIN-022 | Max 5 concurrent tokens enforced |

---

## Definition of Ready Checklist

- [x] Has unique ID (US-LOGIN-004)
- [x] Has user story in standard format
- [x] Has ≥ 3 testable acceptance criteria (5 provided)
- [x] Each AC is automatable as a test case
- [x] Priority assigned (Medium)
- [x] Dependencies identified (1)
- [x] NFRs specified (5)
- [x] Story points estimated (3)
- [x] Linked test cases identified (5)
- [ ] Reviewed by Product Owner
