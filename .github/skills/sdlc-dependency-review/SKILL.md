---
name: sdlc-dependency-review
description: 'Analyse a feature or set of requirements and recommend the best-fit NuGet packages and external dependencies for a .NET project. Scans the existing solution to avoid duplicates, evaluates alternatives, checks licences, and produces a structured Dependency Manifest that gates implementation.'
---

# SDLC Dependency Review Skill

## Primary Directive

Given a feature description or a set of user stories / acceptance criteria, produce a structured **Dependency Manifest** that:

1. Identifies every implied technical need
2. Recommends the best-fit NuGet package (with version, justification, licence, and alternatives)
3. Confirms which packages are already in the solution (no duplicates)
4. Flags any new packages that must be added and to which `.csproj`
5. Outputs in both Markdown (for docs) and JSON (for `rtd.json` → `depRecommendations`)

---

## Input Requirements

Gather the following before executing (auto-detect from context where possible):

| Input | Source |
|---|---|
| Feature description or user stories | User prompt or `docs/requirements/{epic}/` |
| Acceptance criteria | User story `### Acceptance Criteria` sections |
| Existing `.csproj` file(s) | Scan entire `src/` and `tests/` tree |
| Target .NET version | `.github/sdlc-config.json` → `project.targetFramework` or `.csproj` |
| Architecture pattern | `.github/sdlc-config.json` → `project.architectureStyle` |

---

## Execution Steps

### Step 1 — Identify Technical Needs from Requirements

Read every acceptance criterion. For each, map the implied technical need:

| Acceptance Criterion Signal | Implied Need |
|---|---|
| "send email / notification" | Email / SMTP client |
| "upload / download file" | File / blob storage |
| "cache results" | In-memory or distributed cache |
| "schedule / run periodically" | Background job / scheduler |
| "call external API" | HTTP client abstraction |
| "generate PDF / report" | PDF or reporting library |
| "read / write Excel / CSV" | Spreadsheet / CSV library |
| "search across records" | Full-text or vector search |
| "map objects between layers" | Object mapper |
| "validate input" | Validation library |
| "authenticate / authorize users" | Identity / auth library |
| "log structured events" | Logging / telemetry library |
| "expose health check" | Health check library |
| "version the API" | API versioning library |
| "rate limit endpoints" | Rate limiting library |
| "real-time updates" | SignalR / WebSockets |
| "integrate with Azure / AWS / GCP service" | Cloud SDK |
| "run CQRS commands / queries" | Mediator library |

If a need does not match any signal above, describe it explicitly under `Other`.

---

### Step 2 — Scan Existing Solution Packages

For every `.csproj` in the solution, extract all `<PackageReference>` entries.

Build a deduplicated **Solution Package Inventory**:

| Package | Version | Projects |
|---|---|---|
| {PackageName} | {x.y.z} | {ProjectA, ProjectB} |

For each implied need from Step 1:
- If a package in the inventory satisfies it → mark `alreadyInProject: true`.
- If nothing satisfies it → proceed to Step 3 to recommend a new package.

---

### Step 3 — Evaluate and Recommend

For each unsatisfied need, evaluate candidates using this scoring rubric (higher = better):

| Factor | Weight | How to assess |
|---|---|---|
| Maintenance activity | 30% | NuGet download trend, GitHub commit recency, last release date |
| .NET version compatibility | 25% | Targets `net8.0` or `netstandard2.1` at minimum |
| Licence permissiveness | 20% | MIT / Apache-2.0 / BSD preferred; GPL / AGPL flagged |
| Bundle size & footprint | 15% | Prefer small, focused packages over monolithic SDKs |
| Existing project alignment | 10% | Consistent with patterns already used in the solution |

Produce a recommendation entry for every need:

```markdown
### DEP-{NNN}: {Implied Need Name}

**Recommended**: `{Package.Name}` ≥ {x.y.z}
**Licence**: {SPDX identifier}
**Purpose**: {one sentence — what it does in this feature context}
**Justification**: {why this package scores highest for this project}
**Security Notes**: {known CVEs, audit status, or "None known as of {date}"}

#### Alternatives Considered
| Package | Rejected Reason |
|---|---|
| {Alternative1} | {reason} |
| {Alternative2} | {reason} |

**Add to**: `{ProjectName}.csproj`
```

---

### Step 4 — Produce the Dependency Manifest

#### Manifest — Markdown (for RTD Section 9 and story `### Recommended NuGet Packages`)

```markdown
## Recommended Dependencies

**DEP-RECOMMENDATION**: PRESENT

| DEP-ID | Package | Min Version | Purpose | Justification | Alternatives Considered | Licence | Security Notes | Add to Project? |
|---|---|---|---|---|---|---|---|---|
| DEP-001 | {Package.Name} | ≥ {x.y.z} | {purpose} | {justification} | {alt (rejected: reason)} | {licence} | {notes or "None known"} | ✅ Already present / ❌ Add to {Project}.csproj |
```

If all needs are already satisfied:

```markdown
**DEP-RECOMMENDATION**: PRESENT — No new packages required.
All implied needs are satisfied by existing solution dependencies (see Step 2 inventory).
```

#### Manifest — JSON (for `rtd.json` → `depRecommendations` array)

```json
"depRecommendations": [
  {
    "id": "DEP-001",
    "package": "{Package.Name}",
    "minimumVersion": "{x.y.z}",
    "purpose": "...",
    "justification": "...",
    "alternativesConsidered": ["{Alternative} (rejected: {reason})"],
    "licence": "{MIT|Apache-2.0|...}",
    "securityNotes": null,
    "alreadyInProject": false,
    "addToProject": "{ProjectName}.csproj"
  }
]
```

---

### Step 5 — Output Location

| Artifact | Path | Action |
|---|---|---|
| Manifest Markdown | `docs/requirements/{epic-name}/rtd.md` → Section 9 | Append / update |
| Manifest JSON | `docs/requirements/{epic-name}/rtd.json` → `depRecommendations` | Append / update |
| Story dependency block | `docs/requirements/{epic-name}/US-{id}.md` → `### Recommended NuGet Packages` | Update each affected story |

If the RTD files do not yet exist, write the manifest as a standalone file:
`docs/requirements/{epic-name}/dependency-manifest.md`

---

## .NET Package Reference Catalogue

Use this catalogue as the primary recommendation source. Prefer packages already used in the solution before introducing new ones.

### Authentication & Identity
| Need | Preferred Package | Version Floor | Licence |
|---|---|---|---|
| ASP.NET Core Identity (EF) | `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | ≥ 8.0.0 | MIT |
| JWT Bearer authentication | `Microsoft.AspNetCore.Authentication.JwtBearer` | ≥ 8.0.0 | MIT |
| Azure AD / Entra ID | `Microsoft.Identity.Web` | ≥ 3.0.0 | MIT |
| OpenID Connect | `Microsoft.AspNetCore.Authentication.OpenIdConnect` | ≥ 8.0.0 | MIT |

### Data Access
| Need | Preferred Package | Version Floor | Licence |
|---|---|---|---|
| EF Core (SQL Server) | `Microsoft.EntityFrameworkCore.SqlServer` | ≥ 8.0.0 | MIT |
| EF Core (SQLite) | `Microsoft.EntityFrameworkCore.Sqlite` | ≥ 8.0.0 | MIT |
| EF Core (PostgreSQL) | `Npgsql.EntityFrameworkCore.PostgreSQL` | ≥ 8.0.0 | PostgreSQL |
| Micro-ORM | `Dapper` | ≥ 2.1.0 | Apache-2.0 |

### CQRS & Validation
| Need | Preferred Package | Version Floor | Licence |
|---|---|---|---|
| CQRS Mediator | `MediatR` | ≥ 12.0.0 | Apache-2.0 |
| Input validation | `FluentValidation` | ≥ 11.0.0 | Apache-2.0 |
| MediatR + FluentValidation pipeline | `FluentValidation.DependencyInjectionExtensions` | ≥ 11.0.0 | Apache-2.0 |

### Messaging & Email
| Need | Preferred Package | Version Floor | Licence |
|---|---|---|---|
| SMTP / IMAP email | `MailKit` | ≥ 4.3.0 | MIT |
| Transactional email (cloud) | `SendGrid` | ≥ 9.28.0 | MIT |
| Azure email | `Azure.Communication.Email` | ≥ 1.0.0 | MIT |

### File & Blob Storage
| Need | Preferred Package | Version Floor | Licence |
|---|---|---|---|
| Azure Blob Storage | `Azure.Storage.Blobs` | ≥ 12.0.0 | MIT |
| AWS S3 | `AWSSDK.S3` | ≥ 3.7.0 | Apache-2.0 |
| MinIO | `Minio` | ≥ 6.0.0 | Apache-2.0 |

### Caching
| Need | Preferred Package | Version Floor | Licence |
|---|---|---|---|
| In-memory cache | `Microsoft.Extensions.Caching.Memory` (built-in) | ≥ 8.0.0 | MIT |
| Redis distributed cache | `StackExchange.Redis` | ≥ 2.7.0 | MIT |
| Redis with IDistributedCache | `Microsoft.Extensions.Caching.StackExchangeRedis` | ≥ 8.0.0 | MIT |

### Background Jobs & Scheduling
| Need | Preferred Package | Version Floor | Licence |
|---|---|---|---|
| Simple background jobs | Native `IHostedService` / `BackgroundService` (built-in) | — | MIT |
| Persistent job queue | `Hangfire.Core` + `Hangfire.SqlServer` or `Hangfire.InMemory` | ≥ 1.8.0 | LGPL-3.0 (free tier) |
| Advanced scheduling | `Quartz.NET` | ≥ 3.8.0 | Apache-2.0 |

### HTTP & API
| Need | Preferred Package | Version Floor | Licence |
|---|---|---|---|
| Typed HTTP client | Native `IHttpClientFactory` + `HttpClient` (built-in) | — | MIT |
| REST client generation | `Refit` | ≥ 7.0.0 | MIT |
| REST client (manual) | `RestSharp` | ≥ 110.0.0 | Apache-2.0 |
| OpenAPI / Swagger | `Swashbuckle.AspNetCore` | ≥ 6.5.0 | MIT |
| API versioning | `Asp.Versioning.Mvc` | ≥ 8.0.0 | MIT |
| Rate limiting | Built-in `RateLimiter` (.NET 7+) | — | MIT |

### Documents & Reporting
| Need | Preferred Package | Version Floor | Licence |
|---|---|---|---|
| PDF generation | `QuestPDF` | ≥ 2024.3.0 | Community licence / MIT |
| Excel read/write | `ClosedXML` | ≥ 0.102.0 | MIT |
| CSV read/write | `CsvHelper` | ≥ 32.0.0 | MS-PL / Apache-2.0 |

### Logging & Telemetry
| Need | Preferred Package | Version Floor | Licence |
|---|---|---|---|
| Structured logging | `Serilog.AspNetCore` + sinks | ≥ 8.0.0 | Apache-2.0 |
| Application Insights | `Microsoft.ApplicationInsights.AspNetCore` | ≥ 2.22.0 | MIT |
| OpenTelemetry | `OpenTelemetry.Extensions.Hosting` | ≥ 1.7.0 | Apache-2.0 |

### Testing
| Need | Preferred Package | Version Floor | Licence |
|---|---|---|---|
| Unit test framework | `MSTest` | ≥ 3.0.0 | MIT |
| Assertions | `FluentAssertions` | ≥ 7.0.0 | Apache-2.0 |
| Mocking | `Moq` | ≥ 4.20.0 | BSD-3-Clause |
| Integration tests | `Microsoft.AspNetCore.Mvc.Testing` (built-in) | ≥ 8.0.0 | MIT |
| Browser E2E | `Microsoft.Playwright` | ≥ 1.40.0 | MIT |

---

## Security & Licence Flags

Always check for known flags before recommending a package:

| Flag | Action |
|---|---|
| GPL-2.0 / GPL-3.0 / AGPL-3.0 | Warn — copyleft licence; may require open-sourcing dependent code. Obtain legal approval. |
| Unmaintained (last release > 2 years) | Warn — evaluate actively maintained fork or alternative. |
| Known critical CVE | Block recommendation — choose alternative or note pending patch. |
| Commercial / proprietary licence | Warn — confirm licensing cost is approved. |

---

## Validation Checklist

Before declaring `DEP-RECOMMENDATION: PRESENT`:

- [ ] Every acceptance criterion signal has a corresponding DEP entry or an explicit "no package needed" note
- [ ] All existing solution packages are inventoried and checked for coverage
- [ ] Each new package has: version floor, justification, at least one alternative considered, licence identified
- [ ] No GPL / AGPL packages introduced without explicit flag
- [ ] No unmaintained packages (last release > 2 years) without explicit flag
- [ ] All `addToProject` fields reference the correct `.csproj` file(s)
- [ ] JSON manifest is valid (`depRecommendations` array in `rtd.json` is well-formed)
