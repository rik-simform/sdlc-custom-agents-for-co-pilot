---
name: sdlc-bootstrap
description: 'Bootstrap a new .NET project with full SDLC agent suite, copilot instructions, CI/CD pipeline, and documentation structure. Use when setting up a new project or adding SDLC automation to an existing .NET repo.'
---

# SDLC Project Bootstrap

## Primary Directive

Set up a complete SDLC automation suite for a .NET project. This skill installs all agents, instructions, skills, and documentation templates needed for agent-driven development.

## Input Requirements

Gather the following from the project (auto-detect or ask):

1. **Project Name**: Name of the .NET solution/project
2. **Project Type**: Web API | Blazor | Worker Service | gRPC | Console | Library
3. **Target Framework**: .NET 6 | .NET 7 | .NET 8 | .NET 9
4. **Architecture Pattern**: Clean Architecture | Vertical Slices | Minimal API | N-Tier
5. **Database**: SQL Server | PostgreSQL | SQLite | CosmosDB | None
6. **Auth Provider**: Azure AD | Identity Server | JWT Custom | None
7. **Cloud Provider**: Azure | AWS | None
8. **CI/CD**: GitHub Actions | Azure Pipelines
9. **Compliance Requirements**: SOC2 | ISO 27001 | GDPR | HIPAA | None

## Bootstrap Steps

### Step 1: Install Agent Files

Copy the following agents to `.github/agents/`:

| Agent File | SDLC Process |
|-----------|-------------|
| `sdlc-orchestrator.agent.md` | Master coordination |
| `sdlc-requirements.agent.md` | Requirements engineering |
| `sdlc-architect.agent.md` | Architecture & design |
| `sdlc-implementer.agent.md` | Code implementation |
| `sdlc-reviewer.agent.md` | Code review |
| `sdlc-tester.agent.md` | Testing & QA |
| `sdlc-devops.agent.md` | CI/CD, release, environments |
| `sdlc-security.agent.md` | Security engineering |
| `sdlc-compliance.agent.md` | Compliance & governance |
| `sdlc-pm.agent.md` | Project management |
| `sdlc-documentation.agent.md` | Documentation |

### Step 2: Generate Copilot Instructions

Create `.github/copilot-instructions.md` tailored to the project:

```markdown
# {Project Name} — Copilot Instructions

## Project Overview
{Auto-generated from solution structure analysis}

## Tech Stack
- Framework: {detected .NET version}
- Architecture: {detected pattern}
- Database: {detected ORM/database}
- Auth: {detected auth mechanism}

## Conventions
- Naming: PascalCase for public members, camelCase for locals, _camelCase for private fields
- Structure: {project structure pattern}
- Error handling: Result<T> pattern for business logic, exceptions for unexpected errors

## SDLC Agents
This project uses SDLC automation agents. See `.github/agents/sdlc-*.agent.md` for details.
Reference the SDLC Process Catalog at `docs/sdlc-automation/SDLC-PROCESS-CATALOG.md`.

## Workflow
- Branch naming: feature/{story-id}-{description}, bugfix/{issue-id}-{description}
- Commits: Conventional commits (feat:, fix:, docs:, chore:, refactor:, test:)
- PRs: Use PR template, link to issue, require 1+ review
```

### Step 3: Create Instruction Files

Create `.github/instructions/`:

| File | Purpose |
|------|---------|
| `dotnet.instructions.md` | .NET coding standards |
| `testing.instructions.md` | Test conventions |
| `security.instructions.md` | Security practices |
| `documentation.instructions.md` | Documentation standards |
| `code-review.instructions.md` | Review guidelines |

### Step 4: Create Documentation Structure

```
docs/
├── architecture/
│   ├── decisions/
│   └── api/
├── requirements/
├── guides/
│   └── onboarding.md
├── operations/
│   └── runbooks/
├── security/
│   └── threat-models/
├── compliance/
├── project-management/
│   ├── sprints/
│   └── metrics/
└── sdlc-automation/
    ├── SDLC-PROCESS-CATALOG.md
    └── PHASED-ROLLOUT-PLAN.md
```

### Step 5: Generate CI Pipeline

Create `.github/workflows/ci.yml` appropriate for the project type and framework.

### Step 6: Generate Initial Artifacts

- README.md with project overview
- CONTRIBUTING.md with development workflow
- .editorconfig with .NET conventions
- Directory.Build.props with common build settings

### Step 7: Health Check

Run initial SDLC health check and generate baseline report.

## Customization

After bootstrap, customize via:

1. **`.github/sdlc-config.json`** — Process overrides

```json
{
  "project": {
    "name": "{ProjectName}",
    "type": "webapi",
    "framework": "net8.0"
  },
  "processes": {
    "requirements": { "enabled": true, "tool": "github-issues" },
    "architecture": { "enabled": true, "adrPath": "docs/architecture/decisions" },
    "implementation": { "enabled": true, "pattern": "vertical-slices" },
    "codeReview": { "enabled": true, "minApprovals": 1 },
    "testing": { "enabled": true, "coverageThreshold": 80 },
    "ci": { "enabled": true, "platform": "github-actions" },
    "cd": { "enabled": true, "environments": ["staging", "production"] },
    "security": { "enabled": true, "scanners": ["codeql", "dependabot"] },
    "compliance": { "enabled": false, "frameworks": [] },
    "monitoring": { "enabled": true, "provider": "application-insights" }
  },
  "qualityGates": {
    "coverageMinimum": 80,
    "maxCriticalFindings": 0,
    "requiredReviews": 1,
    "conventionalCommits": true
  }
}
```

## Validation

After bootstrap, verify:
- [ ] All agent files present in `.github/agents/`
- [ ] `copilot-instructions.md` generated and accurate
- [ ] Documentation structure created
- [ ] CI pipeline valid (YAML lint passes)
- [ ] Solution builds successfully
- [ ] Existing tests still pass
