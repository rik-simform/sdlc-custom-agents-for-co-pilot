# US-LOGIN-005: Forgot Password / Password Reset Flow

**Type**: Functional
**Priority**: High
**Story Points**: 8
**Source**: EPIC-LOGIN / PRD Section 6
**Status**: Ready

---

## User Story

**As a** registered user who has forgotten my password
**I want to** request a password reset link via email
**So that** I can regain access to my account securely

---

## Acceptance Criteria

- [ ] **AC-001**: Given a registered user, when they submit their email to `POST /api/v1/auth/forgot-password`, then the system sends a password reset email containing a one-time, time-bound reset token and returns HTTP 200 with a generic message `"If an account with that email exists, a reset link has been sent"` (anti-enumeration).
- [ ] **AC-002**: Given a non-existent email, when submitted to the forgot-password endpoint, then the system returns the same HTTP 200 response and message (no enumeration), and no email is sent.
- [ ] **AC-003**: Given a valid reset token, when the user submits a new password to `POST /api/v1/auth/reset-password` with `{ token, email, newPassword }`, then the password is updated successfully and all existing refresh tokens for that user are revoked.
- [ ] **AC-004**: Given a reset token that has expired (> 1 hour, configurable), when the user attempts to reset, then the system returns HTTP 400 with `"Reset token has expired. Please request a new one"`.
- [ ] **AC-005**: Given a reset token that has already been used, when the user attempts to use it again, then the system returns HTTP 400 with `"Reset token is invalid"`.
- [ ] **AC-006**: Given the new password does not meet the password policy (minimum 8 characters, at least one uppercase, one lowercase, one digit, one special character), when submitted, then the system returns HTTP 400 with specific validation errors.
- [ ] **AC-007**: Given the forgot-password endpoint, when more than 3 reset requests are made for the same email within 15 minutes, then subsequent requests are rate-limited and return HTTP 429.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Security | Reset token is cryptographically random, ≥ 256 bits |
| Security | Reset token expires after 1 hour (configurable) |
| Security | Reset token is single-use; invalidated after successful reset |
| Security | All existing sessions/refresh tokens revoked on password change |
| Security | Password reset email sent over TLS |
| Security | Reset link uses HTTPS URL with token as query parameter |
| Performance | Email sent within 5 seconds of request |
| Privacy | Email content does not reveal the user's name or account details beyond the reset link |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-LOGIN-001 (Identity and token infrastructure) | Story |
| DEP-002 | Email service integration (SendGrid / Azure Communication Services) | Infrastructure |

---

## .NET Implementation Notes

### API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/v1/auth/forgot-password` | Request password reset email |
| POST | `/api/v1/auth/reset-password` | Reset password with token |

### Request Models

```csharp
public record ForgotPasswordRequest(string Email);

public record ResetPasswordRequest(string Email, string Token, string NewPassword);
```

### Password Policy (Identity)

```csharp
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
});
```

### Token Generation

```csharp
var token = await _userManager.GeneratePasswordResetTokenAsync(user);
// Token is Base64Url-encoded and sent in the reset email link
```

### Project Layer Mapping

| Layer | Artifacts |
|-------|----------|
| Domain | Password policy rules (Identity options) |
| Application | `ForgotPasswordCommand`, `ResetPasswordCommand`, `IEmailService` |
| Infrastructure | `EmailService` (SendGrid), Identity token provider |
| API | Auth endpoint group extended |

---

## Linked Artifacts

| Type | ID | Description |
|------|-----|------------|
| Design | ADR-LOGIN-003 | Password reset token strategy |
| Tests | TC-LOGIN-023 | Valid email triggers reset email |
| Tests | TC-LOGIN-024 | Non-existent email returns same response |
| Tests | TC-LOGIN-025 | Valid token resets password |
| Tests | TC-LOGIN-026 | Expired token rejected |
| Tests | TC-LOGIN-027 | Used token rejected |
| Tests | TC-LOGIN-028 | Weak password rejected |
| Tests | TC-LOGIN-029 | Rate limiting on reset requests |
| Tests | TC-LOGIN-030 | Password change revokes all refresh tokens |

---

## Definition of Ready Checklist

- [x] Has unique ID (US-LOGIN-005)
- [x] Has user story in standard format
- [x] Has ≥ 3 testable acceptance criteria (7 provided)
- [x] Each AC is automatable as a test case
- [x] Priority assigned (High)
- [x] Dependencies identified (2)
- [x] NFRs specified (8)
- [x] Story points estimated (8)
- [x] Linked test cases identified (8)
- [ ] Reviewed by Product Owner
