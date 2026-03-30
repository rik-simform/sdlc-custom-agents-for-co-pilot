# US-LOGIN-007: Multi-Factor Authentication (TOTP)

**Type**: Functional
**Priority**: Low (Future Sprint)
**Story Points**: 13
**Source**: EPIC-LOGIN / PRD Section 6 — OWASP A07
**Status**: Draft — Future Implementation

---

## User Story

**As a** security-conscious user
**I want to** enable time-based one-time password (TOTP) as a second authentication factor
**So that** my account is protected even if my password is compromised

---

## Acceptance Criteria

- [ ] **AC-001**: Given a user with MFA disabled, when they enable MFA via `POST /api/v1/auth/mfa/setup`, then the system generates a TOTP secret, returns a QR code URI (otpauth://), and provides 10 single-use backup codes.
- [ ] **AC-002**: Given a user completing MFA setup, when they submit a valid TOTP code to `POST /api/v1/auth/mfa/verify-setup`, then MFA is permanently enabled on the account and the TOTP secret is stored encrypted.
- [ ] **AC-003**: Given a user with MFA enabled who has entered valid email/password, when they are prompted for TOTP, then they must submit a valid 6-digit code to `POST /api/v1/auth/mfa/validate` within 90 seconds to complete login.
- [ ] **AC-004**: Given a user entering an invalid or expired TOTP code, when they submit, then the system returns HTTP 401 `"Invalid verification code"` and the failed MFA attempt counts toward lockout.
- [ ] **AC-005**: Given a user who has lost their authenticator app, when they submit a valid backup code to the MFA validate endpoint, then login succeeds and the used backup code is permanently invalidated.
- [ ] **AC-006**: Given a user with MFA enabled, when they disable MFA via `POST /api/v1/auth/mfa/disable` (requires current password confirmation), then TOTP is removed from the account and an audit event is logged.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Security | TOTP secret encrypted at rest using Data Protection API |
| Security | Backup codes are single-use, stored as hashes |
| Security | TOTP window tolerance: ±1 step (30-second intervals) |
| Security | MFA disable requires password re-confirmation |
| Performance | TOTP validation < 100ms |
| Standards | RFC 6238 (TOTP) compliant |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-LOGIN-001 (two-step login flow) | Story |
| DEP-002 | US-LOGIN-006 (MFA events must be audited) | Story |

---

## .NET Implementation Notes

### API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/v1/auth/mfa/setup` | Generate TOTP secret + QR URI |
| POST | `/api/v1/auth/mfa/verify-setup` | Confirm setup with first TOTP code |
| POST | `/api/v1/auth/mfa/validate` | Submit TOTP/backup code during login |
| POST | `/api/v1/auth/mfa/disable` | Disable MFA (requires password) |

### Key Libraries

- `Microsoft.AspNetCore.Identity` (built-in TOTP support via `UserManager.VerifyTwoFactorTokenAsync`)
- QR code generation: `QRCoder` NuGet package

### Two-Step Login Flow

1. `POST /api/v1/auth/login` → returns `{ requiresMfa: true, mfaToken: "..." }` (partial auth token)
2. `POST /api/v1/auth/mfa/validate` with `{ mfaToken, code }` → returns full JWT + refresh token

### Project Layer Mapping

| Layer | Artifacts |
|-------|----------|
| Domain | `MfaBackupCode` entity |
| Application | `SetupMfaCommand`, `ValidateMfaCommand`, `DisableMfaCommand` |
| Infrastructure | Identity TOTP provider, Data Protection integration |
| API | MFA endpoint group |

---

## Linked Artifacts

| Type | ID | Description |
|------|-----|------------|
| Design | ADR-LOGIN-005 | MFA strategy: TOTP vs push vs SMS |
| Tests | TC-LOGIN-037 | MFA setup returns QR URI and backup codes |
| Tests | TC-LOGIN-038 | Valid TOTP code completes login |
| Tests | TC-LOGIN-039 | Invalid TOTP code returns 401 |
| Tests | TC-LOGIN-040 | Backup code works and is single-use |
| Tests | TC-LOGIN-041 | MFA disable requires password |
| Tests | TC-LOGIN-042 | MFA events appear in audit log |

---

## Definition of Ready Checklist

- [x] Has unique ID (US-LOGIN-007)
- [x] Has user story in standard format
- [x] Has ≥ 3 testable acceptance criteria (6 provided)
- [x] Each AC is automatable as a test case
- [x] Priority assigned (Low — Future)
- [x] Dependencies identified (2)
- [x] NFRs specified (6)
- [x] Story points estimated (13)
- [x] Linked test cases identified (6)
- [ ] Reviewed by Product Owner
