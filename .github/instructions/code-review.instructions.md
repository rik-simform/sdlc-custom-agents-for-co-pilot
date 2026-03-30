---
applyTo: '**/*.cs,**/*.md'
---

# Code Review Standards for SDLC Agents

## PR Requirements

Every pull request must:
1. Link to a GitHub Issue or user story
2. Use the PR template
3. Have a descriptive title following conventional commits
4. Pass all CI checks before review
5. Have at least 1 peer approval

## PR Title Convention

```
{type}({scope}): {description}

feat(auth): implement Azure AD SSO login
fix(payments): handle null reference in checkout
docs(api): update OpenAPI specification for v2 endpoints
refactor(users): extract validation into FluentValidation rules
test(orders): add integration tests for order creation
chore(deps): update MediatR to v12.2.0
```

## Review Checklist

### Critical (Must Fix)
- [ ] Security vulnerabilities (OWASP Top 10)
- [ ] Data loss or corruption risk
- [ ] Authentication/authorization bypass
- [ ] Hardcoded secrets or credentials
- [ ] SQL injection or command injection

### High (Should Fix)
- [ ] Missing input validation
- [ ] Incorrect async/await usage (deadlock risk)
- [ ] Missing error handling for external calls
- [ ] N+1 query patterns
- [ ] Missing `CancellationToken` propagation

### Medium (Recommended)
- [ ] Code duplication
- [ ] Missing XML documentation on public APIs
- [ ] Suboptimal LINQ usage
- [ ] Missing test coverage for new code
- [ ] Inconsistent naming conventions

### Low (Nice to Have)
- [ ] Minor readability improvements
- [ ] Spelling errors in comments
- [ ] Unnecessary comments (code is self-explanatory)
- [ ] Method ordering within class

## Review Response Expectations

| Finding Severity | Max Response Time |
|-----------------|-------------------|
| Critical | Immediate (block merge) |
| High | Same day |
| Medium | Within sprint |
| Low | Backlog (optional) |

## PR Template

```markdown
## Description
{What this PR does and why}

## Linked Issue
Closes #{issue_number}

## Changes
- {Change 1}
- {Change 2}

## Type of Change
- [ ] Feature (new functionality)
- [ ] Bug Fix (fixes an issue)
- [ ] Refactor (no behavior change)
- [ ] Documentation
- [ ] Test
- [ ] Chore (build, CI, dependencies)

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Manual testing performed

## Checklist
- [ ] Code follows project coding standards
- [ ] Self-review completed
- [ ] XML docs on public APIs
- [ ] No new analyzer warnings
- [ ] No hardcoded secrets
- [ ] Database migration included (if applicable)
```
