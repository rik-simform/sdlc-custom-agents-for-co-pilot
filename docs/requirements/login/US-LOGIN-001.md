# US-LOGIN-001: Basic Email/Password Login

**Type**: Functional
**Priority**: Critical
**Story Points**: 5
**Source**: EPIC-LOGIN / PRD Section 6
**Status**: Ready

---

## User Story

**As a** registered user
**I want to** log in with my email address and password
**So that** I can access my account and use the application securely

---

## Acceptance Criteria

- [ ] **AC-001**: Given a registered user with valid credentials, when they submit email and password to `POST /api/v1/auth/login`, then the system returns HTTP 200 with a JWT access token (expires in 15 minutes) and an opaque refresh token (expires in 7 days).
- [ ] **AC-002**: Given a user with an incorrect password, when they submit login credentials, then the system returns HTTP 401 with a generic error message "Invalid email or password" (no credential enumeration).
- [ ] **AC-003**: Given a user with a non-existent email, when they submit login credentials, then the system returns HTTP 401 with the same generic message "Invalid email or password" and the response time is indistinguishable from an invalid-password response (timing attack prevention).
- [ ] **AC-004**: Given a valid login, when the JWT is issued, then it contains claims: `sub` (user ID), `email`, `roles`, `iat`, `exp`, and is signed with the configured signing key.
- [ ] **AC-005**: Given a valid refresh token, when the user calls `POST /api/v1/auth/refresh`, then a new JWT access token is issued and the old refresh token is rotated (one-time use).

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | Login endpoint responds < 500ms (p95) under 100 concurrent users |
| Security | Passwords never logged or returned in any response |
| Security | JWT signed with HMAC-SHA256 or RSA-256; signing key from Key Vault |
| Security | Refresh tokens stored as SHA-256 hash in database, not plaintext |
| Security | HTTPS enforced; HSTS header set |
| Scalability | Stateless JWT validation; no server-side session store required |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | ASP.NET Core Identity configured with EF Core stores | Technical |
| DEP-002 | SQL Database provisioned with Identity schema | Infrastructure |
| DEP-003 | Azure Key Vault for JWT signing key | Infrastructure |

---

## .NET Implementation Notes

### API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/v1/auth/login` | Authenticate and issue tokens |
| POST | `/api/v1/auth/refresh` | Refresh access token |
| POST | `/api/v1/auth/revoke` | Revoke refresh token (logout) |

### Key Entities

- `ApplicationUser : IdentityUser` — extended user entity
- `RefreshToken` — entity: `Id`, `UserId`, `TokenHash`, `ExpiresAt`, `CreatedAt`, `DeviceInfo`, `IsRevoked`

### Request/Response Models

```csharp
// Request
public record LoginRequest(string Email, string Password);

// Response
public record LoginResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);
```

### Middleware / Services

- `JwtTokenService` — generates and validates JWT tokens
- `RefreshTokenService` — creates, validates, and rotates refresh tokens
- `AddAuthentication().AddJwtBearer()` — JWT middleware configuration

### Project Layer Mapping (Clean Architecture)

| Layer | Artifacts |
|-------|----------|
| Domain | `ApplicationUser`, `RefreshToken` entity |
| Application | `LoginCommand`, `RefreshTokenCommand`, `ITokenService` |
| Infrastructure | `JwtTokenService`, `RefreshTokenRepository`, Identity config |
| API | `AuthEndpoints` (Minimal API group) |

---

## Linked Artifacts

| Type | ID | Description |
|------|-----|------------|
| Design | ADR-LOGIN-001 | JWT vs session-based authentication decision |
| Tests | TC-LOGIN-001 | Valid credentials return JWT |
| Tests | TC-LOGIN-002 | Invalid password returns 401 |
| Tests | TC-LOGIN-003 | Non-existent email returns 401 |
| Tests | TC-LOGIN-004 | Refresh token rotation works |
| Tests | TC-LOGIN-005 | Revoked refresh token rejected |

---

## Definition of Ready Checklist

- [x] Has unique ID (US-LOGIN-001)
- [x] Has user story in standard format
- [x] Has ≥ 3 testable acceptance criteria (5 provided)
- [x] Each AC is automatable as a test case
- [x] Priority assigned (Critical)
- [x] Dependencies identified (3)
- [x] NFRs specified (6)
- [x] Story points estimated (5)
- [x] Linked test cases identified (5)
- [ ] Reviewed by Product Owner
