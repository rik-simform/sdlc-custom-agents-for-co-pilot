---
applyTo: '**/*Test*.cs,**/*test*.cs'
---

# Testing Standards for SDLC Agents

## Framework Stack

| Layer | Framework | Purpose |
|-------|-----------|---------|
| Unit Tests | MSTest v2 or xUnit | Test isolation |
| Assertions | FluentAssertions | Readable assertions |
| Mocking | Moq | Dependency mocking |
| Integration | WebApplicationFactory | API testing |
| E2E | Playwright (.NET) | Browser testing |
| Performance | BenchmarkDotNet, k6 | Load and benchmark |

## Test Naming Convention

```
{MethodName}_{Scenario}_{ExpectedBehavior}
```

Examples:
- `GetById_WithValidId_ReturnsUser`
- `GetById_WithNonExistentId_ReturnsNull`
- `Create_WithDuplicateEmail_ReturnsFailResult`
- `Login_WithExpiredToken_ReturnsUnauthorized`

## Test Structure (AAA Pattern)

```csharp
[TestMethod]
public async Task MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    var mockRepo = new Mock<IUserRepository>();
    mockRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
        .ReturnsAsync(new User(1, "Test User"));
    var sut = new UserService(mockRepo.Object, NullLogger<UserService>.Instance);

    // Act
    var result = await sut.GetByIdAsync(1, CancellationToken.None);

    // Assert
    result.Should().NotBeNull();
    result!.Name.Should().Be("Test User");
}
```

## Test Categories

Apply categories for selective execution:

```csharp
[TestCategory("Unit")]           // Default, fast, isolated
[TestCategory("Integration")]    // Requires infrastructure (DB, API)
[TestCategory("E2E")]           // Full browser/system tests
[TestCategory("Performance")]    // Load, stress, benchmark
[TestCategory("Security")]       // Security-focused tests
[TestCategory("Smoke")]         // Post-deployment validation
```

## Coverage Requirements

| Project Layer | Minimum | Target |
|-------------|---------|--------|
| Domain | 90% | 95% |
| Application | 80% | 90% |
| API/Controllers | 70% | 80% |
| Infrastructure | 60% | 70% |
| Overall | **80%** | 85% |

## Integration Test Pattern

```csharp
[TestClass]
public class UsersEndpointTests
{
    private static WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    [ClassInitialize]
    public static void ClassInit(TestContext _)
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Replace services for testing
                });
            });
    }

    [TestInitialize]
    public void TestInit()
    {
        _client = _factory.CreateClient();
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task GetUsers_ReturnsOkWithList()
    {
        var response = await _client.GetAsync("/api/v1/users");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

## What to Test

| Scenario | Required | Example |
|----------|----------|---------|
| Happy path | ✅ Always | Valid input → expected output |
| Null/empty input | ✅ Always | null → ArgumentNullException or error result |
| Invalid input | ✅ Always | Invalid email → validation error |
| Not found | ✅ When applicable | Missing ID → null or not found result |
| Authorization | ✅ For secured endpoints | No token → 401 |
| Boundary values | ✅ For numeric/string limits | Max length, 0, negative |
| Concurrency | ⚠️ When applicable | Parallel writes → no data corruption |

## What NOT to Test

- Framework behavior (EF Core, ASP.NET Core internals)
- Simple property getters/setters
- Auto-generated code
- Third-party library internals
