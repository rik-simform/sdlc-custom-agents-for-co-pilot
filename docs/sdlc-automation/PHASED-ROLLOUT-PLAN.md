---
goal: Phased Rollout Plan for SDLC Automation Agents
version: 1.0
date_created: 2026-03-26
last_updated: 2026-03-26
owner: SDLC Automation Team
status: 'Planned'
tags: [sdlc, rollout, phases, mvp, automation]
---

# SDLC Automation — Phased Rollout Plan

![Status: Planned](https://img.shields.io/badge/status-Planned-blue)

---

## Assumptions & Scope

### Project Boundaries

| Boundary | Detail |
|----------|--------|
| **In Scope** | .NET 6+, .NET 8, ASP.NET Core, Entity Framework Core, Minimal APIs, Blazor, Web API, gRPC, Worker Services |
| **In Scope** | GitHub as source control and project management platform |
| **In Scope** | GitHub Copilot as the agent execution platform (VS Code + CLI) |
| **In Scope** | GitHub Actions as CI/CD pipeline |
| **In Scope** | Azure as primary cloud provider (App Service, Azure SQL, Key Vault, App Insights) |
| **Out of Scope** | .NET Framework (< .NET 6), Xamarin, MAUI (mobile), Unity |
| **Out of Scope** | Non-.NET languages/frameworks unless in polyglot integration points |
| **Out of Scope** | Custom LLM training or fine-tuning |
| **Out of Scope** | Third-party CI/CD platforms (Jenkins, TeamCity, CircleCI) — can be added later |
| **Out of Scope** | On-premises infrastructure provisioning |

### Target Audience

| Audience | How They Use It |
|----------|----------------|
| **Software Engineers** | Day-to-day coding, testing, deployment via Copilot agents |
| **Tech Leads / Architects** | Architecture review, design validation, ADR management |
| **Project Managers** | Sprint reporting, risk tracking, status dashboards |
| **QA Engineers** | Test planning, test generation, quality gate enforcement |
| **DevOps Engineers** | Pipeline management, environment provisioning, monitoring |
| **Auditors / Compliance** | Evidence collection, compliance matrix, audit trail |
| **New Team Members** | Onboarding via documentation agents and guided workflows |

### Key Assumptions

- ASM-001: Teams have GitHub Copilot Business or Enterprise licenses
- ASM-002: Projects use Git with GitHub as remote
- ASM-003: .NET SDK 6.0+ installed on developer machines
- ASM-004: Teams follow or are willing to adopt conventional commits
- ASM-005: GitHub Actions available for CI/CD automation
- ASM-006: Azure subscription available for cloud deployments
- ASM-007: Teams have basic familiarity with GitHub Copilot Chat

### Customization Points

Each agent and skill accepts project-specific overrides via:

1. **`.github/copilot-instructions.md`** — Project-level Copilot context
2. **`.github/sdlc-config.json`** — SDLC process configuration overrides
3. **`.editorconfig`** — Coding style enforcement
4. **`Directory.Build.props`** — .NET build configuration
5. **Environment variables** — Runtime configuration for CI/CD agents

---

## Phase 1: MVP — Core Development Loop (Weeks 1–4)

### Objective
Establish the foundational agents covering the inner development loop: implementation, code review, testing, and CI.

### Deliverables

| ID | Deliverable | Agent/Skill | Process Covered |
|----|-------------|-------------|-----------------|
| P1-01 | Copilot instructions for .NET projects | `copilot-instructions.md` | All |
| P1-02 | Implementation agent | `sdlc-implementer.agent.md` | PROC-003 |
| P1-03 | Code review agent | `sdlc-reviewer.agent.md` | PROC-004 |
| P1-04 | Testing agent | `sdlc-tester.agent.md` | PROC-005 |
| P1-05 | CI pipeline skill | `sdlc-ci-pipeline/SKILL.md` | PROC-006 |
| P1-06 | .NET best practices instructions | `dotnet.instructions.md` | PROC-003, PROC-004 |
| P1-07 | Git commit skill | `git-commit/SKILL.md` | PROC-008 |
| P1-08 | Security scanning instructions | `security.instructions.md` | PROC-009 |

### Success Criteria

- [ ] Agents can scaffold a new feature slice (controller + service + tests) from a user story
- [ ] Code review agent identifies ≥ 80% of common .NET anti-patterns
- [ ] Test agent generates meaningful unit and integration tests
- [ ] CI pipeline auto-generated and runs on first push
- [ ] All agents installable via copy to `.github/` directory

---

## Phase 2: Planning & Design (Weeks 5–8)

### Objective
Extend coverage to requirements engineering, architecture, and project management processes.

### Deliverables

| ID | Deliverable | Agent/Skill | Process Covered |
|----|-------------|-------------|-----------------|
| P2-01 | Requirements agent | `sdlc-requirements.agent.md` | PROC-001 |
| P2-02 | Architecture agent | `sdlc-architect.agent.md` | PROC-002 |
| P2-03 | Project management agent | `sdlc-pm.agent.md` | PROC-011 |
| P2-04 | Implementation plan skill | `sdlc-implementation-plan/SKILL.md` | PROC-002, PROC-003 |
| P2-05 | Risk management skill | `sdlc-risk-management/SKILL.md` | PROC-012 |
| P2-06 | Sprint reporting skill | `sdlc-sprint-report/SKILL.md` | PROC-011 |
| P2-07 | Requirements traceability skill | `sdlc-traceability/SKILL.md` | PROC-001, PROC-005 |

### Success Criteria

- [ ] Requirements agent generates structured user stories from natural language descriptions
- [ ] Architecture agent produces valid Mermaid diagrams from codebase analysis
- [ ] PM agent generates sprint reports from GitHub project data
- [ ] RTM auto-populates from linked issues and test cases
- [ ] Risk register auto-updated from blocked issues

---

## Phase 3: Release & Operations (Weeks 9–12)

### Objective
Cover deployment, release management, environment management, and operational monitoring.

### Deliverables

| ID | Deliverable | Agent/Skill | Process Covered |
|----|-------------|-------------|-----------------|
| P3-01 | Release management agent | `sdlc-release.agent.md` | PROC-007 |
| P3-02 | Environment management skill | `sdlc-environment/SKILL.md` | PROC-013 |
| P3-03 | Monitoring & observability skill | `sdlc-monitoring/SKILL.md` | PROC-014 |
| P3-04 | Configuration management skill | `sdlc-config-mgmt/SKILL.md` | PROC-008 |
| P3-05 | Release notes generation skill | `sdlc-release-notes/SKILL.md` | PROC-007 |
| P3-06 | Runbook generation skill | `sdlc-runbook/SKILL.md` | PROC-007, PROC-014 |
| P3-07 | Hotfix workflow skill | `sdlc-hotfix/SKILL.md` | PROC-015 |

### Success Criteria

- [ ] Release notes auto-generated from merged PRs with conventional commits
- [ ] Deployment manifests generated from environment templates
- [ ] Health check and monitoring instrumentation auto-generated for new services
- [ ] Runbooks auto-created from alert definitions
- [ ] Hotfix workflow creates expedited branch, PR, and deployment

---

## Phase 4: Governance & Compliance (Weeks 13–16)

### Objective
Complete the SDLC coverage with compliance, governance, documentation, and maintenance processes.

### Deliverables

| ID | Deliverable | Agent/Skill | Process Covered |
|----|-------------|-------------|-----------------|
| P4-01 | Compliance agent | `sdlc-compliance.agent.md` | PROC-010 |
| P4-02 | Documentation agent | `sdlc-documentation.agent.md` | PROC-016 |
| P4-03 | Maintenance/support agent | `sdlc-maintenance.agent.md` | PROC-015 |
| P4-04 | License scanning skill | `sdlc-license-scan/SKILL.md` | PROC-010 |
| P4-05 | Audit evidence skill | `sdlc-audit-evidence/SKILL.md` | PROC-010 |
| P4-06 | Documentation freshness skill | `sdlc-doc-freshness/SKILL.md` | PROC-016 |
| P4-07 | Tech debt tracking skill | `sdlc-tech-debt/SKILL.md` | PROC-015 |
| P4-08 | Post-mortem generation skill | `sdlc-post-mortem/SKILL.md` | PROC-015 |

### Success Criteria

- [ ] License inventory auto-generated from .csproj files
- [ ] Compliance evidence binder auto-populated from CI/CD logs
- [ ] API documentation auto-generated and published
- [ ] Tech debt score calculated and tracked across sprints
- [ ] Post-mortem templates pre-populated from incident data
- [ ] All 16 SDLC processes have at least one agent or skill

---

## Phase 5: Optimization & Integration (Weeks 17–20)

### Objective
Optimize agent interactions, add cross-process orchestration, and create the single-command project setup.

### Deliverables

| ID | Deliverable | Description |
|----|-------------|-------------|
| P5-01 | SDLC Orchestrator agent | Master agent that coordinates across all SDLC agents |
| P5-02 | Project bootstrap skill | One-command setup for new .NET projects with full SDLC config |
| P5-03 | Process maturity dashboard | Agent generates maturity assessment against L1–L4 scale |
| P5-04 | Cross-process metrics skill | Aggregated DORA metrics and SDLC health scorecard |
| P5-05 | Agent self-improvement | Agents learn from project-specific patterns via memory files |
| P5-06 | Customization wizard | Interactive skill that generates .sdlc-config.json from Q&A |

### Success Criteria

- [ ] New .NET project has full SDLC agent suite configured in < 5 minutes
- [ ] Orchestrator agent can execute end-to-end from story to deployment
- [ ] Maturity dashboard accurately reflects process adoption
- [ ] DORA metrics (deployment frequency, lead time, MTTR, change failure rate) auto-calculated
- [ ] Agent suite adapted to 3+ different .NET project types (API, Blazor, Worker Service)

---

## Summary Timeline

```
Phase 1: MVP Core Dev Loop          ████████░░░░░░░░░░░░  Weeks 1-4
Phase 2: Planning & Design          ░░░░████████░░░░░░░░  Weeks 5-8
Phase 3: Release & Operations       ░░░░░░░░████████░░░░  Weeks 9-12
Phase 4: Governance & Compliance    ░░░░░░░░░░░░████████  Weeks 13-16
Phase 5: Optimization               ░░░░░░░░░░░░░░░░████  Weeks 17-20
```

Each phase is independently valuable — teams can adopt phases incrementally.
