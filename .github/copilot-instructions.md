# SDLC Automation Suite — Copilot Instructions

## Project Overview

This repository contains a comprehensive, agent-driven Software Development Lifecycle (SDLC) automation suite designed for .NET Core and ASP.NET projects. It provides custom GitHub Copilot agents, skills, and instructions that cover the core SDLC processes — from requirements engineering through maintenance.

## Tech Stack

- **Target**: .NET 6+, .NET 8 (preferred), ASP.NET Core
- **Architecture**: Clean Architecture, Vertical Slices, Minimal API (configurable per project)
- **Testing**: MSTest/xUnit, FluentAssertions, Moq, WebApplicationFactory, Playwright
- **CI/CD**: GitHub Actions
- **Cloud**: Azure (App Service, SQL, Key Vault, Application Insights)
- **Security**: CodeQL, Dependabot, OWASP Top 10 compliance

## SDLC Agent Suite

This project ships the following custom agents (`.github/agents/`):

| Agent | Purpose |
|-------|---------|
| `sdlc-orchestrator` | Master agent — routes tasks to specialist agents |
| `sdlc-requirements` | Requirements engineering, user stories, RTM |
| `sdlc-architect` | Architecture, ADRs, diagrams, API contracts |
| `sdlc-implementer` | Code implementation, scaffolding, .NET best practices |
| `sdlc-reviewer` | Code review, security scanning, quality analysis |
| `sdlc-tester` | Testing, QA, coverage, defect reporting |
| `sdlc-devops` | CI/CD pipelines, release, environments |
| `sdlc-security` | Threat modeling, vulnerability management |
| `sdlc-compliance` | License scanning, audit evidence, governance |
| `sdlc-documentation` | Docs generation, runbooks, onboarding |
| `sdlc-research` | Technology research, feasibility studies, pattern analysis |

## Skills

SDLC-specific skills (`.github/skills/`):

| Skill | Purpose |
|-------|---------|
| `sdlc-bootstrap` | Set up full SDLC suite for a new .NET project |
| `sdlc-ci-pipeline` | Generate CI/CD pipelines |
| `sdlc-traceability` | Requirements traceability matrix |
| `sdlc-release-notes` | Auto-generate release notes |
| `sdlc-threat-model` | STRIDE threat modeling |
| `sdlc-dependency-review` | NuGet package analysis — recommends best-fit dependencies per feature, checks existing packages, validates licences, and produces the Dependency Manifest that gates implementation |

## Instructions

Per-context instructions (`.github/instructions/`):

| File | Applies To |
|------|-----------|
| `dotnet.instructions.md` | All `.cs` files |
| `testing.instructions.md` | Test files |
| `security.instructions.md` | All code and config |
| `code-review.instructions.md` | PRs and code files |
| `documentation.instructions.md` | Markdown and code |

## Conventions

- **Naming**: PascalCase (public), _camelCase (private), camelCase (local)
- **Commits**: Conventional commits (`feat:`, `fix:`, `docs:`, `chore:`)
- **Branches**: `feature/{story-id}-{desc}`, `bugfix/{issue-id}-{desc}`, `hotfix/{id}`
- **PRs**: Link to issue, use template, require 1+ review
- **Architecture**: Follow project-specific ADRs in `docs/architecture/decisions/`

## Documentation

- Process catalog: `docs/sdlc-automation/SDLC-PROCESS-CATALOG.md`
- Rollout plan: `docs/sdlc-automation/PHASED-ROLLOUT-PLAN.md`
- Architecture: `docs/architecture/`
- Requirements: `docs/requirements/`
- Operations: `docs/operations/`

## Quality Standards

- Code coverage minimum: 80%
- Zero critical/high security findings at release
- All public APIs have XML documentation
- All requirements traced to tests (RTM)
- Conventional commits enforced

## How to Ship to Any .NET Project

1. Copy `.github/agents/sdlc-*.agent.md` files to target project
2. Copy `.github/skills/sdlc-*` folders to target project
3. Copy `.github/instructions/*.instructions.md` to target project
4. Run the `sdlc-bootstrap` skill to generate project-specific configuration
5. Customize `.github/sdlc-config.json` for project-specific settings
