# Cursor Reusable Command: sdlc-bootstrap

**Name**: `sdlc-bootstrap`  
**Type**: Reusable Command  
**Project**: Auto-detected from repository  
**Version**: 3.0 (Project-Discovery Model)  
**Last Updated**: April 23, 2026

---

## How to Set Up This Command in Cursor

1. Open **Cursor Settings** -> **Features** -> **Commands**
2. Click **Add Custom Command**
3. Set **Name**: `sdlc-bootstrap`
4. Set **Description**: "Discover project metadata from the repository and generate Cursor SDLC assets tailored to this project"
5. Set **Prompt**: Copy the full text from the section below
6. Click **Save**

---

## Bootstrap Prompt (Copy This)

```text
=== SDLC Bootstrap - Cursor Project Discovery Model ===
Objective: Discover the active repository's real project profile, generate a project-specific .cursor/sdlc-config.json, then generate/update .cursor rules, agents, and skills from that discovered profile.

NON-NEGOTIABLE RULES
1) Never hardcode project name or stack values (no MyProject defaults).
2) Read project files first, then derive metadata.
3) .github files are reference-only; do not modify them.
4) .cursor assets are Cursor-native outputs tailored to discovered project data.
5) If a value cannot be detected, set it to "unknown" and include confidence notes.

STEP 1: Discover Repository Profile (Evidence-First)
====================================================
Scan the repository and extract metadata from evidence files:

A. Identity + Solution
- Find top-level *.sln or *.slnx.
- Derive projectName from solution filename.
- Enumerate projects from src/, tests/, and *.csproj files.

B. Tech Stack
- Parse all *.csproj for:
  - <TargetFramework> / <TargetFrameworks>
  - SDK type (Web, Worker, Razor, Library, etc.)
  - package references (auth, db, test frameworks, cloud SDKs)
- Derive:
  - targetFramework (primary)
  - projectType (Web API, Razor Pages, Blazor, Worker, Library, Mixed)
  - language/runtime assumptions (.NET)

C. Architecture Pattern
- Infer from folder patterns and naming:
  - Clean Architecture indicators: Domain/Application/Infrastructure/Api split
  - Vertical Slices indicators: Features/* by use-case
  - Minimal API indicators: MapGet/MapPost in Program or Endpoints classes
- Set architecture with confidence score.

D. Testing Profile
- Detect test projects from tests/ and *.UnitTests/*.IntegrationTests.
- Detect frameworks via package refs (MSTest, xUnit, NUnit, FluentAssertions, Moq).
- Infer coverage threshold from existing config/docs; fallback to 80 if not found, but mark as assumed.

E. Data + Security + Cloud
- Inspect appsettings*.json + package refs for:
  - database provider (SqlServer, PostgreSQL, SQLite, Cosmos, None)
  - auth mechanism (JWT, Azure AD, Identity, None)
  - cloud provider (Azure, AWS, GCP, None)
  - scanners/security tooling hints (CodeQL, Dependabot, etc.)

F. Delivery + Conventions
- Inspect .github/workflows/*.yml for CI platform and stages.
- Inspect README/docs/instructions for:
  - branch naming conventions
  - commit style conventions
  - minimum reviewer requirements

Output of STEP 1:
A structured "discoveredProfile" object with value, evidence files, confidence (high/medium/low), and assumptions list.

Example discoveredProfile (from a real .NET 8 ASP.NET Core project):
{
  "projectName":      { "value": "MyProject",        "evidence": ["MyProject.slnx"],                                         "confidence": "high",   "assumptions": [] },
  "projectType":      { "value": "Web API + Razor Pages", "evidence": ["src/MyProject.Api/MyProject.Api.csproj", "src/MyProject.Web/MyProject.Web.csproj"], "confidence": "high", "assumptions": [] },
  "targetFramework":  { "value": "net8.0",            "evidence": ["src/MyProject.Api/MyProject.Api.csproj"],                 "confidence": "high",   "assumptions": [] },
  "architecture":     { "value": "Clean Architecture + CQRS", "evidence": ["src/MyProject.Domain/", "src/MyProject.Application/", "src/MyProject.Infrastructure/"], "confidence": "high", "assumptions": [] },
  "database":         { "value": "SQL Server (EF Core)", "evidence": ["src/MyProject.Infrastructure/MyProject.Infrastructure.csproj (Microsoft.EntityFrameworkCore.SqlServer)"], "confidence": "high", "assumptions": [] },
  "auth":             { "value": "JWT + ASP.NET Core Identity", "evidence": ["src/MyProject.Infrastructure/MyProject.Infrastructure.csproj (Microsoft.AspNetCore.Authentication.JwtBearer)"], "confidence": "high", "assumptions": [] },
  "testingFramework": { "value": "MSTest + FluentAssertions + Moq", "evidence": ["tests/MyProject.UnitTests/MyProject.UnitTests.csproj"], "confidence": "high", "assumptions": [] },
  "coverageThreshold":{ "value": 80,                  "evidence": [],                                                         "confidence": "low",    "assumptions": ["No explicit threshold found; defaulting to 80%"] },
  "cloud":            { "value": "Azure",             "evidence": ["src/MyProject.Api/appsettings.json (ApplicationInsights)", "MyProject.Infrastructure.csproj (Azure.Extensions.AspNetCore.Configuration.Secrets)"], "confidence": "medium", "assumptions": ["Azure assumed from Key Vault + App Insights refs; no IaC found"] },
  "ciPlatform":       { "value": "GitHub Actions",    "evidence": [".github/workflows/"],                                     "confidence": "high",   "assumptions": [] },
  "sourcePath":       { "value": "src/",              "evidence": ["MyProject.slnx"],                                         "confidence": "high",   "assumptions": [] },
  "testsPath":        { "value": "tests/",            "evidence": ["MyProject.slnx"],                                         "confidence": "high",   "assumptions": [] },
  "docsPath":         { "value": "docs/",             "evidence": ["docs/"],                                                  "confidence": "high",   "assumptions": [] },
  "conventionalCommits": { "value": true,             "evidence": [".github/copilot-instructions.md (Conventional commits)"], "confidence": "medium", "assumptions": [] },
  "minReviewers":     { "value": 1,                   "evidence": [".github/copilot-instructions.md (require 1+ review)"],   "confidence": "medium", "assumptions": [] },
  "branchNaming":     { "value": "feature/{id}-{desc}, bugfix/{id}-{desc}, hotfix/{id}", "evidence": [".github/copilot-instructions.md"], "confidence": "medium", "assumptions": [] }
}

STEP 2: Generate .cursor/sdlc-config.json from discoveredProfile
================================================================
Create .cursor/sdlc-config.json using discovered values only.

Required schema:
{
  "cursor": {
    "project": {
      "name": "<detected>",
      "type": "<detected>",
      "targetFramework": "<detected>",
      "architecture": "<detected>",
      "defaultBranch": "<detected-or-main>",
      "description": "<derived from README/docs or concise generated summary>"
    },
    "discovery": {
      "generatedAt": "<ISO8601>",
      "confidence": {
        "projectName": "high|medium|low",
        "projectType": "high|medium|low",
        "framework": "high|medium|low",
        "architecture": "high|medium|low",
        "database": "high|medium|low",
        "auth": "high|medium|low",
        "cloud": "high|medium|low"
      },
      "evidence": {
        "project": ["<paths used>"],
        "architecture": ["<paths used>"],
        "security": ["<paths used>"],
        "testing": ["<paths used>"]
      },
      "assumptions": ["<only when needed>"]
    },
    "agents": {
      "enabled": [
        "sdlc-orchestrator",
        "sdlc-requirements",
        "sdlc-architect",
        "sdlc-implementer",
        "sdlc-reviewer",
        "sdlc-tester",
        "sdlc-devops",
        "sdlc-security",
        "sdlc-compliance",
        "sdlc-documentation",
        "sdlc-research",
        "prompt-engineer"
      ],
      "modelPreference": "claude-opus"
    },
    "skills": [
      "sdlc-bootstrap",
      "sdlc-ci-pipeline",
      "sdlc-dependency-review",
      "sdlc-release-notes",
      "sdlc-threat-model",
      "sdlc-traceability"
    ],
    "rules": [
      "sdlc-core",
      "dotnet",
      "testing",
      "security",
      "documentation",
      "code-review"
    ],
    "paths": {
      "source": "<detected: usually src/>",
      "tests": "<detected: usually tests/>",
      "docs": "<detected: usually docs/>",
      "architecture": "<detected architecture docs path>",
      "requirements": "<detected requirements path>",
      "operations": "<detected operations path or unknown>"
    },
    "standards": {
      "testing": {
        "framework": "<detected>",
        "coverageThreshold": <detected-or-assumed>
      },
      "cloud": {
        "provider": "<detected>",
        "services": ["<detected list>"]
      },
      "security": {
        "sast": "<detected>",
        "dependencyScanning": "<detected>",
        "owaspCompliance": "<detected-or-unknown>"
      },
      "quality": {
        "conventionalCommits": <detected true/false/unknown>,
        "minReviewers": <detected or 1>,
        "branchNamingPattern": "<detected or unknown>"
      }
    }
  }
}

STEP 3: Generate/Update .cursor/rules from discovered config
============================================================
Create/update these files:
- .cursor/rules/sdlc-core.mdc
- .cursor/rules/dotnet.mdc
- .cursor/rules/testing.mdc
- .cursor/rules/security.mdc
- .cursor/rules/documentation.mdc
- .cursor/rules/code-review.mdc

Requirements:
- Pull paths, framework, architecture, and standards from .cursor/sdlc-config.json.
- Use discovered globs (source/tests/docs paths), not hardcoded project paths.
- Mention .github references as knowledge source only.

STEP 4: Generate/Update .cursor/agents (mirror .github/agents)
===============================================================
Create/update .cursor/agents/*.agent.md for all 12 agents.

Each agent file must:
- Reference .github/agents/<name>.agent.md.
- Include project-specific context sourced from .cursor/sdlc-config.json.
- Use detected architecture/stack/security/testing language.
- Avoid hardcoded project names unless discovered project name is used.

STEP 5: Generate/Update .cursor/skills (mirror .github/skills)
===============================================================
Create/update these skill files using mirrored folder structure:
- .cursor/skills/sdlc-bootstrap/SKILL.md
- .cursor/skills/sdlc-ci-pipeline/SKILL.md
- .cursor/skills/sdlc-dependency-review/SKILL.md
- .cursor/skills/sdlc-release-notes/SKILL.md
- .cursor/skills/sdlc-threat-model/SKILL.md
- .cursor/skills/sdlc-traceability/SKILL.md

Each skill file must:
- Reference .github/skills/<name>/SKILL.md.
- Include discovered project context (framework, architecture, paths, quality gates).

STEP 6: Update Entry and Documentation References
=================================================
Update AGENTS.md and any Cursor docs that reference:
- old folder paths
- hardcoded project name
- outdated skill paths

Use discovered project name and current .cursor structure.

STEP 7: Validation and Output Report
====================================
Validate:
- .cursor/sdlc-config.json exists and is valid JSON.
- 12 agent files exist in .cursor/agents/.
- 6 skill folders with SKILL.md exist in .cursor/skills/.
- 6 rules exist in .cursor/rules/.
- No .github files were modified.
- No hardcoded "MyProject" remains unless repository name is actually MyProject.

Output a report containing:
1) Discovered profile summary.
2) Evidence files used per detected field.
3) Confidence + assumptions.
4) Files created/updated counts.
5) Any unknown values requiring user confirmation.
```

---

## Using the Command

Once set up, run in Cursor by:
1. Open Command Palette (`Ctrl+K` / `Cmd+K`)
2. Type `sdlc-bootstrap`
3. Press Enter

---

## Expected Output

After successful bootstrap:
1. `.cursor/sdlc-config.json` tailored to the current project
2. `.cursor/rules/` updated using discovered profile
3. `.cursor/agents/` updated using discovered profile
4. `.cursor/skills/<name>/SKILL.md` updated using discovered profile
5. `AGENTS.md` and related Cursor docs aligned with discovered project identity

---

## Related Files

- [CURSOR-README.md](./CURSOR-README.md)
- [AGENTS.md](./AGENTS.md)
- [.github/sdlc-config.json](./.github/sdlc-config.json)
- [.cursor/rules/sdlc-core.mdc](./.cursor/rules/sdlc-core.mdc)

---

**Version**: 3.0 (Project-Discovery Model)  
**Last Updated**: April 23, 2026
