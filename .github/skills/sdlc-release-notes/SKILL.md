---
name: sdlc-release-notes
description: 'Auto-generate release notes from conventional commits, linked PRs, and associated user stories for .NET project releases.'
---

# SDLC Release Notes Generator

## Primary Directive

Generate comprehensive release notes from Git history, merged PRs, and linked issues. Output follows a consistent template suitable for stakeholders, developers, and auditors.

## Trigger

Activate when:
- A release tag is created (`v*`)
- User requests release notes generation
- A deployment to staging/production is planned

## Data Collection

### Sources
1. **Git commits** since last release tag (conventional commit format)
2. **Merged PRs** since last release
3. **Linked issues** from PR descriptions (`Closes #N`, `Fixes #N`)
4. **Dependency changes** from lock file diff
5. **Migration files** added since last release

### Conventional Commit Parsing

| Prefix | Category | Icon |
|--------|----------|------|
| `feat:` | Features | 🚀 |
| `fix:` | Bug Fixes | 🐛 |
| `perf:` | Performance | ⚡ |
| `security:` or CVE mentions | Security | 🔒 |
| `docs:` | Documentation | 📝 |
| `refactor:` | Refactoring | ♻️ |
| `test:` | Tests | 🧪 |
| `chore:` | Maintenance | 🔧 |
| `BREAKING CHANGE:` | Breaking Changes | ⚠️ |

## Output Template

```markdown
# Release {version} — {YYYY-MM-DD}

## Highlights
{2-3 sentence summary of the most impactful changes}

## 🚀 Features
- **{scope}**: {description} (#{PR}) — {linked US/REQ}

## 🐛 Bug Fixes
- **{scope}**: {description} (#{PR}) — {linked issue}

## ⚡ Performance
- **{scope}**: {description} (#{PR})

## 🔒 Security
- {package} updated to {version} ({CVE-id})

## ⚠️ Breaking Changes
- {description of breaking change and migration steps}

## 📦 Dependency Updates
| Package | From | To | Reason |
|---------|------|----|--------|
| {name} | {old} | {new} | {CVE or feature} |

## 📋 Deployment Notes
- [ ] Run database migration: `dotnet ef database update`
- [ ] Update environment variable: `{VAR_NAME}`
- [ ] {Other deployment steps}

## ↩️ Rollback Procedure
1. {Step to rollback}
2. Verify rollback: {verification step}

## Contributors
@{contributor1}, @{contributor2}

## Full Changelog
{link to compare: previous_tag...current_tag}
```

## Semantic Versioning Rules

Determine version bump from commit types:

| Commit Type | Version Bump |
|-------------|-------------|
| `BREAKING CHANGE:` | Major (X.0.0) |
| `feat:` | Minor (0.X.0) |
| `fix:`, `perf:`, `security:` | Patch (0.0.X) |
| `docs:`, `chore:`, `test:` | No bump |

## Validation

Before publishing release notes:
- [ ] Version follows semantic versioning
- [ ] All merged PRs categorized
- [ ] Breaking changes have migration instructions
- [ ] Deployment notes are present if DB migrations or config changes exist
- [ ] Rollback procedure is documented
- [ ] Security updates reference CVE IDs
