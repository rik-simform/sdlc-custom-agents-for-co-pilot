# Cursor Setup Guide for the SDLC Automation Suite

This guide explains how to use the existing GitHub Copilot SDLC assets in this repository from Cursor.
This repository already contains the SDLC source material in `.github/agents/`, `.github/instructions/`, `.github/skills/`, and `.github/sdlc-config.json`.
Cursor does not execute GitHub Copilot custom agent files directly, so the correct approach is to treat the `.github` assets as the canonical source and project them into Cursor rules, commands, and optional `AGENTS.md` files.

## What This Repository Already Provides

The repository already contains the SDLC assets listed below.

| Asset | Current Location | Purpose | Cursor Equivalent |
|---|---|---|---|
| Agents | `.github/agents/` | Role-based SDLC behaviors such as orchestrator, implementer, tester, and security | Manual or intelligent `.cursor/rules/*.mdc` rules, plus optional `AGENTS.md` |
| Instructions | `.github/instructions/` | File-type-specific coding, documentation, testing, review, and security standards | File-scoped `.cursor/rules/*.mdc` rules using `globs` |
| Skills | `.github/skills/` | Reusable workflows such as bootstrap, CI pipeline, release notes, traceability, and threat modeling | Reusable Cursor commands and manual rules |
| Config | `.github/sdlc-config.json` | Project metadata and SDLC defaults | Shared source of truth referenced by Cursor rules and commands |
| Documentation | `docs/` | ADRs, requirements, rollout plan, and SDLC process catalog | Context sources that Cursor should read before execution |

## Current Repository SDLC Profile

The current `.github/sdlc-config.json` defines the following baseline for this repository.

| Area | Current Value |
|---|---|
| Project name | `MyProject` |
| Project type | `ASP.NET Core + Razor Pages` |
| Target framework | `net8.0` |
| Architecture | `Clean Architecture with CQRS` |
| Testing | `MSTest` with `80` coverage threshold |
| Cloud | `Azure` with `App Service` and `Key Vault` |
| Security | `CodeQL`, `Dependabot`, `OWASP Top 10` |
| Docs paths | `docs/architecture/decisions`, `docs/requirements`, `docs/operations` |
| Enabled agents | `sdlc-orchestrator`, `sdlc-requirements`, `sdlc-architect`, `sdlc-implementer`, `sdlc-reviewer`, `sdlc-tester`, `sdlc-devops`, `sdlc-security` |

Keep `.github/sdlc-config.json` as the single source of truth.
Do not duplicate those values into multiple Cursor rules unless you have a hard requirement to do so.
Cursor rules should reference the file and instruct Agent to read it first.

## Recommended Cursor Operating Model

Use this repository model when you want the same SDLC behavior in Cursor.

1. Keep the existing `.github/` SDLC assets in source control.
2. Add a `.cursor/rules/` folder for Cursor-native rules.
3. Optionally add a root `AGENTS.md` file as a lightweight entrypoint for Cursor Agent.
4. Register one reusable Cursor command named `sdlc-bootstrap`.
5. Let the command generate or refresh the Cursor rule files from the `.github` assets.
6. Use `.github/sdlc-config.json` to drive project metadata and quality gates.

## How the GitHub Copilot Assets Map to Cursor

### Agents

GitHub Copilot agent files are not consumed natively by Cursor.
Treat each `.github/agents/*.agent.md` file as canonical prompt content that should be converted into a Cursor project rule or referenced from `AGENTS.md`.

| GitHub Copilot Asset | Suggested Cursor Mapping | Recommended Activation |
|---|---|---|
| `sdlc-orchestrator.agent.md` | `orchestrator.mdc` | Apply manually with `@orchestrator` |
| `sdlc-requirements.agent.md` | `requirements.mdc` | Apply manually |
| `sdlc-architect.agent.md` | `architect.mdc` | Apply manually |
| `sdlc-implementer.agent.md` | `implementer.mdc` | Apply manually |
| `sdlc-reviewer.agent.md` | `reviewer.mdc` | Apply manually |
| `sdlc-tester.agent.md` | `tester.mdc` | Apply manually |
| `sdlc-devops.agent.md` | `devops.mdc` | Apply manually |
| `sdlc-security.agent.md` | `security-agent.mdc` | Apply manually |
| `sdlc-compliance.agent.md` | `compliance.mdc` | Apply manually |
| `sdlc-documentation.agent.md` | `documentation-agent.mdc` | Apply manually |
| `sdlc-research.agent.md` | `research.mdc` | Apply manually |
| `prompt-engineer.agent.md` | `prompt-engineer.mdc` | Apply manually |

### Instructions

The instruction files map cleanly to Cursor file-scoped rules.
These should be small `.mdc` files with `globs`, `description`, and focused content that points back to the canonical `.github/instructions/*` file.

| GitHub Copilot Instruction | Suggested Cursor Rule | Glob Strategy |
|---|---|---|
| `.github/instructions/dotnet.instructions.md` | `.cursor/rules/dotnet.mdc` | `src/**/*.cs`, `tests/**/*.cs` |
| `.github/instructions/testing.instructions.md` | `.cursor/rules/testing.mdc` | `tests/**/*Test*.cs`, `tests/**/*test*.cs` |
| `.github/instructions/security.instructions.md` | `.cursor/rules/security.mdc` | `**/*.cs`, `**/*.json`, `**/*.yml`, `**/*.yaml` |
| `.github/instructions/documentation.instructions.md` | `.cursor/rules/documentation.mdc` | `**/*.md`, `**/*.cs` |
| `.github/instructions/code-review.instructions.md` | `.cursor/rules/code-review.mdc` | `**/*.cs`, `**/*.md` |

### Skills

The skill files are best treated as workflow prompts.
In Cursor, the most practical equivalents are reusable commands and manually-invoked rules.

| Skill | Cursor Equivalent | Typical Use |
|---|---|---|
| `sdlc-bootstrap` | Command named `sdlc-bootstrap` | Initialize or refresh Cursor rules and SDLC setup |
| `sdlc-ci-pipeline` | Command named `sdlc-ci-pipeline` | Generate or update workflow files |
| `sdlc-traceability` | Rule or command named `sdlc-traceability` | Update RTM files |
| `sdlc-release-notes` | Command named `sdlc-release-notes` | Draft release notes |
| `sdlc-threat-model` | Command named `sdlc-threat-model` | Produce STRIDE artifacts |
| `sdlc-dependency-review` | Command named `sdlc-dependency-review` | Dependency manifest and package decisions |

## Cursor Bootstrap Command

Create a reusable Cursor command named `sdlc-bootstrap` in `Cursor Settings > Rules, Commands`.
Use the prompt below as the command body.

```text
Bootstrap this repository for Cursor using the existing SDLC assets as the canonical source.

Read these files and folders first:
- .github/sdlc-config.json
- .github/copilot-instructions.md
- .github/agents/
- .github/instructions/
- .github/skills/
- docs/sdlc-automation/SDLC-PROCESS-CATALOG.md
- docs/sdlc-automation/PHASED-ROLLOUT-PLAN.md

Then perform these tasks in order:
1. Analyse the project type, framework, architecture, testing stack, cloud settings, security posture, and documentation paths from .github/sdlc-config.json.
2. Create or refresh .cursor/rules/ with focused rule files that map the GitHub Copilot instructions and agents into Cursor-native rules.
3. Keep rules short and composable, prefer references to canonical files, and avoid duplicating long guidance blocks.
4. Create or refresh a root AGENTS.md that tells Cursor Agent to read .github/sdlc-config.json first and then use the relevant .cursor/rules files.
5. Create rule files for dotnet, testing, security, documentation, code review, and orchestrator workflows.
6. Where appropriate, use globs so rules apply automatically to matching files.
7. Use manual rules for specialist SDLC roles such as orchestrator, architect, implementer, tester, reviewer, devops, security, compliance, documentation, and research.
8. Produce a short report summarizing which Cursor rules were created, which GitHub Copilot assets they map to, and any gaps that still require manual setup in Cursor Settings.

Constraints:
- Treat .github/sdlc-config.json as the single source of truth.
- Do not overwrite source SDLC assets in .github unless explicitly asked.
- Preserve existing repo structure and docs paths.
- Prefer .cursor/rules/*.mdc files with description, globs, and alwaysApply only when necessary.
- Keep each rule under 500 lines and split large concerns into multiple rules.
```

## Recommended Cursor Rule Layout

After running the bootstrap command, the target repository should ideally contain a structure similar to the one below.

```text
.cursor/
  rules/
    00-sdlc-core.mdc
    10-dotnet.mdc
    20-testing.mdc
    30-security.mdc
    40-documentation.mdc
    50-code-review.mdc
    60-orchestrator.mdc
    61-requirements.mdc
    62-architect.mdc
    63-implementer.mdc
    64-reviewer.mdc
    65-tester.mdc
    66-devops.mdc
    67-security-agent.mdc
    68-compliance.mdc
    69-documentation-agent.mdc
    70-research.mdc
AGENTS.md
.github/
  sdlc-config.json
  agents/
  instructions/
  skills/
```

## Suggested Content Model for the Cursor Rules

Use a small number of core patterns when generating the `.cursor/rules` files.

### `00-sdlc-core.mdc`

Make this the shared foundation rule.
It should tell Cursor Agent to read `.github/sdlc-config.json`, then consult `.github/copilot-instructions.md`, and then apply specialized rules depending on the task.

### Language and file rules

Use the existing instruction files as the canonical sources for the rules below.

1. `10-dotnet.mdc` for C# coding standards.
2. `20-testing.mdc` for test naming, coverage, and AAA structure.
3. `30-security.mdc` for OWASP, secrets handling, and dependency scanning.
4. `40-documentation.mdc` for Markdown style and freshness standards.
5. `50-code-review.mdc` for review expectations and PR-quality checks.

### Specialist workflow rules

Use manual rules for the specialist SDLC roles.
This keeps them out of every chat session while still making them available by explicit mention.

1. `60-orchestrator.mdc` for multi-phase SDLC flow control.
2. `61-requirements.mdc` for user stories, acceptance criteria, and RTM work.
3. `62-architect.mdc` for ADRs, contracts, and diagrams.
4. `63-implementer.mdc` for production code generation.
5. `64-reviewer.mdc` for review passes.
6. `65-tester.mdc` for test authoring and verification.
7. `66-devops.mdc` for CI, CD, and environment setup.
8. `67-security-agent.mdc` for threat modeling and secure design review.
9. `68-compliance.mdc` for governance workflows.
10. `69-documentation-agent.mdc` for docs updates and freshness.
11. `70-research.mdc` for technical evaluations and comparisons.

## How to Set This Up in Cursor

### Option A: Minimum setup

Use this option when you want the fastest path to basic compatibility.

1. Open the repository in Cursor.
2. Keep the existing `.github/` folder unchanged.
3. Create a root `AGENTS.md` file that points Cursor to `.github/sdlc-config.json`, `.github/copilot-instructions.md`, and the docs folders.
4. Add the reusable `sdlc-bootstrap` command in `Cursor Settings > Rules, Commands`.
5. Run the command from Cursor Agent chat and let it generate `.cursor/rules/`.
6. Review the generated rules and commit them.

### Option B: Full team setup

Use this option when you want Cursor to behave consistently across contributors.

1. Complete all steps from Option A.
2. Move shared organization-wide guidance into Cursor Team Rules if your plan supports them.
3. Keep repo-specific logic in `.cursor/rules/`.
4. Keep user-specific style preferences in Cursor User Rules only.
5. Re-run `sdlc-bootstrap` whenever `.github/sdlc-config.json`, instruction files, or agent definitions change materially.

## How to Use the SDLC Suite from Cursor After Setup

Once the rules and command are in place, use Cursor Agent with explicit task framing.

### Example prompts

```text
Use the orchestrator workflow to deliver feature US-ORD-003 end to end.
Read .github/sdlc-config.json first.
```

```text
Apply the implementer and testing rules.
Add inventory search filters to the web app and update unit tests.
```

```text
Apply the reviewer and security rules.
Review the current auth endpoints for broken access control and missing validation.
```

```text
Apply the documentation and traceability workflows.
Update the affected ADR, requirements file, and traceability matrix for the inventory UI change.
```

## Guidance for `AGENTS.md`

Cursor supports `AGENTS.md` as a simple markdown instruction file.
Use it as a lightweight front door rather than copying all SDLC content into one large file.

Your `AGENTS.md` should do the following.

1. Tell Cursor Agent to read `.github/sdlc-config.json` before acting.
2. Point to `.github/copilot-instructions.md` for project-wide conventions.
3. Point to `.cursor/rules/` for task-specific behavior.
4. Point to `docs/requirements/`, `docs/architecture/decisions/`, and `docs/sdlc-automation/` for requirements and process context.
5. Instruct the agent not to modify `.github` SDLC source assets unless explicitly requested.

## Guidance for `.github/sdlc-config.json` in Cursor

The config file should remain technology-neutral and tool-neutral enough to serve both GitHub Copilot and Cursor.
Cursor should read it, not replace it.

Use these practices.

1. Update `project`, `testing`, `cloud`, `security`, `documentation`, `agents`, and `quality` whenever the repo architecture changes.
2. Keep path values accurate because the rules and commands should use them directly.
3. Keep coverage thresholds and review minima aligned with CI.
4. Add new enabled agents when you introduce new SDLC role files.
5. Commit config changes before regenerating Cursor rules so the generated rules match source control.

## Recommended Validation Checklist

After setup, validate the Cursor integration with the checks below.

1. Confirm `.github/sdlc-config.json` is present and accurate.
2. Confirm `.cursor/rules/` exists and contains focused rule files.
3. Confirm the core rule references the `.github` canonical files instead of duplicating large documents.
4. Confirm C# files trigger the .NET and security rules.
5. Confirm test files trigger the testing rule.
6. Confirm Markdown files trigger the documentation and review rules.
7. Confirm the `sdlc-bootstrap` command can refresh the rules without changing `.github` source assets.
8. Confirm at least one manual specialist rule can be invoked on demand.

## Important Differences Between GitHub Copilot Agents and Cursor

Keep these differences in mind when porting the workflow.

1. Cursor does not natively execute `.github/agents/*.agent.md` as GitHub Copilot custom agents.
2. Cursor project rules in `.cursor/rules/` are the primary compatibility mechanism.
3. `AGENTS.md` is useful for a simple top-level instruction layer, but it is not a replacement for well-scoped rule files.
4. Cursor rules should stay short, focused, and composable.
5. Cursor supports manual rule invocation, file-scoped rules via globs, always-applied rules, and intelligent application based on descriptions.

## Recommended Rollout Strategy

Use a phased rollout instead of attempting full parity in one step.

1. Start with `00-sdlc-core.mdc` plus the five instruction-derived rules.
2. Add the orchestrator and implementer manual rules next.
3. Add the remaining specialist rules once the team has validated prompt quality.
4. Promote stable repo-wide guidance into Team Rules only if you need organization-wide enforcement.

## Source Files You Should Treat as Canonical

Use these repository files as the authoritative sources when building Cursor support.

1. `.github/copilot-instructions.md`
2. `.github/sdlc-config.json`
3. `.github/agents/*.agent.md`
4. `.github/instructions/*.instructions.md`
5. `.github/skills/*/SKILL.md`
6. `docs/sdlc-automation/SDLC-PROCESS-CATALOG.md`
7. `docs/sdlc-automation/PHASED-ROLLOUT-PLAN.md`

## Final Recommendation

Do not fork the SDLC guidance into a separate Cursor-only truth source.
Keep the `.github` SDLC assets canonical, let Cursor consume them through rules and commands, and use `sdlc-bootstrap` as the repeatable synchronization point.