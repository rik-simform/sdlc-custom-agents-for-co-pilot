# US-LOGIN-002: Login Input Validation

**Type**: Functional
**Priority**: Critical
**Story Points**: 3
**Source**: EPIC-LOGIN / PRD Section 6
**Status**: Ready

---

## User Story

**As a** registered user
**I want to** receive clear validation feedback when I submit invalid login input
**So that** I can correct my input and log in successfully

---

## Acceptance Criteria

- [ ] **AC-001**: Given an empty email field, when the user submits the login form, then the system returns HTTP 400 with a validation error `"Email is required"` and the request never reaches the Identity layer.
- [ ] **AC-002**: Given an email that is not a valid email format (e.g., `"notanemail"`), when the user submits, then the system returns HTTP 400 with `"Email must be a valid email address"`.
- [ ] **AC-003**: Given an empty password field, when the user submits, then the system returns HTTP 400 with `"Password is required"`.
- [ ] **AC-004**: Given an email longer than 256 characters, when the user submits, then the system returns HTTP 400 with `"Email must not exceed 256 characters"`.
- [ ] **AC-005**: Given a password longer than 128 characters, when the user submits, then the system returns HTTP 400 with `"Password must not exceed 128 characters"` (DoS prevention via hashing cost).
- [ ] **AC-006**: Given a request body with extraneous fields (e.g., `"role": "admin"`), when the user submits, then the extraneous fields are ignored and do not affect processing (mass assignment prevention).

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Security | Server-side validation is canonical; client-side validation is convenience only |
| Security | Validation errors do not reveal whether an email exists in the system |
| Security | Input is sanitized against XSS (HTML-encoded in any error response) |
| Performance | Validation rejects invalid input before any database or hash operations |
| Standards | Use FluentValidation with ProblemDetails (RFC 7807) response format |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-LOGIN-001 (login endpoint exists) | Story |

---

## .NET Implementation Notes

### FluentValidation Validator

```csharp
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters")
            .EmailAddress().WithMessage("Email must be a valid email address");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MaximumLength(128).WithMessage("Password must not exceed 128 characters");
    }
}
```

### Middleware

- `FluentValidation.AspNetCore` auto-validation via endpoint filter
- `ProblemDetails` response format configured via `builder.Services.AddProblemDetails()`

### Project Layer Mapping

| Layer | Artifacts |
|-------|----------|
| Application | `LoginRequestValidator` |
| API | FluentValidation endpoint filter, ProblemDetails config |

---

## Linked Artifacts

| Type | ID | Description |
|------|-----|------------|
| Tests | TC-LOGIN-006 | Empty email returns 400 |
| Tests | TC-LOGIN-007 | Invalid email format returns 400 |
| Tests | TC-LOGIN-008 | Empty password returns 400 |
| Tests | TC-LOGIN-009 | Oversized email returns 400 |
| Tests | TC-LOGIN-010 | Oversized password returns 400 |
| Tests | TC-LOGIN-011 | Extraneous fields ignored |

---

## Definition of Ready Checklist

- [x] Has unique ID (US-LOGIN-002)
- [x] Has user story in standard format
- [x] Has ≥ 3 testable acceptance criteria (6 provided)
- [x] Each AC is automatable as a test case
- [x] Priority assigned (Critical)
- [x] Dependencies identified (1)
- [x] NFRs specified (5)
- [x] Story points estimated (3)
- [x] Linked test cases identified (6)
- [ ] Reviewed by Product Owner
