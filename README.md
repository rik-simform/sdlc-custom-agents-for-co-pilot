# SDLC Automation Suite for .NET — GitHub Copilot Agents

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-6%2B%20%7C%208-purple)](https://dotnet.microsoft.com/)
[![Copilot](https://img.shields.io/badge/GitHub%20Copilot-Agent%20Suite-green)](https://github.com/features/copilot)

A comprehensive, agent-driven **Software Development Lifecycle (SDLC) automation suite** for .NET Core and ASP.NET projects. Ships as a set of GitHub Copilot custom agents, skills, and instructions covering all 16 standard SDLC processes — from requirements engineering through maintenance.

---

## Quick Start

### Ship to Any .NET Project

```bash
# 1. Copy agent files
cp -r .github/agents/sdlc-*.agent.md  <target-project>/.github/agents/

# 2. Copy skill files
cp -r .github/skills/sdlc-*           <target-project>/.github/skills/

# 3. Copy instruction files
cp -r .github/instructions/            <target-project>/.github/instructions/

# 4. Copy copilot instructions
cp .github/copilot-instructions.md     <target-project>/.github/

# 5. Copy SDLC config template
cp .github/sdlc-config.json           <target-project>/.github/

# 6. Run the bootstrap skill in Copilot Chat
# @sdlc-orchestrator Bootstrap this project with full SDLC automation
```

---

## Architecture

```
.github/
├── copilot-instructions.md           # Main Copilot context for the project
├── sdlc-config.json                  # Per-project SDLC configuration
├── agents/                           # Custom Copilot agents (one per SDLC process)
│   ├── sdlc-orchestrator.agent.md    # Master coordinator
│   ├── sdlc-requirements.agent.md    # Requirements engineering
│   ├── sdlc-architect.agent.md       # Architecture & design
│   ├── sdlc-implementer.agent.md     # Code implementation
│   ├── sdlc-reviewer.agent.md        # Code review
│   ├── sdlc-tester.agent.md          # Testing & QA
│   ├── sdlc-devops.agent.md          # CI/CD, release, environments
│   ├── sdlc-security.agent.md        # Security engineering
│   ├── sdlc-compliance.agent.md      # Compliance & governance
│   ├── sdlc-pm.agent.md              # Project management
│   └── sdlc-documentation.agent.md   # Documentation management
├── skills/                           # Reusable SDLC skills
│   ├── sdlc-bootstrap/SKILL.md       # Project setup automation
│   ├── sdlc-ci-pipeline/SKILL.md     # CI/CD pipeline generation
│   ├── sdlc-traceability/SKILL.md    # Requirements traceability
│   ├── sdlc-release-notes/SKILL.md   # Release notes generation
│   ├── sdlc-threat-model/SKILL.md    # STRIDE threat modeling
│   └── sdlc-sprint-report/SKILL.md   # Sprint report generation
└── instructions/                     # Context-specific instructions
    ├── dotnet.instructions.md        # .NET/C# coding standards
    ├── testing.instructions.md       # Test conventions
    ├── security.instructions.md      # Security practices
    ├── code-review.instructions.md   # Review guidelines
    └── documentation.instructions.md # Documentation standards

docs/
├── sdlc-automation/
│   ├── SDLC-PROCESS-CATALOG.md       # All 16 SDLC process definitions
│   └── PHASED-ROLLOUT-PLAN.md        # MVP to full implementation plan
├── architecture/                     # ADRs, diagrams, API contracts
├── requirements/                     # User stories, RTM
├── security/                         # Threat models, scan reports
├── compliance/                       # License inventory, audit evidence
├── operations/                       # Runbooks, deployment guides
└── project-management/               # Sprint reports, risk register
```

---

## SDLC Process Coverage

| # | Process | Agent | Skills | Status |
|---|---------|-------|--------|--------|
| 1 | Requirements Engineering | `sdlc-requirements` | `sdlc-traceability` | ✅ |
| 2 | Architecture & Design | `sdlc-architect` | — | ✅ |
| 3 | Implementation | `sdlc-implementer` | `sdlc-bootstrap` | ✅ |
| 4 | Code Review | `sdlc-reviewer` | — | ✅ |
| 5 | Testing & QA | `sdlc-tester` | — | ✅ |
| 6 | Continuous Integration | `sdlc-devops` | `sdlc-ci-pipeline` | ✅ |
| 7 | CD & Release Management | `sdlc-devops` | `sdlc-release-notes` | ✅ |
| 8 | Configuration Management | `sdlc-devops` | — | ✅ |
| 9 | Security Engineering | `sdlc-security` | `sdlc-threat-model` | ✅ |
| 10 | Compliance & Governance | `sdlc-compliance` | — | ✅ |
| 11 | Project Management | `sdlc-pm` | `sdlc-sprint-report` | ✅ |
| 12 | Risk Management | `sdlc-pm` | — | ✅ |
| 13 | Environment Management | `sdlc-devops` | — | ✅ |
| 14 | Monitoring & Observability | `sdlc-devops` | — | ✅ |
| 15 | Maintenance & Support | `sdlc-documentation` | — | ✅ |
| 16 | Documentation Management | `sdlc-documentation` | — | ✅ |

---

## Agent Usage Examples

### Requirements Engineering
```
@sdlc-requirements Generate user stories for a user authentication feature using Azure AD
```

### Architecture
```
@sdlc-architect Create an ADR for adopting vertical slice architecture with MediatR
```

### Implementation
```
@sdlc-implementer Scaffold a new feature: Order Management with CRUD endpoints
```

### Code Review
```
@sdlc-reviewer Review the changes in this PR for .NET best practices and security
```

### Testing
```
@sdlc-tester Generate unit and integration tests for the OrderService class
```

### CI/CD
```
@sdlc-devops Generate a GitHub Actions CI pipeline for this .NET 8 solution
```

### Security
```
@sdlc-security Create a STRIDE threat model for the authentication module
```

### Sprint Report
```
@sdlc-pm Generate a sprint report for Sprint 12
```

### Full Orchestration
```
@sdlc-orchestrator Deliver feature US-001 end-to-end from requirements to deployment
```

---

## Configuration

Customize SDLC behavior per project via `.github/sdlc-config.json`:

```json
{
  "project": {
    "name": "MyProject",
    "type": "webapi",
    "framework": "net8.0",
    "architecture": "vertical-slices"
  },
  "processes": {
    "testing": { "coverageThreshold": 80 },
    "codeReview": { "minApprovals": 1 },
    "security": { "owaspTop10": true },
    "compliance": { "enabled": true, "frameworks": ["SOC2"] }
  }
}
```

See [`.github/sdlc-config.json`](.github/sdlc-config.json) for the full configuration schema.

---

## Phased Rollout

| Phase | Scope | Timeline |
|-------|-------|----------|
| **Phase 1: MVP** | Implementation, Code Review, Testing, CI | Weeks 1–4 |
| **Phase 2: Planning** | Requirements, Architecture, Project Management | Weeks 5–8 |
| **Phase 3: Release** | CD, Environments, Monitoring, Config Management | Weeks 9–12 |
| **Phase 4: Governance** | Compliance, Documentation, Maintenance | Weeks 13–16 |
| **Phase 5: Optimization** | Orchestrator, Bootstrap, Cross-Process Metrics | Weeks 17–20 |

See [`docs/sdlc-automation/PHASED-ROLLOUT-PLAN.md`](docs/sdlc-automation/PHASED-ROLLOUT-PLAN.md) for full details.

---

## Technology Compatibility

| Technology | Supported | Notes |
|-----------|-----------|-------|
| .NET 6 | ✅ | LTS |
| .NET 7 | ✅ | STS |
| .NET 8 | ✅ | LTS, preferred |
| .NET 9 | ✅ | STS |
| ASP.NET Core Web API | ✅ | Primary target |
| ASP.NET Core MVC | ✅ | |
| Blazor (Server/WASM) | ✅ | |
| Minimal API | ✅ | |
| gRPC | ✅ | |
| Worker Services | ✅ | |
| Class Libraries | ✅ | |
| Entity Framework Core | ✅ | |
| Azure Functions | ⚠️ | Partial (no IaC templates yet) |

---

## Documentation

| Document | Description |
|----------|-------------|
| [SDLC Process Catalog](docs/sdlc-automation/SDLC-PROCESS-CATALOG.md) | All 16 process definitions with artifacts, metrics, and automation hooks |
| [Phased Rollout Plan](docs/sdlc-automation/PHASED-ROLLOUT-PLAN.md) | MVP to full implementation with success criteria |
| [Copilot Instructions](.github/copilot-instructions.md) | Main project-level Copilot context |
| [SDLC Config Schema](.github/sdlc-config.json) | Per-project configuration template |

---

## Contributing

1. Fork this repository
2. Create your feature branch: `feature/{description}`
3. Follow conventional commit format
4. Submit a PR with linked issue
5. Ensure all checks pass

---

## License

MIT