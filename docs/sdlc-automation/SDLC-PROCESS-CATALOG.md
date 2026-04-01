---
goal: SDLC Process Automation Catalog for .NET Projects
version: 1.0
date_created: 2026-03-26
last_updated: 2026-03-26
owner: SDLC Automation Team
status: 'Planned'
tags: [sdlc, automation, dotnet, process, governance, agents]
---

# SDLC Process Automation Catalog

![Status: Planned](https://img.shields.io/badge/status-Planned-blue)

A comprehensive, agent-driven Software Development Lifecycle (SDLC) process catalog designed for .NET Core and ASP.NET projects. Each process is defined with automation hooks, acceptance criteria, and minimal viable safeguards suitable for GitHub Copilot custom agents.

---

## Executive Summary

This catalog defines **15 SDLC processes** with full traceability from requirements through deployment and maintenance. Each process is structured for autonomous execution by GitHub Copilot custom agents while retaining human oversight gates at critical decision points.

**Target Audience**: Engineers, Tech Leads, Auditors, Compliance Officers
**Technology Scope**: .NET 6+, ASP.NET Core, Entity Framework Core, Azure DevOps/GitHub Actions
**Customization**: Each process supports domain-specific overrides via configuration files

---

## 1. Process Catalog

### PROC-001: Requirements Engineering

| Attribute | Detail |
|-----------|--------|
| **Description** | Elicit, analyze, validate, and manage functional and non-functional requirements throughout the project lifecycle. |
| **Key Activities** | Stakeholder interviews, user story creation, acceptance criteria definition, requirements traceability, backlog grooming, impact analysis |
| **Artifacts** | Product Requirements Document (PRD), User Stories, Acceptance Criteria, Requirements Traceability Matrix (RTM), Backlog Items |
| **Inputs** | Business objectives, stakeholder needs, market research, existing system documentation |
| **Outputs** | Validated requirements, prioritized backlog, RTM, Definition of Ready |
| **Roles** | Product Owner, Business Analyst, Tech Lead, Stakeholders |
| **Acceptance Criteria** | All requirements have unique IDs, acceptance criteria, priority, and at least one linked test case. RTM covers 100% of requirements. |

**Automation Hooks**:
- Agent auto-generates user story templates from PRD text
- Auto-links requirements to test cases in RTM
- Validates completeness (ID, AC, priority, estimate)

**Metrics**: Requirements coverage %, stories with AC %, RTM completeness
**Safeguards**: Human approval gate before requirements are marked "Ready"

<details>
<summary>Example Artifact: User Story</summary>

```markdown
## US-001: User Authentication via Azure AD

**As a** registered user
**I want to** authenticate using Azure AD SSO
**So that** I can securely access the application without managing separate credentials

### Acceptance Criteria
- [ ] AC-001: User can sign in via Azure AD redirect
- [ ] AC-002: Failed auth shows descriptive error message
- [ ] AC-003: Session expires after 30 minutes of inactivity
- [ ] AC-004: Refresh token support for seamless re-auth

**Priority**: High | **Estimate**: 5 SP | **Linked Tests**: TC-001, TC-002, TC-003
```
</details>

---

### PROC-002: Architecture & Design

| Attribute | Detail |
|-----------|--------|
| **Description** | Define system architecture, component design, data models, and integration patterns that satisfy requirements and NFRs. |
| **Key Activities** | Architecture Decision Records (ADRs), component diagrams, API contract design, data modeling, technology stack selection, NFR analysis, design reviews |
| **Artifacts** | Architecture Decision Records, System Context Diagram, Component Diagram, API Specifications (OpenAPI), Data Model (ERD), Deployment Diagram, Design Review Checklist |
| **Inputs** | Validated requirements, NFRs, technology constraints, existing architecture |
| **Outputs** | Approved architecture documentation, API contracts, data models, ADRs |
| **Roles** | Solution Architect, Tech Lead, Senior Developers, Security Engineer |
| **Acceptance Criteria** | All ADRs reviewed and approved. API contracts pass schema validation. Design covers all NFRs with documented trade-offs. |

**Automation Hooks**:
- Agent generates Mermaid diagrams from code structure
- Auto-validates OpenAPI specs against naming conventions
- Generates ADR templates from architectural decisions
- Detects architectural drift via dependency analysis

**Metrics**: ADR count, API contract coverage %, design review pass rate
**Safeguards**: Architect sign-off required before implementation begins

<details>
<summary>Example Artifact: Architecture Decision Record</summary>

```markdown
# ADR-001: Use Vertical Slice Architecture

## Status: Accepted

## Context
The project requires a maintainable architecture for a medium-complexity ASP.NET Core API with 15+ bounded contexts.

## Decision
Adopt Vertical Slice Architecture with MediatR for command/query separation.

## Consequences
- (+) Features are self-contained and independently deployable
- (+) Reduces cross-cutting coupling between features
- (-) Slight overhead for simple CRUD operations
- (-) Team requires training on the pattern

## Compliance
- REQ-NFR-001: Maintainability — Satisfied
- REQ-NFR-003: Testability — Satisfied
```
</details>

---

### PROC-003: Implementation (Coding)

| Attribute | Detail |
|-----------|--------|
| **Description** | Translate approved designs into production-quality code following established standards, patterns, and conventions. |
| **Key Activities** | Feature branch creation, code implementation, unit test authoring, code self-review, documentation updates, dependency management |
| **Artifacts** | Source code, unit tests, inline documentation (XML docs), changelogs, NuGet package references |
| **Inputs** | Approved design, user stories with AC, coding standards, API contracts |
| **Outputs** | Working code on feature branch, passing unit tests, updated documentation |
| **Roles** | Developers, Tech Lead |
| **Acceptance Criteria** | Code compiles, all unit tests pass, code coverage ≥ 80%, no critical/high static analysis findings, XML docs on public APIs. |

**Automation Hooks**:
- Agent scaffolds feature slices from user stories
- Auto-generates boilerplate (controllers, services, DTOs, validators)
- Enforces coding standards via .editorconfig and analyzers
- Auto-creates unit test stubs for new public methods

**Metrics**: Code coverage %, build success rate, lines of code per story point
**Safeguards**: Pre-commit hooks enforce formatting; CI blocks on test failure

<details>
<summary>Example Artifact: Controller Scaffold</summary>

```csharp
/// <summary>
/// Manages user authentication operations.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AuthController(IMediator mediator, ILogger<AuthController> logger) : ControllerBase
{
    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Error);
    }
}
```
</details>

---

### PROC-004: Code Review

| Attribute | Detail |
|-----------|--------|
| **Description** | Systematic examination of source code to identify defects, enforce standards, share knowledge, and improve code quality before merging. |
| **Key Activities** | Pull request creation, automated checks (lint, build, test), peer review, review feedback resolution, approval and merge |
| **Artifacts** | Pull Request, Review Comments, Approval Records, Merged Code |
| **Inputs** | Feature branch code, coding standards, design documentation, test results |
| **Outputs** | Approved and merged code, documented review decisions, improvement action items |
| **Roles** | Author (Developer), Reviewers (Peers), Tech Lead (Approver) |
| **Acceptance Criteria** | Minimum 1 peer approval, all CI checks green, no unresolved blocking comments, PR description links to user story. |

**Automation Hooks**:
- Agent auto-generates PR descriptions from commit messages and linked stories
- Automated code review for .NET best practices (nullable, async patterns, SOLID)
- Auto-labels PRs by scope (feature, bugfix, chore, docs)
- Auto-assigns reviewers based on CODEOWNERS

**Metrics**: PR review turnaround time, review comments per PR, defect escape rate
**Safeguards**: Branch protection rules enforce review requirements

<details>
<summary>Example Artifact: PR Template</summary>

```markdown
## Description
Implements US-001: User Authentication via Azure AD

## Changes
- Added `AuthController` with login/logout endpoints
- Implemented `LoginCommand` and `LoginCommandHandler`
- Added JWT token generation service
- Unit tests for auth flow (12 tests, 100% pass)

## Checklist
- [x] Code follows project coding standards
- [x] Unit tests added/updated
- [x] XML documentation on public APIs
- [x] No new analyzer warnings
- [x] Linked to user story: #42
```
</details>

---

### PROC-005: Testing & Quality Assurance

| Attribute | Detail |
|-----------|--------|
| **Description** | Verify and validate that the software meets requirements and quality standards through systematic testing at multiple levels. |
| **Key Activities** | Test planning, unit testing, integration testing, E2E testing, performance testing, security testing, exploratory testing, regression testing, test reporting |
| **Artifacts** | Test Plan, Test Cases, Test Results, Defect Reports, Coverage Reports, Performance Benchmarks |
| **Inputs** | Requirements with AC, design documentation, code under test, test data |
| **Outputs** | Test execution results, coverage reports, defect log, quality sign-off |
| **Roles** | QA Engineer, Developers, Performance Engineer, Security Tester |
| **Acceptance Criteria** | All critical/high test cases pass, code coverage ≥ 80%, zero critical defects, performance within NFR thresholds, OWASP Top 10 scan clean. |

**Automation Hooks**:
- Agent generates test cases from acceptance criteria
- Auto-runs test suites on PR and reports results
- Generates coverage trend reports
- Auto-creates defect issues from failed test runs
- Runs OWASP ZAP scan and maps findings to issues

**Metrics**: Test pass rate, code coverage %, defect density, mean time to detect
**Safeguards**: Quality gate blocks release if critical tests fail

<details>
<summary>Example Artifact: Test Case</summary>

```markdown
## TC-001: Successful Azure AD Login

**Linked Requirement**: US-001 / AC-001
**Priority**: Critical
**Type**: Integration

### Preconditions
- Azure AD tenant configured
- Test user exists with valid credentials

### Steps
1. Navigate to /api/v1/auth/login
2. POST with valid Azure AD token
3. Verify response status 200
4. Verify JWT token in response body
5. Verify token contains expected claims (sub, email, roles)

### Expected Result
- HTTP 200 with valid JWT token
- Token expires in 30 minutes
- Refresh token included in response

### Automation
```csharp
[TestMethod]
public async Task Login_WithValidAzureAdToken_ReturnsJwt()
{
    // Arrange
    var command = new LoginCommand { Token = _validAzureAdToken };

    // Act
    var response = await _client.PostAsJsonAsync("/api/v1/auth/login", command);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
    result.AccessToken.Should().NotBeNullOrEmpty();
    result.ExpiresIn.Should().Be(1800);
}
```
</details>

---

### PROC-006: Continuous Integration (CI)

| Attribute | Detail |
|-----------|--------|
| **Description** | Automatically build, test, and validate code changes on every commit to detect issues early and maintain a releasable codebase. |
| **Key Activities** | Build automation, automated test execution, static analysis, dependency scanning, artifact generation, build notifications |
| **Artifacts** | Build Logs, Test Reports, Static Analysis Reports, Build Artifacts (.nupkg, Docker images), Dependency Audit Reports |
| **Inputs** | Source code commits, build configuration, test suites, analyzer rulesets |
| **Outputs** | Build status (pass/fail), test results, analysis reports, deployable artifacts |
| **Roles** | DevOps Engineer, Developers, Tech Lead |
| **Acceptance Criteria** | Build completes in < 10 minutes, all tests pass, no critical analyzer findings, dependency vulnerabilities resolved or documented. |

**Automation Hooks**:
- GitHub Actions / Azure Pipelines YAML auto-generated from project structure
- Agent detects new projects and updates CI pipeline
- Auto-triage build failures with root-cause suggestions
- Dependency vulnerability alerts auto-create issues

**Metrics**: Build success rate, build duration, time to fix broken build
**Safeguards**: CI pipeline is immutable; changes require PR review

<details>
<summary>Example Artifact: CI Pipeline (GitHub Actions)</summary>

```yaml
name: CI
on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - run: dotnet restore
      - run: dotnet build --no-restore --configuration Release
      - run: dotnet test --no-build --configuration Release --collect:"XPlat Code Coverage"
      - uses: codecov/codecov-action@v4
```
</details>

---

### PROC-007: Continuous Deployment (CD) & Release Management

| Attribute | Detail |
|-----------|--------|
| **Description** | Automate the packaging, versioning, and deployment of validated builds to target environments with controlled release gates. |
| **Key Activities** | Environment provisioning, deployment automation, blue-green/canary deployments, rollback procedures, release notes generation, version management |
| **Artifacts** | Deployment Manifests, Release Notes, Runbooks, Environment Configuration, Rollback Plan |
| **Inputs** | Validated build artifacts, environment configs, release approval, deployment scripts |
| **Outputs** | Deployed application, release notes, deployment verification results |
| **Roles** | DevOps Engineer, Release Manager, Tech Lead, Operations |
| **Acceptance Criteria** | Zero-downtime deployment, smoke tests pass in target environment, rollback tested and documented, release notes published. |

**Automation Hooks**:
- Agent generates release notes from merged PRs and linked stories
- Auto-increments semantic version based on conventional commits
- Deployment manifests generated from environment templates
- Post-deploy smoke test execution and result reporting
- Auto-rollback on health check failure

**Metrics**: Deployment frequency, lead time for changes, deployment failure rate, MTTR
**Safeguards**: Manual approval gate for production deployments

<details>
<summary>Example Artifact: Release Notes</summary>

```markdown
# Release v2.3.0 — 2026-03-26

## Features
- **US-001**: User Authentication via Azure AD (#42)
- **US-015**: Dashboard performance optimization (#78)

## Bug Fixes
- **BUG-034**: Fixed null reference in payment processing (#91)

## Breaking Changes
- None

## Deployment Notes
- Run database migration: `dotnet ef database update`
- Update Azure AD app registration redirect URI

## Rollback
- Revert to v2.2.1: `kubectl rollout undo deployment/api --to-revision=5`
```
</details>

---

### PROC-008: Configuration Management

| Attribute | Detail |
|-----------|--------|
| **Description** | Control and track changes to all configuration items (code, infrastructure, settings, dependencies) throughout the lifecycle. |
| **Key Activities** | Version control management, branching strategy, environment configuration, secrets management, infrastructure-as-code, dependency tracking |
| **Artifacts** | Git Repository, Branch Strategy Document, Environment Config Files, Secrets Vault References, IaC Templates (Bicep/Terraform) |
| **Inputs** | Source code, configuration files, infrastructure definitions, dependency manifests |
| **Outputs** | Version-controlled artifacts, auditable change history, reproducible environments |
| **Roles** | DevOps Engineer, Developers, Infrastructure Engineer |
| **Acceptance Criteria** | All config changes tracked in VCS, secrets never in plain text, environments reproducible from IaC, branching strategy documented and enforced. |

**Automation Hooks**:
- Agent detects hardcoded secrets and flags for remediation
- Auto-validates appsettings.json schema on commit
- Generates environment diff reports between stages
- IaC drift detection and alerting

**Metrics**: Configuration drift incidents, secret exposure events, environment parity score
**Safeguards**: Secret scanning blocks commits containing credentials

<details>
<summary>Example Artifact: appsettings Structure</summary>

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "#{DATABASE_CONNECTION_STRING}#"
  },
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "#{AZURE_AD_TENANT_ID}#",
    "ClientId": "#{AZURE_AD_CLIENT_ID}#"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```
</details>

---

### PROC-009: Security Engineering

| Attribute | Detail |
|-----------|--------|
| **Description** | Integrate security practices throughout the SDLC to identify, mitigate, and prevent vulnerabilities from design through operations. |
| **Key Activities** | Threat modeling, SAST/DAST scanning, dependency vulnerability scanning, penetration testing, security code review, security architecture review, incident response planning |
| **Artifacts** | Threat Model, Security Scan Reports, Penetration Test Report, Security Architecture Review, Vulnerability Register, Incident Response Plan |
| **Inputs** | Architecture documentation, source code, deployment configuration, compliance requirements |
| **Outputs** | Remediated vulnerabilities, security sign-off, compliance evidence, security posture report |
| **Roles** | Security Engineer, Developers, DevOps Engineer, Compliance Officer |
| **Acceptance Criteria** | Zero critical/high SAST findings in release, OWASP Top 10 mitigated, dependency vulnerabilities patched or risk-accepted, threat model current. |

**Automation Hooks**:
- SAST scan (Roslyn analyzers, CodeQL) on every PR
- Dependency vulnerability scan (Dependabot/Snyk) on schedule
- Agent generates threat model from architecture diagrams
- Auto-creates security issues from scan findings with severity
- DAST scan against staging environment on release candidate

**Metrics**: Vulnerability count by severity, mean time to remediate, false positive rate
**Safeguards**: Security scan failure blocks merge; critical findings require security team review

<details>
<summary>Example Artifact: Threat Model Entry</summary>

```markdown
## THREAT-001: SQL Injection via Search Endpoint

**STRIDE Category**: Tampering
**Component**: SearchController.Query()
**Attack Vector**: Malicious SQL in search parameter
**Impact**: High — Data exfiltration, data modification
**Likelihood**: Medium — Endpoint is authenticated but accepts free-text input

### Mitigations
- [x] Use parameterized queries via Entity Framework Core
- [x] Input validation with FluentValidation (max length, allowed characters)
- [x] WAF rules for SQL injection patterns
- [ ] Rate limiting on search endpoint

**Residual Risk**: Low (after mitigations)
```
</details>

---

### PROC-010: Compliance & Governance

| Attribute | Detail |
|-----------|--------|
| **Description** | Ensure the software development process and deliverables comply with regulatory, legal, and organizational standards. |
| **Key Activities** | Compliance mapping, audit trail maintenance, policy enforcement, license management, data privacy assessment, SOC2/ISO27001 evidence collection |
| **Artifacts** | Compliance Matrix, Audit Logs, License Inventory, Privacy Impact Assessment, Policy Documents, Evidence Collection Binder |
| **Inputs** | Regulatory requirements, organizational policies, industry standards, audit findings |
| **Outputs** | Compliance evidence, audit-ready documentation, remediation plans, compliance dashboard |
| **Roles** | Compliance Officer, Legal, Tech Lead |
| **Acceptance Criteria** | All applicable regulations mapped, audit trail complete, open-source licenses compatible, PIA completed for PII handling. |

**Automation Hooks**:
- Agent scans NuGet packages for license compatibility
- Auto-generates license inventory from .csproj files
- Compliance checklist auto-populated from process execution evidence
- Audit trail auto-generated from Git history, CI/CD logs, PR reviews

**Metrics**: Compliance gap count, audit finding resolution time, license violation count
**Safeguards**: License-incompatible packages blocked at PR level

<details>
<summary>Example Artifact: License Inventory</summary>

```markdown
| Package | Version | License | Compatible | Notes |
|---------|---------|---------|------------|-------|
| MediatR | 12.2.0 | Apache-2.0 | ✅ | |
| FluentValidation | 11.9.0 | Apache-2.0 | ✅ | |
| Newtonsoft.Json | 13.0.3 | MIT | ✅ | |
| SomePkg | 1.0.0 | GPL-3.0 | ❌ | Requires legal review |
```
</details>

---

### PROC-011: Environment Management

| Attribute | Detail |
|-----------|--------|
| **Description** | Provision, configure, and maintain consistent development, testing, staging, and production environments. |
| **Key Activities** | Environment provisioning, infrastructure-as-code, environment monitoring, capacity planning, environment refresh, access control |
| **Artifacts** | IaC Templates (Bicep/Terraform), Environment Inventory, Access Control Matrix, Monitoring Dashboards, Capacity Plans |
| **Inputs** | Architecture requirements, NFRs, security policies, deployment manifests |
| **Outputs** | Provisioned environments, monitoring alerts, environment health reports |
| **Roles** | DevOps Engineer, Infrastructure Engineer, Security Engineer |
| **Acceptance Criteria** | All environments provisioned from IaC (no manual changes), monitoring covers all critical paths, environments match production parity rules. |

**Automation Hooks**:
- Agent generates Bicep/Terraform from architecture diagrams
- Auto-provisions PR-specific review environments
- Environment drift detection and auto-remediation
- Health check monitoring with auto-alerting

**Metrics**: Environment provisioning time, drift incidents, uptime per environment
**Safeguards**: Production changes require IaC PR review and approval

<details>
<summary>Example Artifact: Bicep Template (App Service)</summary>

```bicep
param appName string
param location string = resourceGroup().location
param sku string = 'S1'

resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: '${appName}-plan'
  location: location
  sku: { name: sku, tier: 'Standard' }
  kind: 'linux'
  properties: { reserved: true }
}

resource webApp 'Microsoft.Web/sites@2023-01-01' = {
  name: appName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      alwaysOn: true
      healthCheckPath: '/health'
    }
  }
}
```
</details>

---

### PROC-012: Monitoring & Observability

| Attribute | Detail |
|-----------|--------|
| **Description** | Implement and maintain monitoring, logging, and alerting to ensure application health, performance, and rapid incident detection. |
| **Key Activities** | Instrumentation, structured logging, metrics collection, distributed tracing, alerting configuration, dashboard creation, SLO/SLI definition |
| **Artifacts** | Monitoring Configuration, Alert Rules, Dashboards, SLO Definitions, Runbooks, Incident Reports |
| **Inputs** | NFRs, architecture documentation, deployment configuration, SLA requirements |
| **Outputs** | Real-time dashboards, alert notifications, performance reports, incident timelines |
| **Roles** | SRE / DevOps Engineer, Developers, Operations |
| **Acceptance Criteria** | All critical paths have health checks, structured logging on all services, alerting covers SLO breaches, dashboards for all environments. |

**Automation Hooks**:
- Agent generates OpenTelemetry instrumentation for new services
- Auto-creates Application Insights dashboards from endpoint inventory
- Alert rules auto-generated from SLO definitions
- Runbook templates generated from alert types

**Metrics**: MTTD (mean time to detect), MTTR (mean time to recover), alert noise ratio, SLO compliance %
**Safeguards**: Alert fatigue monitoring; suppress noisy alerts automatically

<details>
<summary>Example Artifact: Health Check</summary>

```csharp
public static class HealthCheckExtensions
{
    public static IServiceCollection AddApplicationHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>("database")
            .AddAzureBlobStorage(name: "blob-storage")
            .AddRedis(name: "redis-cache")
            .AddCheck<ExternalApiHealthCheck>("payment-api");

        return services;
    }
}
```
</details>

---

### PROC-013: Maintenance & Support

| Attribute | Detail |
|-----------|--------|
| **Description** | Provide ongoing support, defect resolution, performance optimization, and technical debt management for production systems. |
| **Key Activities** | Incident management, bug triage, hotfix process, technical debt tracking, dependency updates, performance tuning, knowledge base maintenance |
| **Artifacts** | Incident Reports, Hotfix PRs, Tech Debt Register, Dependency Update Log, Knowledge Base Articles, Post-Mortem Documents |
| **Inputs** | Production incidents, user feedback, monitoring alerts, dependency vulnerability reports |
| **Outputs** | Resolved incidents, updated dependencies, reduced tech debt, improved documentation |
| **Roles** | Support Engineer, Developers, DevOps Engineer, Product Owner |
| **Acceptance Criteria** | Critical incidents resolved within SLA, tech debt sprint allocation ≥ 20%, dependencies updated within 30 days of security advisory. |

**Automation Hooks**:
- Agent auto-triages issues from error logs (severity, component, likely cause)
- Dependabot PRs auto-created for outdated dependencies
- Tech debt score calculated from code analysis metrics
- Post-mortem template auto-populated from incident timeline

**Metrics**: MTTR, incident count by severity, tech debt trend, dependency freshness score
**Safeguards**: Hotfix requires expedited review; post-mortem required for P1/P2 incidents

<details>
<summary>Example Artifact: Post-Mortem</summary>

```markdown
# Post-Mortem: Payment Processing Outage — 2026-03-20

## Summary
Payment processing failed for 47 minutes due to expired SSL certificate on payment gateway integration.

## Timeline
- 14:22 UTC — Monitoring alert: Payment API 5xx spike
- 14:25 UTC — On-call engineer acknowledged
- 14:32 UTC — Root cause identified: expired client certificate
- 14:45 UTC — Certificate renewed and deployed
- 15:09 UTC — Full recovery confirmed

## Root Cause
Client certificate for payment gateway expired. Certificate rotation was not automated.

## Action Items
- [ ] Implement automated certificate rotation (Owner: DevOps, Due: 2026-04-03)
- [ ] Add certificate expiry monitoring with 30-day warning (Owner: SRE, Due: 2026-04-01)
- [ ] Runbook for manual certificate rotation (Owner: DevOps, Due: 2026-03-28)
```
</details>

---

### PROC-014: Documentation Management

| Attribute | Detail |
|-----------|--------|
| **Description** | Create, maintain, and govern all project documentation to ensure accuracy, accessibility, and traceability throughout the lifecycle. |
| **Key Activities** | Technical writing, API documentation generation, architecture documentation, runbook creation, knowledge base management, documentation reviews |
| **Artifacts** | API Documentation, Architecture Guides, Runbooks, Developer Onboarding Guide, Knowledge Base, Changelog |
| **Inputs** | Source code, architecture decisions, operational procedures, team knowledge |
| **Outputs** | Published documentation, updated knowledge base, onboarding materials |
| **Roles** | Technical Writer, Developers, Architect, DevOps Engineer |
| **Acceptance Criteria** | API docs auto-generated and current, runbooks tested quarterly, onboarding guide enables new dev setup in < 1 day, docs reviewed per release. |

**Automation Hooks**:
- Agent generates API docs from OpenAPI specs and XML comments
- Auto-detects stale documentation from code change history
- README generation from project structure analysis
- Changelog auto-generated from conventional commits

**Metrics**: Documentation coverage %, docs freshness score, onboarding time for new developers
**Safeguards**: Documentation review required in PR checklist for architectural changes

<details>
<summary>Example Artifact: Auto-Generated Changelog</summary>

```markdown
# Changelog

## [2.3.0] — 2026-03-26
### Added
- User authentication via Azure AD (US-001)
- Role-based authorization middleware (US-002)

### Changed
- Improved dashboard query performance by 40% (US-015)

### Fixed
- Null reference in payment processing (#91)

### Security
- Updated System.Text.Json to 8.0.3 (CVE-2026-1234)
```
</details>

---

### PROC-015: Research & Analysis

| Attribute | Detail |
|-----------|--------|
| **Description** | Conduct technology research, feasibility studies, proof-of-concept evaluation, and pattern analysis to inform architecture and implementation decisions. |
| **Key Activities** | Technology evaluation, alternatives analysis, PoC development, pattern research, benchmark comparison, framework assessment, migration feasibility |
| **Artifacts** | Research Report, Alternatives Comparison Matrix, PoC Results, Feasibility Assessment, Recommendation Document |
| **Inputs** | Feature requirements, architectural constraints, technology landscape, team skills inventory |
| **Outputs** | Validated recommendations, PoC code (if applicable), risk assessment, adoption roadmap |
| **Roles** | Tech Lead, Architect, Senior Developer, Research Analyst |
| **Acceptance Criteria** | Report includes ≥ 2 alternatives with pros/cons, clear recommendation with rationale, risk assessment, and alignment to project constraints. |

**Automation Hooks**:
- Agent researches .NET ecosystem (NuGet packages, framework features, architecture patterns)
- Generates structured comparison matrices with weighted scoring
- Evaluates PoC feasibility against project constraints
- Cross-references with existing ADRs to avoid conflicting decisions

**Metrics**: Research turnaround time, recommendation adoption rate, PoC success rate
**Safeguards**: Architect review required before PoC implementation, time-boxed research phases

<details>
<summary>Example Artifact: Technology Research Report</summary>

```markdown
# Research Report: Authentication Strategy for .NET 8

## Summary
Evaluated 3 authentication approaches for the project's requirements.

## Alternatives
| Criteria | JWT Custom | Azure AD B2C | Duende IdentityServer |
|----------|-----------|-------------|----------------------|
| Complexity | Low | Medium | High |
| Cost | Free | Pay-per-auth | License fee |
| Scalability | High | Very High | High |
| .NET 8 Support | Native | SDK available | Full support |

## Recommendation
JWT Custom — best fit for current scale and team expertise.

## Risks
- Custom implementation requires thorough security review
- Token rotation must be implemented manually
```
</details>

### RACI Matrix

| Process | Responsible | Accountable | Consulted | Informed |
|---------|------------|-------------|-----------|----------|
| Requirements | Business Analyst | Product Owner | Stakeholders | Dev Team |
| Architecture | Solution Architect | Tech Lead | Security Engineer | Dev Team |
| Implementation | Developers | Tech Lead | Architect | PM |
| Code Review | Reviewers | Tech Lead | Author | PM |
| Testing/QA | QA Engineer | QA Lead | Developers | PM, PO |
| CI | DevOps Engineer | Tech Lead | Developers | PM |
| CD/Release | DevOps Engineer | Release Manager | Tech Lead | Stakeholders |
| Configuration | DevOps Engineer | Tech Lead | Security Engineer | Dev Team |
| Security | Security Engineer | CISO/Security Lead | Architect | All |
| Compliance | Compliance Officer | Legal Lead | Tech Lead | Stakeholders |
| Research & Analysis | Research Analyst | Tech Lead | Architect | Dev Team |
| Project Mgmt | Scrum Master | PM | PO, Tech Lead | Stakeholders |
| Risk Mgmt | PM | Steering Committee | Tech Lead | Stakeholders |
| Environment | DevOps Engineer | Infrastructure Lead | Architect | Dev Team |
| Monitoring | SRE/DevOps | Operations Lead | Developers | PM |
| Maintenance | Support Engineer | Tech Lead | DevOps | PM, PO |
| Documentation | Tech Writer | Tech Lead | Developers | All |

### Process Maturity Levels

| Level | Description | Criteria |
|-------|-------------|----------|
| **L1 — Initial** | Ad hoc, manual execution | Process exists informally |
| **L2 — Defined** | Documented and repeatable | Written procedures, templates available |
| **L3 — Managed** | Measured and controlled | Metrics collected, quality gates enforced |
| **L4 — Optimized** | Continuously improved via automation | Agent-driven, self-healing, data-driven decisions |

---

## 3. Quality & Risk Controls

### Quality Gates

| Gate | Process | Criteria | Enforcement |
|------|---------|----------|-------------|
| QG-001 | Requirements | All stories have AC + linked tests | Agent validates before "Ready" |
| QG-002 | Design | ADR approved, API contract valid | Architect sign-off |
| QG-003 | Implementation | Build passes, coverage ≥ 80% | CI pipeline |
| QG-004 | Code Review | 1+ approval, no blocking comments | Branch protection |
| QG-005 | Testing | All critical tests pass, 0 critical bugs | QA sign-off |
| QG-006 | Security | SAST/DAST clean, threat model current | Security gate in CI |
| QG-007 | Release | Smoke tests pass, rollback tested | Release manager approval |
| QG-008 | Compliance | License check pass, audit trail complete | Compliance gate |

### Risk Categories

| Category | Description | Monitoring |
|----------|-------------|------------|
| Technical | Architecture, performance, scalability | Code metrics, load tests |
| Security | Vulnerabilities, data breaches | SAST/DAST scans, pen tests |
| Schedule | Delays, scope creep | Velocity, burndown |
| Resource | Team capacity, skill gaps | Availability tracking |
| External | Third-party dependencies, vendor risks | Dependency monitoring |
| Compliance | Regulatory changes, audit findings | Compliance dashboard |

---

## 4. Tooling & Automation Opportunities

| Process | Primary Tools | Automation Level | Agent Opportunity |
|---------|--------------|------------------|-------------------|
| Requirements | GitHub Issues, Azure Boards | L2 → L3 | Story generation, RTM auto-linking |
| Architecture | Mermaid, Draw.io, ADR tools | L2 → L3 | Diagram generation, drift detection |
| Implementation | VS Code, GitHub Copilot, .NET CLI | L3 → L4 | Scaffold generation, code completion |
| Code Review | GitHub PRs, CodeQL | L3 → L4 | Auto-review, PR description generation |
| Testing | xUnit/MSTest, Playwright, k6 | L3 → L4 | Test generation, coverage reporting |
| CI | GitHub Actions, Azure Pipelines | L3 → L4 | Pipeline generation, failure triage |
| CD/Release | GitHub Actions, Helm, Bicep | L3 → L4 | Release notes, version management |
| Configuration | Git, Azure Key Vault, Bicep | L2 → L3 | Secret detection, config validation |
| Security | CodeQL, Dependabot, OWASP ZAP | L3 → L4 | Threat modeling, finding triage |
| Compliance | GitHub Audit Log, SBOM tools | L2 → L3 | License scanning, evidence collection |
| Project Mgmt | GitHub Projects, Azure Boards | L2 → L3 | Report generation, metric calculation |
| Risk Mgmt | Risk Register (GitHub Issues) | L2 → L3 | Auto-identification, scoring |
| Environment | Bicep/Terraform, Azure | L3 → L4 | IaC generation, drift detection |
| Monitoring | App Insights, OpenTelemetry | L2 → L3 | Instrumentation, alert generation |
| Maintenance | GitHub Issues, Dependabot | L2 → L3 | Auto-triage, dependency updates |
| Documentation | DocFX, Swagger, GitHub Pages | L2 → L3 | API doc generation, staleness detection |
| Research & Analysis | Web search, NuGet, GitHub | L2 → L3 | Technology evaluation, alternatives comparison |

---

## 5. Traceability Matrix

| Requirement → | Design → | Code → | Test → | Deployment → | Monitoring |
|---------------|----------|--------|--------|--------------|------------|
| US-{id} | ADR-{id}, API-{id} | PR-{id}, Commit-{hash} | TC-{id} | Release-{ver} | Alert-{id} |

### Change Control Process

1. **Request**: Change requested via GitHub Issue with impact analysis
2. **Assess**: Tech Lead assesses scope, risk, and effort
3. **Approve**: Change board (PO + Tech Lead + Architect) approves/rejects
4. **Implement**: Standard SDLC flow (branch → code → review → test → deploy)
5. **Verify**: Post-deployment verification against change objectives
6. **Close**: Change issue closed with evidence of successful implementation

---

## 6. Glossary

| Term | Definition |
|------|-----------|
| **AC** | Acceptance Criteria — conditions that must be met for a story to be considered complete |
| **ADR** | Architecture Decision Record — documented architectural decision with context and consequences |
| **CI/CD** | Continuous Integration / Continuous Deployment |
| **DAST** | Dynamic Application Security Testing |
| **IaC** | Infrastructure as Code |
| **MTTR** | Mean Time to Recovery |
| **MTTD** | Mean Time to Detect |
| **NFR** | Non-Functional Requirement |
| **PIA** | Privacy Impact Assessment |
| **PRD** | Product Requirements Document |
| **RTM** | Requirements Traceability Matrix |
| **SAST** | Static Application Security Testing |
| **SBOM** | Software Bill of Materials |
| **SLI/SLO** | Service Level Indicator / Objective |
| **SP** | Story Points |

---

## 7. References

- [Microsoft .NET Architecture Guides](https://learn.microsoft.com/en-us/dotnet/architecture/)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Azure Well-Architected Framework](https://learn.microsoft.com/en-us/azure/well-architected/)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)
