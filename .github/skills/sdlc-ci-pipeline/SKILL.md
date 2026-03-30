---
name: sdlc-ci-pipeline
description: 'Generate and maintain CI/CD pipelines for .NET projects. Creates GitHub Actions workflows with build, test, security scanning, and deployment stages.'
---

# SDLC CI/CD Pipeline Generator

## Primary Directive

Generate production-ready CI/CD pipeline configurations for .NET projects using GitHub Actions. Pipelines include build, test, security scanning, and deployment stages with quality gates.

## Detection

Analyze the project to determine:

1. **Solution structure**: `*.sln` file location, project references
2. **Target framework**: From `*.csproj` `<TargetFramework>` element
3. **Test projects**: Projects ending in `.Tests` or `.UnitTests` / `.IntegrationTests`
4. **Docker**: Presence of `Dockerfile`
5. **Database**: EF Core migrations presence
6. **NuGet packages**: For library projects

## Pipeline Templates

### Standard Web API / ASP.NET Core

Generate `.github/workflows/ci.yml`:

```yaml
name: CI

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

permissions:
  contents: read
  checks: write
  pull-requests: write
  security-events: write

env:
  DOTNET_VERSION: '8.0.x'
  CONFIGURATION: Release

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Restore
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration ${{ env.CONFIGURATION }}

      - name: Unit Tests
        run: >
          dotnet test --no-build --configuration ${{ env.CONFIGURATION }}
          --filter "TestCategory!=Integration&TestCategory!=E2E"
          --collect:"XPlat Code Coverage"
          --logger "trx;LogFileName=unit-tests.trx"
          --results-directory ./TestResults

      - name: Publish Test Results
        uses: dorny/test-reporter@v1
        if: always()
        with:
          name: Unit Test Results
          path: '**/unit-tests.trx'
          reporter: dotnet-trx

      - name: Code Coverage
        uses: codecov/codecov-action@v4
        with:
          files: '**/coverage.cobertura.xml'

  security-scan:
    runs-on: ubuntu-latest
    needs: build-and-test
    steps:
      - uses: actions/checkout@v4

      - name: Initialize CodeQL
        uses: github/codeql-action/init@v3
        with:
          languages: csharp

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Build
        run: dotnet build --configuration ${{ env.CONFIGURATION }}

      - name: CodeQL Analysis
        uses: github/codeql-action/analyze@v3

  dependency-review:
    runs-on: ubuntu-latest
    if: github.event_name == 'pull_request'
    steps:
      - uses: actions/checkout@v4
      - uses: actions/dependency-review-action@v4
        with:
          fail-on-severity: high
```

### CD Pipeline (Tag-based Release)

Generate `.github/workflows/cd.yml`:

```yaml
name: CD

on:
  push:
    tags: ['v*']

permissions:
  contents: write
  packages: write

env:
  DOTNET_VERSION: '8.0.x'

jobs:
  release:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Build
        run: dotnet publish -c Release -o ./publish

      - name: Generate Release Notes
        id: release_notes
        run: |
          git log $(git describe --tags --abbrev=0 HEAD^)..HEAD --pretty=format:"- %s" > release_notes.txt

      - name: Create Release
        uses: softprops/action-gh-release@v2
        with:
          body_path: release_notes.txt
          files: ./publish/**
```

## Quality Gates (Enforced in Pipeline)

| Gate | Enforcement | Failure Action |
|------|------------|----------------|
| Build | Zero errors, zero warnings | Block merge |
| Unit Tests | 100% pass | Block merge |
| Coverage | ≥ 80% | Warning (configurable to block) |
| Security | Zero critical CodeQL findings | Block merge |
| Dependencies | No high/critical vulnerabilities | Block merge |
| Integration Tests | 100% pass | Block deploy to staging |
| Smoke Tests | 100% pass | Block deploy to production |

## Customization

Adjust pipeline via `.github/sdlc-config.json`:

```json
{
  "ci": {
    "dotnetVersion": "8.0.x",
    "configuration": "Release",
    "coverageThreshold": 80,
    "codeqlEnabled": true,
    "dependencyReviewEnabled": true,
    "integrationTestsEnabled": true
  }
}
```
