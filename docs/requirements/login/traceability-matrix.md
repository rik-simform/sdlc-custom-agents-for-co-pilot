# Requirements Traceability Matrix — EPIC-LOGIN

Last Updated: 2026-03-26

---

## Forward Traceability (Requirement → Verification)

| Req ID | Title | Priority | Design | Implementation | Test Cases | Status |
|--------|-------|----------|--------|----------------|------------|--------|
| US-LOGIN-001 | Basic Email/Password Login | Critical | ADR-LOGIN-001 | — | TC-LOGIN-001 – TC-LOGIN-005 | ⏳ Pending |
| US-LOGIN-002 | Login Input Validation | Critical | — | — | TC-LOGIN-006 – TC-LOGIN-011 | ⏳ Pending |
| US-LOGIN-003 | Account Lockout | Critical | ADR-LOGIN-002 | — | TC-LOGIN-012 – TC-LOGIN-017 | ⏳ Pending |
| US-LOGIN-004 | Remember Me / Persistent Session | Medium | — | — | TC-LOGIN-018 – TC-LOGIN-022 | ⏳ Pending |
| US-LOGIN-005 | Forgot Password / Reset Flow | High | ADR-LOGIN-003 | — | TC-LOGIN-023 – TC-LOGIN-030 | ⏳ Pending |
| US-LOGIN-006 | Login Audit Logging | High | ADR-LOGIN-004 | — | TC-LOGIN-031 – TC-LOGIN-036 | ⏳ Pending |
| US-LOGIN-007 | Multi-Factor Authentication (TOTP) | Low | ADR-LOGIN-005 | — | TC-LOGIN-037 – TC-LOGIN-042 | 📋 Future |
| US-LOGIN-008 | Social Login (External Providers) | Low | ADR-LOGIN-006 | — | TC-LOGIN-043 – TC-LOGIN-049 | 📋 Future |

---

## Backward Traceability (Test → Requirement)

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-LOGIN-001 | Valid credentials return JWT | Integration | US-LOGIN-001/AC-001 | — | ⏳ Pending |
| TC-LOGIN-002 | Invalid password returns 401 | Integration | US-LOGIN-001/AC-002 | — | ⏳ Pending |
| TC-LOGIN-003 | Non-existent email returns 401 | Integration | US-LOGIN-001/AC-003 | — | ⏳ Pending |
| TC-LOGIN-004 | Refresh token rotation works | Integration | US-LOGIN-001/AC-005 | — | ⏳ Pending |
| TC-LOGIN-005 | Revoked refresh token rejected | Integration | US-LOGIN-001/AC-005 | — | ⏳ Pending |
| TC-LOGIN-006 | Empty email returns 400 | Unit | US-LOGIN-002/AC-001 | — | ⏳ Pending |
| TC-LOGIN-007 | Invalid email format returns 400 | Unit | US-LOGIN-002/AC-002 | — | ⏳ Pending |
| TC-LOGIN-008 | Empty password returns 400 | Unit | US-LOGIN-002/AC-003 | — | ⏳ Pending |
| TC-LOGIN-009 | Oversized email returns 400 | Unit | US-LOGIN-002/AC-004 | — | ⏳ Pending |
| TC-LOGIN-010 | Oversized password returns 400 | Unit | US-LOGIN-002/AC-005 | — | ⏳ Pending |
| TC-LOGIN-011 | Extraneous fields ignored | Unit | US-LOGIN-002/AC-006 | — | ⏳ Pending |
| TC-LOGIN-012 | 5 failures triggers lockout | Integration | US-LOGIN-003/AC-001 | — | ⏳ Pending |
| TC-LOGIN-013 | Lockout expires after configured duration | Integration | US-LOGIN-003/AC-002 | — | ⏳ Pending |
| TC-LOGIN-014 | Successful login resets counter | Integration | US-LOGIN-003/AC-003 | — | ⏳ Pending |
| TC-LOGIN-015 | Retry-After header present on lockout | Integration | US-LOGIN-003/AC-004 | — | ⏳ Pending |
| TC-LOGIN-016 | Rate limit exceeded returns 429 | Integration | US-LOGIN-003/AC-006 | — | ⏳ Pending |
| TC-LOGIN-017 | Locked account returns consistent error | Integration | US-LOGIN-003/AC-005 | — | ⏳ Pending |
| TC-LOGIN-018 | Remember me issues 30-day refresh token | Integration | US-LOGIN-004/AC-001 | — | ⏳ Pending |
| TC-LOGIN-019 | Default issues 7-day refresh token | Integration | US-LOGIN-004/AC-002 | — | ⏳ Pending |
| TC-LOGIN-020 | Explicit logout revokes persistent token | Integration | US-LOGIN-004/AC-003 | — | ⏳ Pending |
| TC-LOGIN-021 | Refresh preserves absolute expiry | Integration | US-LOGIN-004/AC-004 | — | ⏳ Pending |
| TC-LOGIN-022 | Max concurrent tokens enforced | Integration | US-LOGIN-004/AC-005 | — | ⏳ Pending |
| TC-LOGIN-023 | Valid email triggers reset email | Integration | US-LOGIN-005/AC-001 | — | ⏳ Pending |
| TC-LOGIN-024 | Non-existent email returns same response | Integration | US-LOGIN-005/AC-002 | — | ⏳ Pending |
| TC-LOGIN-025 | Valid token resets password | Integration | US-LOGIN-005/AC-003 | — | ⏳ Pending |
| TC-LOGIN-026 | Expired token rejected | Integration | US-LOGIN-005/AC-004 | — | ⏳ Pending |
| TC-LOGIN-027 | Used token rejected | Integration | US-LOGIN-005/AC-005 | — | ⏳ Pending |
| TC-LOGIN-028 | Weak password rejected | Unit | US-LOGIN-005/AC-006 | — | ⏳ Pending |
| TC-LOGIN-029 | Rate limiting on reset requests | Integration | US-LOGIN-005/AC-007 | — | ⏳ Pending |
| TC-LOGIN-030 | Password change revokes refresh tokens | Integration | US-LOGIN-005/AC-003 | — | ⏳ Pending |
| TC-LOGIN-031 | Successful login writes audit entry | Integration | US-LOGIN-006/AC-001 | — | ⏳ Pending |
| TC-LOGIN-032 | Failed login writes audit entry | Integration | US-LOGIN-006/AC-002 | — | ⏳ Pending |
| TC-LOGIN-033 | Lockout writes warning-level audit entry | Integration | US-LOGIN-006/AC-003 | — | ⏳ Pending |
| TC-LOGIN-034 | Password reset writes audit entry | Integration | US-LOGIN-006/AC-004 | — | ⏳ Pending |
| TC-LOGIN-035 | Audit entries never contain secrets | Unit | US-LOGIN-006/AC-005 | — | ⏳ Pending |
| TC-LOGIN-036 | Logging failure does not block login | Integration | US-LOGIN-006/AC-006 | — | ⏳ Pending |
| TC-LOGIN-037 | MFA setup returns QR URI and backup codes | Integration | US-LOGIN-007/AC-001 | — | 📋 Future |
| TC-LOGIN-038 | Valid TOTP code completes login | Integration | US-LOGIN-007/AC-003 | — | 📋 Future |
| TC-LOGIN-039 | Invalid TOTP code returns 401 | Integration | US-LOGIN-007/AC-004 | — | 📋 Future |
| TC-LOGIN-040 | Backup code works and is single-use | Integration | US-LOGIN-007/AC-005 | — | 📋 Future |
| TC-LOGIN-041 | MFA disable requires password | Integration | US-LOGIN-007/AC-006 | — | 📋 Future |
| TC-LOGIN-042 | MFA events appear in audit log | Integration | US-LOGIN-007/AC-006 | — | 📋 Future |
| TC-LOGIN-043 | Google OAuth flow completes login | E2E | US-LOGIN-008/AC-001 | — | 📋 Future |
| TC-LOGIN-044 | Microsoft OAuth flow completes login | E2E | US-LOGIN-008/AC-002 | — | 📋 Future |
| TC-LOGIN-045 | GitHub OAuth flow completes login | E2E | US-LOGIN-008/AC-003 | — | 📋 Future |
| TC-LOGIN-046 | Matching email links to existing account | Integration | US-LOGIN-008/AC-004 | — | 📋 Future |
| TC-LOGIN-047 | No matching account returns error | Integration | US-LOGIN-008/AC-005 | — | 📋 Future |
| TC-LOGIN-048 | External login audit event logged | Integration | US-LOGIN-008/AC-006 | — | 📋 Future |
| TC-LOGIN-049 | Unlink requires active auth method | Integration | US-LOGIN-008/AC-007 | — | 📋 Future |

---

## Coverage Summary

| Category | Total | Designed | Implemented | Tested | Verified |
|----------|-------|----------|-------------|--------|----------|
| Critical (US-001–003) | 3 | 2 (67%) | 0 (0%) | 0 (0%) | 0 (0%) |
| High (US-005–006) | 2 | 2 (100%) | 0 (0%) | 0 (0%) | 0 (0%) |
| Medium (US-004) | 1 | 0 (0%) | 0 (0%) | 0 (0%) | 0 (0%) |
| Low / Future (US-007–008) | 2 | 2 (100%) | 0 (0%) | 0 (0%) | 0 (0%) |
| **Total** | **8** | **6 (75%)** | **0 (0%)** | **0 (0%)** | **0 (0%)** |

### Test Coverage by Type

| Test Type | Count | Stories Covered |
|-----------|-------|----------------|
| Unit | 8 | US-LOGIN-002, US-LOGIN-005, US-LOGIN-006 |
| Integration | 34 | US-LOGIN-001 – US-LOGIN-008 |
| E2E | 3 | US-LOGIN-008 |
| Performance | 0 (planned) | NFR-LOGIN-PERF |
| **Total** | **49** | **8 stories** |

---

## Gaps

| Gap Type | Count | Details |
|----------|-------|---------|
| Unimplemented stories | 8 | All stories pending implementation |
| Missing ADRs | 2 | US-LOGIN-002, US-LOGIN-004 (no design decision needed) |
| Missing performance tests | 1 | NFR-LOGIN-PERF — benchmark tests to be created |
| Missing security scan | 1 | CodeQL scan to be configured in CI |

---

## Dependency Graph

```
US-LOGIN-001 (Core Login)
├── US-LOGIN-002 (Validation) ─── depends on ──▶ US-LOGIN-001
├── US-LOGIN-003 (Lockout) ───── depends on ──▶ US-LOGIN-001
├── US-LOGIN-004 (Remember Me) ─ depends on ──▶ US-LOGIN-001
├── US-LOGIN-005 (Password Reset) depends on ─▶ US-LOGIN-001
├── US-LOGIN-006 (Audit Logging)  depends on ─▶ US-LOGIN-001, US-LOGIN-003, US-LOGIN-005
├── US-LOGIN-007 (MFA) ────────── depends on ─▶ US-LOGIN-001, US-LOGIN-006
└── US-LOGIN-008 (Social Login) ─ depends on ─▶ US-LOGIN-001, US-LOGIN-006
```

---

## Quality Gate Checklist (Pre-Release)

- [ ] 100% of Critical/High stories have linked passing tests
- [ ] ≥ 80% of Medium stories have linked tests
- [ ] Zero orphaned tests (all tests trace to requirements)
- [ ] No broken links (all referenced PRs, ADRs exist)
- [ ] CodeQL reports zero critical/high findings
- [ ] Performance benchmarks met (p95 < 500ms)
- [ ] Audit logging verified in staging environment

---

*Matrix generated by SDLC Requirements Engineer Agent.
Auto-populated from user story documents in `docs/requirements/login/`.*
