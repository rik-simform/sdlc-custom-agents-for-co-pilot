---
applyTo: '**/*.cs,**/*.json,**/*.yaml,**/*.yml'
---

# Security Standards for SDLC Agents

## OWASP Top 10 Compliance (.NET)

### A01: Broken Access Control
- Always use `[Authorize]` attribute (default deny)
- Implement resource-level ownership checks (prevent IDOR)
- Use policy-based authorization for complex rules
- Never rely solely on client-side access control

```csharp
// CORRECT: Server-side ownership check
[HttpGet("{id}")]
[Authorize]
public async Task<IActionResult> GetOrder(int id, CancellationToken ct)
{
    var order = await _mediator.Send(new GetOrderQuery(id), ct);
    if (order.UserId != User.GetUserId())
        return Forbid();
    return Ok(order);
}
```

### A02: Cryptographic Failures
- Enforce HTTPS: `app.UseHttpsRedirection()`
- Enable HSTS: `app.UseHsts()`
- Use Data Protection API for sensitive data encryption
- Never use MD5/SHA1 for security-sensitive hashing

### A03: Injection
- Use Entity Framework Core (parameterized by default)
- Never concatenate user input into SQL, commands, or LDAP queries
- Use FluentValidation for all input models
- Encode output for HTML contexts

```csharp
// WRONG: SQL injection risk
var sql = $"SELECT * FROM Users WHERE Name = '{name}'";

// CORRECT: Parameterized via EF Core
var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == name, ct);
```

### A05: Security Misconfiguration
- Disable developer exception page in production
- Remove server header: `builder.WebHost.ConfigureKestrel(o => o.AddServerHeader = false)`
- Configure security headers via middleware

```csharp
// Required security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "0");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
    await next();
});
```

### A09: Security Logging
- Log authentication events (success + failure)
- Log authorization failures
- Never log sensitive data (passwords, tokens, PII)
- Use structured logging with correlation IDs

```csharp
// WRONG: Logging sensitive data
_logger.LogInformation("User {Email} logged in with password {Password}", email, password);

// CORRECT: Structured, sanitized
_logger.LogInformation("User authentication successful for {UserId}", userId);
```

## Secrets Management

- **NEVER** store secrets in source code, appsettings.json, or environment variables in config files
- Use Azure Key Vault or equivalent for production secrets
- Use User Secrets for local development: `dotnet user-secrets set "Key" "Value"`
- Use token replacement in CI/CD: `#{SECRET_NAME}#`

## Dependency Security

- Enable Dependabot or Snyk for automated vulnerability scanning
- Run `dotnet list package --vulnerable` in CI pipeline
- Block PRs that introduce packages with known critical CVEs
- Maintain an SBOM for every release
