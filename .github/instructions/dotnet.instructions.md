---
applyTo: '**/*.cs'
---

# .NET Coding Standards for SDLC Agents

## Language & Framework

- Target: .NET 6+ / .NET 8 preferred
- Language: C# 12+ features enabled
- Nullable reference types: `#nullable enable` globally

## Naming Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| Classes | PascalCase | `UserService` |
| Interfaces | IPascalCase | `IUserService` |
| Methods | PascalCase | `GetUserAsync` |
| Properties | PascalCase | `FirstName` |
| Private fields | _camelCase | `_logger` |
| Local variables | camelCase | `userName` |
| Constants | PascalCase | `MaxRetryCount` |
| Async methods | Suffix with Async | `GetUserAsync` |
| Generic types | T prefix | `TEntity`, `TResponse` |

## Code Structure

### Project Organization
```
src/
  {Name}.Api/              → ASP.NET Core host, controllers, middleware
  {Name}.Application/      → Use cases, commands, queries, DTOs, validators
  {Name}.Domain/           → Entities, value objects, domain interfaces
  {Name}.Infrastructure/   → EF Core, external services, repositories
tests/
  {Name}.UnitTests/        → Unit tests (MSTest + FluentAssertions)
  {Name}.IntegrationTests/ → Integration tests (WebApplicationFactory)
```

### File Organization
- One type per file (class, interface, enum, record)
- File name matches type name
- Feature folders over layer folders in Application project

## Patterns (Mandatory)

### Dependency Injection
```csharp
// Primary constructors for DI
public class UserService(IUserRepository repository, ILogger<UserService> logger)
```

### Async/Await
```csharp
// Always propagate CancellationToken
public async Task<User> GetAsync(int id, CancellationToken ct = default)
{
    return await _repository.GetByIdAsync(id, ct);
}
```

### DTOs
```csharp
// Records for immutable DTOs
public record UserResponse(int Id, string Name, string Email);
public record CreateUserRequest(string Name, string Email);
```

### Validation
```csharp
// FluentValidation
public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
```

### Error Handling
```csharp
// Result pattern for business logic (no exceptions for expected errors)
public async Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken ct)
{
    if (await _repository.ExistsByEmailAsync(request.Email, ct))
        return Result.Fail<UserResponse>("Email already exists");

    var user = new User(request.Name, request.Email);
    await _repository.AddAsync(user, ct);
    return Result.Ok(user.ToResponse());
}
```

## Anti-Patterns to Flag

| Anti-Pattern | Issue | Fix |
|-------------|-------|-----|
| `new HttpClient()` | Socket exhaustion | Use `IHttpClientFactory` |
| `Task.Result` / `.Wait()` | Deadlock risk | Use `await` |
| `catch (Exception) { }` | Swallowed errors | Log or rethrow |
| `DateTime.Now` | Not UTC-safe | Use `DateTimeOffset.UtcNow` |
| `string.Format` in SQL | SQL injection | Use EF Core parameters |
| `public` fields | Encapsulation break | Use properties |
| Magic strings | Fragile, untestable | Use constants or enums |

## XML Documentation

Required on all public APIs:

```csharp
/// <summary>
/// Retrieves a user by their unique identifier.
/// </summary>
/// <param name="id">The user's unique identifier.</param>
/// <param name="ct">Cancellation token.</param>
/// <returns>The user if found; otherwise null.</returns>
/// <exception cref="ArgumentException">Thrown when id is less than 1.</exception>
public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
```
