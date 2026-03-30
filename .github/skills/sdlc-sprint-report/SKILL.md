---
name: sdlc-sprint-report
description: 'Generate sprint reports with velocity, burndown, and quality metrics from GitHub Projects and Issues data for .NET projects.'
---

# SDLC Sprint Report Generator

## Primary Directive

Generate comprehensive sprint reports by collecting data from GitHub Issues, PRs, and project boards. Reports include velocity, quality metrics, risks, and retrospective inputs.

## Data Collection

### From GitHub Issues
- Stories completed (closed issues with `story` label in sprint milestone)
- Story points (from issue body or custom field)
- Bugs found and resolved
- Blockers (issues with `blocked` label)

### From GitHub PRs
- PRs merged during sprint
- Review turnaround time
- CI pass/fail rate

### From GitHub Actions
- Build success rate
- Deployment frequency
- Test pass rate

## Report Template

```markdown
# Sprint {N} Report

**Sprint Period**: {Start} to {End}
**Sprint Goal**: {Goal from sprint planning}
**Status**: ✅ Achieved | ⚠️ Partially | ❌ Not Achieved

---

## Velocity

| Metric | Sprint {N} | Sprint {N-1} | Sprint {N-2} | 3-Sprint Avg |
|--------|-----------|-------------|-------------|-------------|
| Planned SP | {N} | {N} | {N} | {N} |
| Completed SP | {N} | {N} | {N} | {N} |
| Completion % | {N}% | {N}% | {N}% | {N}% |

## Quality

| Metric | Value | Target | Status |
|--------|-------|--------|--------|
| Code Coverage | {N}% | 80% | ✅/❌ |
| Bugs Found | {N} | — | — |
| Bugs Resolved | {N} | — | — |
| Bug Escape Rate | {N}% | < 5% | ✅/❌ |
| Build Success Rate | {N}% | > 95% | ✅/❌ |
| PR Review Time (avg) | {hours} | < 24h | ✅/❌ |

## Completed Work

| Issue | Title | SP | Type | Assignee |
|-------|-------|----|------|----------|
| #{N} | {Title} | {SP} | Story | @{user} |

## Carried Over

| Issue | Title | SP | Reason |
|-------|-------|----|--------|
| #{N} | {Title} | {SP} | {Reason} |

## Blockers & Risks

| Issue | Description | Impact | Mitigation | Status |
|-------|-------------|--------|------------|--------|
| #{N} | {Description} | {H/M/L} | {Plan} | {Status} |

## Retrospective Inputs

### What Went Well
- {Auto-detected: sprint goal met, high coverage, fast reviews}

### What Could Improve
- {Auto-detected: carried over stories, slow reviews, build failures}

### Action Items
| Action | Owner | Due | Status |
|--------|-------|-----|--------|
| {Action} | @{user} | {Date} | New |
```

## Metrics Calculation

| Metric | Formula |
|--------|---------|
| Velocity | Sum of SP on completed stories |
| Completion Rate | Completed SP / Planned SP × 100 |
| Bug Escape Rate | Bugs found in prod / Total stories × 100 |
| Cycle Time | Avg days from In Progress to Done |
| Lead Time | Avg days from Created to Done |
| PR Review Time | Avg hours from PR opened to first review |

## Output Location

Save to: `docs/project-management/sprints/sprint-{N}.md`
