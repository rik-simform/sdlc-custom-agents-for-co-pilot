# US-LOGIN-008: Social Login (External Providers)

**Type**: Functional
**Priority**: Low (Future Sprint)
**Story Points**: 8
**Source**: EPIC-LOGIN / PRD Section 6
**Status**: Draft — Future Implementation

---

## User Story

**As a** registered user
**I want to** log in using my existing Google, Microsoft, or GitHub account
**So that** I can access the application without managing a separate password

---

## Acceptance Criteria

- [ ] **AC-001**: Given a user who has linked their Google account, when they initiate login via `GET /api/v1/auth/external/google`, then the system redirects to Google OAuth 2.0 consent screen and, upon successful authorization, returns JWT + refresh token.
- [ ] **AC-002**: Given a user who has linked their Microsoft account, when they initiate login via `GET /api/v1/auth/external/microsoft`, then the same flow completes with Microsoft identity platform.
- [ ] **AC-003**: Given a user who has linked their GitHub account, when they initiate login via `GET /api/v1/auth/external/github`, then the same flow completes with GitHub OAuth.
- [ ] **AC-004**: Given an external login where the external email matches an existing local account, when the OAuth callback is received, then the external provider is linked to the existing account (not a duplicate account created).
- [ ] **AC-005**: Given an external login where no matching local account exists, when the OAuth callback is received, then the system returns an error directing the user to register first (auto-registration is out of scope for this epic).
- [ ] **AC-006**: Given any external provider login, when the login succeeds, then an audit event is logged with `EventType: "ExternalLoginSuccess"` and `Provider: "{providerName}"`.
- [ ] **AC-007**: Given a user with a linked external provider, when they unlink it via `POST /api/v1/auth/external/unlink` (requires at least one other auth method active), then the provider is removed from the account.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Security | OAuth client secrets stored in Azure Key Vault, never in config files |
| Security | State parameter validated to prevent CSRF on OAuth callbacks |
| Security | External tokens (from provider) are NOT stored; only the provider + external user ID mapping |
| Security | PKCE (Proof Key for Code Exchange) required for all OAuth flows |
| Performance | External login redirect completes in < 2 seconds (excluding provider latency) |
| Resilience | If an external provider is unavailable, the login page shows degraded state with email/password fallback |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-LOGIN-001 (JWT issuance after external auth) | Story |
| DEP-002 | US-LOGIN-006 (external login audit events) | Story |
| DEP-003 | OAuth app registration with Google, Microsoft, GitHub | Infrastructure |
| DEP-004 | Azure Key Vault for client secrets | Infrastructure |

---

## .NET Implementation Notes

### API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/v1/auth/external/{provider}` | Initiate OAuth redirect |
| GET | `/api/v1/auth/external/callback` | OAuth callback handler |
| POST | `/api/v1/auth/external/unlink` | Unlink external provider |

### Authentication Configuration

```csharp
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = config["Auth:Google:ClientId"]!;
        options.ClientSecret = config["Auth:Google:ClientSecret"]!;
    })
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = config["Auth:Microsoft:ClientId"]!;
        options.ClientSecret = config["Auth:Microsoft:ClientSecret"]!;
    })
    .AddGitHub(options =>
    {
        options.ClientId = config["Auth:GitHub:ClientId"]!;
        options.ClientSecret = config["Auth:GitHub:ClientSecret"]!;
    });
```

### Key Entities

- `AspNetUserLogins` — built-in Identity table for external provider mappings (`LoginProvider`, `ProviderKey`, `UserId`)

### Project Layer Mapping

| Layer | Artifacts |
|-------|----------|
| Application | `ExternalLoginCommand`, `UnlinkProviderCommand` |
| Infrastructure | OAuth provider configuration, external login callback handler |
| API | External auth endpoint group |

---

## Linked Artifacts

| Type | ID | Description |
|------|-----|------------|
| Design | ADR-LOGIN-006 | External provider selection and PKCE strategy |
| Tests | TC-LOGIN-043 | Google OAuth flow completes login |
| Tests | TC-LOGIN-044 | Microsoft OAuth flow completes login |
| Tests | TC-LOGIN-045 | GitHub OAuth flow completes login |
| Tests | TC-LOGIN-046 | Matching email links to existing account |
| Tests | TC-LOGIN-047 | No matching account returns registration prompt |
| Tests | TC-LOGIN-048 | External login audit event logged |
| Tests | TC-LOGIN-049 | Unlink requires active alternative auth method |

---

## Definition of Ready Checklist

- [x] Has unique ID (US-LOGIN-008)
- [x] Has user story in standard format
- [x] Has ≥ 3 testable acceptance criteria (7 provided)
- [x] Each AC is automatable as a test case
- [x] Priority assigned (Low — Future)
- [x] Dependencies identified (4)
- [x] NFRs specified (6)
- [x] Story points estimated (8)
- [x] Linked test cases identified (7)
- [ ] Reviewed by Product Owner
