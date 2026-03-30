---
name: sdlc-traceability
description: 'Generate and maintain Requirements Traceability Matrix (RTM) linking requirements through design, implementation, testing, and deployment for .NET projects.'
---

# SDLC Traceability Matrix

## Primary Directive

Maintain end-to-end traceability from requirements through deployment. The RTM links every requirement to its design decisions, implementation, tests, and deployment verification.

## Traceability Chain

```
Requirement (REQ/US) → Design (ADR/API) → Code (PR/Commit) → Test (TC) → Deploy (Release) → Monitor (Alert)
```

## RTM Template

Generate and update `docs/requirements/traceability-matrix.md`:

```markdown
# Requirements Traceability Matrix

Last Updated: {YYYY-MM-DD}

## Forward Traceability (Requirement → Verification)

| Req ID | Title | Priority | Design | Implementation | Test Cases | Status |
|--------|-------|----------|--------|----------------|------------|--------|
| US-001 | Azure AD Auth | High | ADR-001 | PR-42 | TC-001, TC-002, TC-003 | ✅ Verified |
| US-002 | Role-based Auth | High | ADR-001 | PR-45 | TC-004, TC-005 | ✅ Verified |
| US-003 | Session Mgmt | Medium | ADR-002 | PR-48 | TC-006 | 🔄 In Progress |
| REQ-NFR-001 | 200ms Response | High | ADR-003 | PR-50 | PERF-001 | ⏳ Pending |

## Backward Traceability (Test → Requirement)

| Test ID | Test Name | Type | Req IDs | Last Run | Status |
|---------|-----------|------|---------|----------|--------|
| TC-001 | Login with valid token | Integration | US-001/AC-001 | 2026-03-25 | ✅ Pass |
| TC-002 | Login with expired token | Integration | US-001/AC-002 | 2026-03-25 | ✅ Pass |
| TC-003 | Session timeout | E2E | US-001/AC-003 | 2026-03-25 | ✅ Pass |
| PERF-001 | API latency p95 | Performance | REQ-NFR-001 | 2026-03-24 | ✅ Pass |

## Coverage Summary

| Category | Total | Designed | Implemented | Tested | Verified |
|----------|-------|----------|-------------|--------|----------|
| Functional | 12 | 12 (100%) | 10 (83%) | 8 (67%) | 7 (58%) |
| Non-Functional | 5 | 5 (100%) | 3 (60%) | 2 (40%) | 2 (40%) |
| Security | 3 | 3 (100%) | 3 (100%) | 3 (100%) | 2 (67%) |
| **Total** | **20** | **20 (100%)** | **16 (80%)** | **13 (65%)** | **11 (55%)** |

## Gaps

| Gap Type | Count | Details |
|----------|-------|---------|
| Untested requirements | 4 | US-010, US-011, REQ-NFR-004, REQ-NFR-005 |
| Unimplemented requirements | 4 | US-009, US-010, US-011, US-012 |
| Unlinked tests | 2 | TC-015, TC-016 (orphaned tests) |
```

## Auto-Population Rules

| RTM Column | Data Source | Collection Method |
|-----------|------------|-------------------|
| Req ID | GitHub Issues (labeled `requirement` or `user-story`) | GitHub API |
| Design | ADR files + API specs in `docs/architecture/` | File scan |
| Implementation | PRs linked to issues (`Closes #N`) | GitHub API |
| Test Cases | Test methods with `[LinkedRequirement("US-xxx")]` attribute | Code scan |
| Status | CI test results + deployment verification | CI artifacts |

## Custom Test Attribute

For automated traceability, use this attribute in test code:

```csharp
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class LinkedRequirementAttribute(string requirementId) : Attribute
{
    public string RequirementId { get; } = requirementId;
}

// Usage:
[TestMethod]
[LinkedRequirement("US-001")]
[LinkedRequirement("AC-001")]
public async Task Login_WithValidToken_ReturnsJwt()
{
    // test implementation
}
```

## Validation Rules

- Every requirement MUST have at least one linked test case
- Every test case SHOULD link to at least one requirement
- Orphaned tests (no linked requirement) are flagged for review
- Requirements without implementation are flagged as "At Risk"
- Stale links (deleted PRs/tests) are automatically cleaned

## Quality Gate

RTM quality check before release:
- [ ] 100% of Critical/High requirements have linked passing tests
- [ ] ≥ 80% of Medium requirements have linked tests
- [ ] Zero orphaned tests (all tests trace to requirements)
- [ ] No broken links (all referenced PRs, tests, ADRs exist)
