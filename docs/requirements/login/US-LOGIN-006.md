# US-LOGIN-006: Login Audit Logging

**Type**: Functional
**Priority**: High
**Story Points**: 5
**Source**: EPIC-LOGIN / PRD Section 6 — OWASP A09 (Security Logging and Monitoring Failures)
**Status**: Ready

---

## User Story

**As a** system administrator
**I want** all authentication events (successful logins, failed attempts, lockouts, password resets) to be logged with structured audit data
**So that** I can monitor for suspicious activity, investigate incidents, and meet compliance requirements

---

## Acceptance Criteria

- [ ] **AC-001**: Given a successful login, when the JWT is issued, then an audit log entry is written with: `EventType: "LoginSuccess"`, `UserId`, `Email` (masked: `r***@example.com`), `Timestamp` (UTC ISO 8601), `IpAddress`, `UserAgent`, `CorrelationId`.
- [ ] **AC-002**: Given a failed login attempt, when the response is returned, then an audit log entry is written with: `EventType: "LoginFailure"`, `AttemptedEmail` (masked), `Timestamp`, `IpAddress`, `UserAgent`, `FailureReason` (one of: `InvalidCredentials`, `AccountLocked`, `AccountDisabled`), `CorrelationId`.
- [ ] **AC-003**: Given an account lockout event, when the lockout is triggered, then an audit log entry is written with: `EventType: "AccountLockout"`, `UserId`, `Email` (masked), `LockoutEnd` (UTC), `FailedAttemptCount`, `IpAddress`, `CorrelationId`, and severity is `Warning`.
- [ ] **AC-004**: Given a password reset completion, when the password is changed, then an audit log entry is written with: `EventType: "PasswordReset"`, `UserId`, `Timestamp`, `IpAddress`, `CorrelationId`.
- [ ] **AC-005**: Given any audit log entry, when it is written, then it NEVER contains: plaintext passwords, full email addresses, JWT tokens, refresh tokens, or reset tokens.
- [ ] **AC-006**: Given the audit log sink (e.g., Application Insights) is temporarily unavailable, when an auth event occurs, then the event is logged to a fallback local structured log file and the login flow is NOT blocked (fail open for logging).

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Security | PII masking applied to email addresses (show first char + domain only) |
| Security | Audit logs are append-only; no update or delete capability |
| Security | Logs stored with 90-day retention minimum (configurable) |
| Performance | Logging is asynchronous; adds < 5ms to request latency |
| Compliance | Log format compatible with SIEM ingestion (JSON structured) |
| Reliability | Dual-write: Application Insights + local fallback |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-LOGIN-001 (login events to capture) | Story |
| DEP-002 | US-LOGIN-003 (lockout events to capture) | Story |
| DEP-003 | US-LOGIN-005 (password reset events to capture) | Story |
| DEP-004 | Serilog + Application Insights sink configured | Infrastructure |

---

## .NET Implementation Notes

### Audit Event Model

```csharp
public record AuthAuditEvent
{
    public required string EventType { get; init; }
    public string? UserId { get; init; }
    public string? MaskedEmail { get; init; }
    public required DateTime Timestamp { get; init; }
    public required string IpAddress { get; init; }
    public required string UserAgent { get; init; }
    public string? FailureReason { get; init; }
    public required string CorrelationId { get; init; }
}
```

### Email Masking Utility

```csharp
public static string MaskEmail(string email)
{
    var parts = email.Split('@');
    if (parts.Length != 2) return "***";
    return $"{parts[0][0]}***@{parts[1]}";
}
```

### Serilog Configuration

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.WithCorrelationId()
    .WriteTo.ApplicationInsights(TelemetryConverter.Traces)
    .WriteTo.File("logs/auth-audit-.json",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 90,
        formatter: new JsonFormatter())
    .CreateLogger();
```

### Project Layer Mapping

| Layer | Artifacts |
|-------|----------|
| Domain | `AuthAuditEvent` record |
| Application | `IAuditLogger` interface, audit calls in command handlers |
| Infrastructure | `SerilogAuditLogger`, Application Insights config |
| API | Correlation ID middleware |

---

## Linked Artifacts

| Type | ID | Description |
|------|-----|------------|
| Design | ADR-LOGIN-004 | Audit logging strategy and sink selection |
| Tests | TC-LOGIN-031 | Successful login writes audit entry |
| Tests | TC-LOGIN-032 | Failed login writes audit entry |
| Tests | TC-LOGIN-033 | Lockout writes warning-level audit entry |
| Tests | TC-LOGIN-034 | Password reset writes audit entry |
| Tests | TC-LOGIN-035 | Audit entries never contain plaintext secrets |
| Tests | TC-LOGIN-036 | Logging failure does not block login flow |

---

## Definition of Ready Checklist

- [x] Has unique ID (US-LOGIN-006)
- [x] Has user story in standard format
- [x] Has ≥ 3 testable acceptance criteria (6 provided)
- [x] Each AC is automatable as a test case
- [x] Priority assigned (High)
- [x] Dependencies identified (4)
- [x] NFRs specified (6)
- [x] Story points estimated (5)
- [x] Linked test cases identified (6)
- [ ] Reviewed by Product Owner
