# Cursor SDLC Automation Suite — MyProject

This repository contains a comprehensive **Cursor-native SDLC automation suite** for the MyProject platform.
All Cursor assets (`.cursor/`) are **independently created and tailored to Cursor's conventions**, taking reference from canonical `.github/` files while maintaining complete operational independence.

## Project Profile (Cursor-Tailored)

| Property | Value |
|----------|-------|
| **Project Name** | MyProject |
| **Project Type** | ASP.NET Core + Razor Pages |
| **Target Framework** | .NET 8.0 |
| **Architecture** | Clean Architecture with CQRS |
| **Default Branch** | main |
| **Testing Framework** | MSTest |
| **Coverage Threshold** | 80% |
| **Cloud Provider** | Azure (App Service, Key Vault) |
| **Security Scanning** | CodeQL (SAST), Dependabot (Dependencies), OWASP Top 10 |
| **Enabled Agents** | 12 specialist agents (see below) |
| **Commit Convention** | Conventional Commits (feat:, fix:, docs:, chore:) |
| **Min Reviewers** | 1 |
| **Branch Pattern** | `feature/{story-id}-{desc}` |

## Cursor-Native SDLC Automation

This environment is fully configured with **12 independent specialist agents**, **6 reusable skills**, **6 context rules**, and a **Cursor-tailored configuration**—all optimized for MyProject's architecture and conventions.

### Entry Point

Start here: [AGENTS.md](./AGENTS.md) — Cursor agent registry and usage guide.

## Architecture: Independence Model

**Key Principle**: `.cursor/` assets are **independent, purpose-built for Cursor workflows**, not projections or synchronizations of `.github/` files.

### Why Independence?

- **Cursor Optimization**: Agents, skills, and rules are written in Cursor's `.agent.md`, `SKILL.md`, and `.mdc` formats with Cursor-specific conventions
- **Project-Tailored**: All assets reference `.cursor/sdlc-config.json` (MyProject-specific Cursor configuration)
- **Maintained Separately**: When `.github/` assets are updated, bootstrap intelligently extracts and transforms content for Cursor, rather than copying
- **No Duplication Overhead**: Cursor doesn't need to parse or understand GitHub agent syntax; assets are natively readable
- **Full Operational Control**: Cursor rules, skills, and agents work exclusively within Cursor—no external dependencies

### Reference Sources

The `.github/` folder serves as **reference material** for domain expertise and best practices:

- [`.github/sdlc-config.json`](./.github/sdlc-config.json) — Project metadata (referenced during bootstrap)
- [`.github/copilot-instructions.md`](./.github/copilot-instructions.md) — Master SDLC framework
- [`.github/agents/`](./.github/agents/) — Domain knowledge for agent creation
- [`.github/skills/`](./.github/skills/) — Workflow patterns and process standards
- [`.github/instructions/`](./.github/instructions/) — Coding standards and quality gates
- [`docs/sdlc-automation/SDLC-PROCESS-CATALOG.md`](./docs/sdlc-automation/SDLC-PROCESS-CATALOG.md) — Full SDLC process guide
- [`docs/sdlc-automation/PHASED-ROLLOUT-PLAN.md`](./docs/sdlc-automation/PHASED-ROLLOUT-PLAN.md) — Implementation roadmap

## Cursor-Native SDLC Assets

### Agents (12 Specialist Agents)

Each agent is **independently created for Cursor** with MyProject-specific context embedded. Folder structure mirrors `.github/agents/`. See [AGENTS.md](./AGENTS.md) for full details.

| Agent | Purpose | Cursor File |
|-------|---------|------------|
| **SDLC Orchestrator** | Master coordination, task routing | `.cursor/agents/sdlc-orchestrator.agent.md` |
| **Requirements Engineer** | User stories, RTM, impact analysis | `.cursor/agents/sdlc-requirements.agent.md` |
| **Architect** | ADRs, system design, API contracts | `.cursor/agents/sdlc-architect.agent.md` |
| **Implementer** | Code scaffolding, .NET best practices | `.cursor/agents/sdlc-implementer.agent.md` |
| **Code Reviewer** | Quality gates, security, performance | `.cursor/agents/sdlc-reviewer.agent.md` |
| **Tester** | Test plans, coverage, QA automation | `.cursor/agents/sdlc-tester.agent.md` |
| **DevOps Engineer** | CI/CD, release management, Azure | `.cursor/agents/sdlc-devops.agent.md` |
| **Security Engineer** | Threat modeling, vulnerability mgmt | `.cursor/agents/sdlc-security.agent.md` |
| **Compliance Officer** | License scanning, audit evidence | `.cursor/agents/sdlc-compliance.agent.md` |
| **Documentation Manager** | API docs, runbooks, guides | `.cursor/agents/sdlc-documentation.agent.md` |
| **Research Analyst** | Technology research, feasibility | `.cursor/agents/sdlc-research.agent.md` |
| **Prompt Engineer** | Prompt optimization, improvement | `.cursor/agents/prompt-engineer.agent.md` |

### Skills (6 Reusable Workflows)

Each skill encapsulates a complex SDLC workflow, **independently crafted for Cursor**. Folder structure mirrors `.github/skills/`:

| Skill | Purpose | Cursor File |
|-------|---------|------------|
| **Bootstrap** | Initialize/refresh Cursor SDLC environment | `.cursor/skills/sdlc-bootstrap/SKILL.md` |
| **CI/CD Pipeline** | Generate GitHub Actions workflows | `.cursor/skills/sdlc-ci-pipeline/SKILL.md` |
| **Dependency Review** | Analyze NuGet packages, licenses | `.cursor/skills/sdlc-dependency-review/SKILL.md` |
| **Release Notes** | Auto-generate from commits & PRs | `.cursor/skills/sdlc-release-notes/SKILL.md` |
| **Threat Modeling** | STRIDE analysis for security | `.cursor/skills/sdlc-threat-model/SKILL.md` |
| **Traceability Matrix** | Link requirements through tests | `.cursor/skills/sdlc-traceability/SKILL.md` |

### Rules (6 Context Rules for File Types)

Rules are applied automatically based on file globs. **Independently created in Cursor's `.mdc` format**:

| Rule File | Applies To | Purpose |
|-----------|-----------|---------|
| `.cursor/rules/00-sdlc-core.mdc` | All tasks | Cursor routing, independence model, agent/skill usage |
| `.cursor/rules/10-dotnet.mdc` | `src/**/*.cs`, `tests/**/*.cs` | .NET 8, naming, architecture patterns |
| `.cursor/rules/20-testing.mdc` | `tests/**/*Test*.cs` | MSTest standards, coverage expectations |
| `.cursor/rules/30-security.mdc` | `**/*.cs`, `**/*.json`, `**/*.yml` | OWASP Top 10, input validation, auth |
| `.cursor/rules/40-documentation.mdc` | `**/*.md`, `**/*.cs` | API docs, ADRs, markdown standards |
| `.cursor/rules/50-code-review.mdc` | `**/*.cs`, `**/*.md` | PR standards, review checklist, gates |

### Cursor Configuration

The Cursor SDLC environment uses **independent configuration**:

- [`.cursor/sdlc-config.json`](./.cursor/sdlc-config.json) — MyProject-specific Cursor settings (agents, skills, paths, conventions)

## Directory Structure

The complete Cursor environment is organized as:

```
.cursor/
├── rules/
│   ├── 00-sdlc-core.mdc                          # SDLC routing & source-of-truth policy
│   ├── 10-dotnet.mdc                             # .NET 8 conventions & patterns
│   ├── 20-testing.mdc                            # MSTest standards & coverage
│   ├── 30-security.mdc                           # OWASP Top 10 & secure coding
│   ├── 40-documentation.mdc                      # API docs, ADRs, markdown
│   └── 50-code-review.mdc                        # PR review & quality gates
│
├── agents/
│   ├── sdlc-orchestrator.agent.md                # Master orchestration
│   ├── sdlc-requirements.agent.md                # Requirements engineering
│   ├── sdlc-architect.agent.md                   # Architecture & ADRs
│   ├── sdlc-implementer.agent.md                 # Code implementation
│   ├── sdlc-reviewer.agent.md                    # Code review
│   ├── sdlc-tester.agent.md                      # Testing & QA
│   ├── sdlc-devops.agent.md                      # CI/CD & DevOps
│   ├── sdlc-security.agent.md                    # Security & threat modeling
│   ├── sdlc-compliance.agent.md                  # Compliance & governance
│   ├── sdlc-documentation.agent.md               # Documentation
│   ├── sdlc-research.agent.md                    # Research & analysis
│   └── prompt-engineer.agent.md                  # Prompt engineering
│
├── skills/
│   ├── sdlc-bootstrap/
│   │   └── SKILL.md                              # Setup & synchronization
│   ├── sdlc-ci-pipeline/
│   │   └── SKILL.md                              # GitHub Actions workflows
│   ├── sdlc-dependency-review/
│   │   └── SKILL.md                              # NuGet dependency analysis
│   ├── sdlc-release-notes/
│   │   └── SKILL.md                              # Release notes automation
│   ├── sdlc-threat-model/
│   │   └── SKILL.md                              # STRIDE threat modeling
│   └── sdlc-traceability/
│       └── SKILL.md                              # Requirements traceability
│
├── sdlc-config.json                              # Cursor-specific configuration
└── README.md

.github/
├── sdlc-config.json                              # Project configuration (CANONICAL)
├── copilot-instructions.md                       # Master instructions (CANONICAL)
├── agents/
│   └── sdlc-*.agent.md (12 files)                # Agent specs (CANONICAL)
├── skills/
│   └── sdlc-*/SKILL.md (6 folders)               # Skill workflows (CANONICAL)
├── instructions/
│   ├── dotnet.instructions.md                    # .NET standards
│   ├── testing.instructions.md                   # Testing standards
│   ├── security.instructions.md                  # Security standards
│   ├── documentation.instructions.md             # Documentation standards
│   └── code-review.instructions.md               # Review standards
└── workflows/
    └── *.yml (CI/CD pipelines)

docs/
├── sdlc-automation/
│   ├── SDLC-PROCESS-CATALOG.md                   # Full process documentation
│   └── PHASED-ROLLOUT-PLAN.md                    # Implementation roadmap
├── requirements/
│   ├── login/, orders/, rbac-inventory/, rbac-inventory-ui/
│   └── (User stories, PRDs, traceability matrices)
└── architecture/
    └── decisions/
        └── ADR-*.md (Architecture Decision Records)

AGENTS.md                                          # Cursor entry point & agent registry
CURSOR-README.md                                   # This file

```

## Quick Start for Cursor Users

### 1. Initial Setup

Read these files in order:

1. [`AGENTS.md`](./AGENTS.md) — Cursor agent registry and usage guide
2. [`.cursor/rules/00-sdlc-core.mdc`](./.cursor/rules/00-sdlc-core.mdc) — Core routing and policy
3. [`.github/sdlc-config.json`](./.github/sdlc-config.json) — Project profile reference

### 2. Using Specialist Agents

For complex tasks, invoke agents from [`AGENTS.md`](./AGENTS.md):

- **Requirements**: @sdlc-requirements — Elicit & validate requirements
- **Architecture**: @sdlc-architect — Design systems, create ADRs
- **Implementation**: @sdlc-implementer — Scaffold features, generate code
- **Testing**: @sdlc-tester — Write tests, measure coverage
- **Review**: @sdlc-reviewer — Analyze code quality & security
- **DevOps**: @sdlc-devops — CI/CD pipelines, deployment
- **Security**: @sdlc-security — Threat modeling, vulnerability analysis
- **Orchestration**: @sdlc-orchestrator — Multi-phase delivery coordination

### 3. Using Reusable Skills

For repeatable workflows, reference skills from [`.cursor/skills/`](./.cursor/skills/):

- **sdlc-bootstrap** (`.cursor/skills/sdlc-bootstrap/SKILL.md`) — Initialize/refresh independent Cursor environment
- **sdlc-ci-pipeline** (`.cursor/skills/sdlc-ci-pipeline/SKILL.md`) — Generate GitHub Actions workflows
- **sdlc-dependency-review** (`.cursor/skills/sdlc-dependency-review/SKILL.md`) — Analyze NuGet dependencies
- **sdlc-release-notes** (`.cursor/skills/sdlc-release-notes/SKILL.md`) — Auto-generate release notes
- **sdlc-threat-model** (`.cursor/skills/sdlc-threat-model/SKILL.md`) — STRIDE threat modeling
- **sdlc-traceability** (`.cursor/skills/sdlc-traceability/SKILL.md`) — Link requirements to tests

### 4. Working with Rules

Rules are applied automatically by file type:

- **dotnet** — Applied to `src/**/*.cs` and `tests/**/*.cs`
- **testing** — Applied to `tests/**/*Test*.cs`
- **security** — Applied to all code and config files
- **documentation** — Applied to `**/*.md` and `**/*.cs`
- **code-review** — Applied during PR review

---

## Bootstrap & Independence Model

### When to Run Bootstrap

Run the `sdlc-bootstrap` command when:

1. **Initial Setup** — First time creating independent Cursor environment
2. **Reference Updates** — After updating `.github/sdlc-config.json`, agents, skills, or instructions
3. **Project Changes** — When project type, framework, or architecture changes
4. **Periodic Maintenance** — Quarterly to align with latest practices

### How Bootstrap Works

The `sdlc-bootstrap` command executes a **transformation pipeline**:

1. **Reference Analysis** — Reads `.github/` files as domain knowledge (read-only)
2. **Asset Creation** — Creates independent Cursor-native assets in `.cursor/`
3. **Configuration** — Generates `.cursor/sdlc-config.json` with Cursor-specific settings
4. **Asset Generation** — Creates all 6 rules, 12 agents, and 6 skills independently
5. **Entry Point Update** — Updates `AGENTS.md` with complete registries
6. **Validation Report** — Outputs independence metrics and quality checks

**Key**: No files are copied from `.github/`; instead, bootstrap *extracts domain knowledge and creates new Cursor-native artifacts*.

### Bootstrap Command

For complete bootstrap instructions, see: [SDLC-BOOTSTRAP-COMMAND.md](./SDLC-BOOTSTRAP-COMMAND.md)

Copy the bootstrap prompt into **Cursor Settings** → **Features** → **Commands** as a reusable command named `sdlc-bootstrap`.

---

## Quality Standards & Conventions

### Code Standards

All code in MyProject follows these standards:

- **Naming**: PascalCase (public), _camelCase (private), camelCase (local)
- **Architecture**: Clean Architecture with Vertical Slices and CQRS
- **Async**: Async/await throughout .NET 8 codebase
- **Testing**: MSTest with FluentAssertions, 80% coverage minimum
- **Security**: OWASP Top 10 compliance, CodeQL scanning

### Commit Conventions

- `feat:` — New feature
- `fix:` — Bug fix
- `docs:` — Documentation
- `test:` — Test additions/changes
- `chore:` — Build/tooling changes
- `refactor:` — Code restructuring (no behavior change)

### Branch Naming

- **Feature**: `feature/{story-id}-{desc}` (e.g., `feature/ORD-001-order-confirmation`)
- **Bugfix**: `bugfix/{issue-id}-{desc}`
- **Hotfix**: `hotfix/{id}`

### PR Requirements

- Link to GitHub issue
- Use PR template
- Minimum 1 approval required
- All tests must pass
- Code coverage must be maintained at 80%+
- No critical/high security findings

---

## Independence Model Deep Dive

### The Three-Layer Architecture

**Layer 1: Source of Truth (.github/)**
- Contains canonical SDLC framework and conventions
- Never modified by Cursor runtime
- Serves as reference for all .cursor asset creation

**Layer 2: Cursor-Native Assets (.cursor/)**
- Independently created for Cursor environment
- Optimized for Cursor's execution model
- Include MyProject-specific context embedded
- Fully operational without referencing .github at runtime

**Layer 3: Operations (AGENTS.md, .cursor/sdlc-config.json)**
- Primary entry points for Cursor workflows
- Registry of all available agents and skills
- Project configuration specific to Cursor execution

### Benefits of Independence

✅ **Zero Overhead**: Cursor never needs to parse `.github/` files  
✅ **Performance**: All context embedded in `.cursor/` assets  
✅ **Portability**: `.cursor/` can be used independently  
✅ **Clarity**: No confusion about what is canonical vs. projected  
✅ **Maintainability**: Each layer has clear responsibilities  
✅ **Scalability**: Easy to add new agents/skills without modifying `.github/`

---

## For More Information

| Topic | File |
|-------|------|
| **Agent Registry** | [AGENTS.md](./AGENTS.md) |
| **Bootstrap Command** | [SDLC-BOOTSTRAP-COMMAND.md](./SDLC-BOOTSTRAP-COMMAND.md) |
| **SDLC Processes** | [`docs/sdlc-automation/SDLC-PROCESS-CATALOG.md`](./docs/sdlc-automation/SDLC-PROCESS-CATALOG.md) |
| **Project Architecture** | [`docs/architecture/`](./docs/architecture/) |
| **Requirements** | [`docs/requirements/`](./docs/requirements/) |
| **.NET Standards** | [`.cursor/rules/10-dotnet.mdc`](./.cursor/rules/10-dotnet.mdc) |
| **Testing Standards** | [`.cursor/rules/20-testing.mdc`](./.cursor/rules/20-testing.mdc) |
| **Security Standards** | [`.cursor/rules/30-security.mdc`](./.cursor/rules/30-security.mdc) |

---

**Cursor SDLC Suite Version**: 2.0 (Independence Model)  
**Last Updated**: April 23, 2026  
**Project**: MyProject  
**Architecture**: Clean Architecture with CQRS  
**Framework**: ASP.NET Core + Razor Pages (.NET 8.0)


---

## Validation Checklist

Run this checklist after bootstrap to verify complete synchronization:

### Configuration
- [ ] `.github/sdlc-config.json` exists and contains MyProject profile
- [ ] Project name: `MyProject`
- [ ] Type: `ASP.NET Core + Razor Pages`
- [ ] Framework: `net8.0`
- [ ] Architecture: `Clean Architecture with CQRS`
- [ ] Testing: `MSTest` with `80%` coverage threshold
- [ ] Cloud: `Azure` with `App Service` and `Key Vault`
- [ ] Security enabled: `CodeQL`, `Dependabot`, `OWASP Top 10`

### Rules
- [ ] `.cursor/rules/00-sdlc-core.mdc` exists with source-of-truth policy
- [ ] `.cursor/rules/10-dotnet.mdc` exists (from `.github/instructions/dotnet.instructions.md`)
- [ ] `.cursor/rules/20-testing.mdc` exists (from `.github/instructions/testing.instructions.md`)
- [ ] `.cursor/rules/30-security.mdc` exists (from `.github/instructions/security.instructions.md`)
- [ ] `.cursor/rules/40-documentation.mdc` exists (from `.github/instructions/documentation.instructions.md`)
- [ ] `.cursor/rules/50-code-review.mdc` exists (from `.github/instructions/code-review.instructions.md`)
- [ ] All rules reference their canonical `.github/instructions/` sources
- [ ] File globs match repository patterns:
  - [ ] `src/**/*.cs`, `tests/**/*.cs` exist for dotnet rule
  - [ ] `tests/**/*Test*.cs` files exist for testing rule
  - [ ] `**/*.md` files exist for documentation rule

### Agents (12 files)
- [ ] `.cursor/agents/sdlc-orchestrator.agent.md` references `.github/agents/sdlc-orchestrator.agent.md`
- [ ] `.cursor/agents/sdlc-requirements.agent.md` references `.github/agents/sdlc-requirements.agent.md`
- [ ] `.cursor/agents/sdlc-architect.agent.md` references `.github/agents/sdlc-architect.agent.md`
- [ ] `.cursor/agents/sdlc-implementer.agent.md` references `.github/agents/sdlc-implementer.agent.md`
- [ ] `.cursor/agents/sdlc-reviewer.agent.md` references `.github/agents/sdlc-reviewer.agent.md`
- [ ] `.cursor/agents/sdlc-tester.agent.md` references `.github/agents/sdlc-tester.agent.md`
- [ ] `.cursor/agents/sdlc-devops.agent.md` references `.github/agents/sdlc-devops.agent.md`
- [ ] `.cursor/agents/sdlc-security.agent.md` references `.github/agents/sdlc-security.agent.md`
- [ ] `.cursor/agents/sdlc-compliance.agent.md` references `.github/agents/sdlc-compliance.agent.md`
- [ ] `.cursor/agents/sdlc-documentation.agent.md` references `.github/agents/sdlc-documentation.agent.md`
- [ ] `.cursor/agents/sdlc-research.agent.md` references `.github/agents/sdlc-research.agent.md`
- [ ] `.cursor/agents/prompt-engineer.agent.md` references `.github/agents/prompt-engineer.agent.md`
- [ ] All agents are valid Markdown with descriptions and use cases
- [ ] All agents reference their canonical sources

### Skills (6 folders)
- [ ] `.cursor/skills/sdlc-bootstrap/SKILL.md` references `.github/skills/sdlc-bootstrap/SKILL.md`
- [ ] `.cursor/skills/sdlc-ci-pipeline/SKILL.md` references `.github/skills/sdlc-ci-pipeline/SKILL.md`
- [ ] `.cursor/skills/sdlc-dependency-review/SKILL.md` references `.github/skills/sdlc-dependency-review/SKILL.md`
- [ ] `.cursor/skills/sdlc-release-notes/SKILL.md` references `.github/skills/sdlc-release-notes/SKILL.md`
- [ ] `.cursor/skills/sdlc-threat-model/SKILL.md` references `.github/skills/sdlc-threat-model/SKILL.md`
- [ ] `.cursor/skills/sdlc-traceability/SKILL.md` references `.github/skills/sdlc-traceability/SKILL.md`
- [ ] All skill files are valid Markdown with descriptions and workflows
- [ ] All skill folders mirror `.github/skills/` structure

### Entry Point
- [ ] `AGENTS.md` exists as the primary Cursor entry point
- [ ] AGENTS.md starts with mandatory reading order
- [ ] AGENTS.md includes all 12 agents with descriptions
- [ ] AGENTS.md includes all 6 skills with descriptions
- [ ] AGENTS.md includes MyProject configuration summary
- [ ] AGENTS.md links to `.cursor/rules/00-sdlc-core.mdc`
- [ ] AGENTS.md links to `.github/sdlc-config.json`
- [ ] AGENTS.md links to CURSOR-README.md for source-of-truth policy

### Integration
- [ ] No `.github` files were modified during bootstrap
- [ ] All `.cursor` artifacts are readable Markdown
- [ ] All cross-references are valid (link targets exist)
- [ ] No duplicated long guidance copied from `.github/` into `.cursor/`
- [ ] All canonical sources are linked, not copied
- [ ] Project context (MyProject, .NET 8, CQRS, etc.) is preserved

### Documentation
- [ ] CURSOR-README.md is up-to-date with project information
- [ ] CURSOR-README.md includes this validation checklist
- [ ] CURSOR-README.md includes enhanced bootstrap command
- [ ] Quick Start section is accurate and usable
- [ ] Directory structure diagram matches actual repository layout

---

## Troubleshooting

### Issue: Cursor not recognizing rules

**Solution:** Verify `.cursor/rules/` files are valid Markdown with `alwaysApply: true` frontmatter in core rule.

### Issue: Agents not available

**Solution:** Ensure `.cursor/agents/` contains all 12 `.agent.md` files and AGENTS.md lists them all.

### Issue: Out of sync with .github updates

**Solution:** Re-run `sdlc-bootstrap` command to refresh all `.cursor/` artifacts.

### Issue: File globs not matching

**Solution:** Check that file patterns in rules (e.g., `src/**/*.cs`) match actual repository structure.

---

## Support & Contribution

- **Report Issues**: Create an issue linking to the specific `.cursor/` or `.github/` file
- **Suggest Improvements**: PRs welcome for better Cursor integration
- **Update Process**: Always modify `.github/` canonicals first, then run `sdlc-bootstrap` to sync `.cursor/`

---

**Last Updated**: April 23, 2026  
**Project**: MyProject  
**Cursor Version**: Compatible with Cursor v0.42+  
**SDLC Suite Version**: 2.0