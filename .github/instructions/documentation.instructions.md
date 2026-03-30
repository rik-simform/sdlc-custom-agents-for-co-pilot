---
applyTo: '**/*.md,**/*.cs'
---

# Documentation Standards for SDLC Agents

## XML Documentation (C#)

Required on all public APIs:

```csharp
/// <summary>
/// Brief description of the type/member.
/// </summary>
/// <param name="paramName">Description of the parameter.</param>
/// <returns>Description of the return value.</returns>
/// <exception cref="ExceptionType">When this exception is thrown.</exception>
/// <remarks>Additional context or usage notes.</remarks>
/// <example>
/// <code>
/// var result = await service.GetAsync(42);
/// </code>
/// </example>
```

## Markdown Documentation

### Required Documents

| Document | Location | Owner | Update Frequency |
|----------|----------|-------|-----------------|
| README.md | Root | Tech Lead | Per release |
| CONTRIBUTING.md | Root | Tech Lead | Quarterly |
| CHANGELOG.md | Root | Auto-generated | Per release |
| Architecture docs | docs/architecture/ | Architect | Per ADR |
| API docs | Auto-generated | CI pipeline | Per build |
| Onboarding guide | docs/guides/ | Tech Lead | Quarterly |
| Runbooks | docs/operations/ | DevOps | Quarterly |

### Markdown Conventions

- Use ATX-style headers (`#`, `##`, `###`)
- One sentence per line (for better diffs)
- Use tables for structured data
- Use code fences with language identifiers
- Use Mermaid for diagrams
- Use relative links for cross-references within the repo
- Keep line length ≤ 120 characters in prose

### Changelog Format (Keep a Changelog)

```markdown
## [Unreleased]

### Added
- New feature descriptions

### Changed
- Modified behavior descriptions

### Fixed
- Bug fix descriptions

### Security
- Security-related changes

### Deprecated
- Soon-to-be-removed features

### Removed
- Removed features
```

## Documentation Freshness

Documents have a maximum age before they are flagged as stale:

| Type | Max Age | Check |
|------|---------|-------|
| API Docs | Per release | Auto from build |
| Architecture | 90 days | Manual review |
| Runbooks | 90 days | Test execution |
| Onboarding | 60 days | New hire feedback |
| Security docs | 30 days | Scan date |
