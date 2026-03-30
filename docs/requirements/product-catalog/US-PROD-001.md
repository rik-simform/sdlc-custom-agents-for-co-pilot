# US-PROD-001: Create Product (Admin Only)

**Type**: Functional
**Priority**: Critical
**Story Points**: 5
**Source**: EPIC-PROD / PRD Section 6
**Status**: Ready

---

## User Story

**As an** Admin user
**I want to** create a new product with name, description, price, category, and image URL
**So that** customers can discover and browse the product in the catalog

---

## Acceptance Criteria

- [ ] **AC-001**: Given an authenticated Admin user with a valid request body, when they call `POST /api/v1/products`, then the system returns HTTP 201 with the created product including a server-generated `Id` (GUID) and `CreatedAt` timestamp, and a `Location` header pointing to the new resource.
- [ ] **AC-002**: Given a request with a product name that already exists in the same category, when the Admin submits, then the system returns HTTP 409 Conflict with a ProblemDetails response indicating a duplicate product.
- [ ] **AC-003**: Given a request with any invalid field (see validation rules), when the Admin submits, then the system returns HTTP 400 with ProblemDetails listing all validation errors.
- [ ] **AC-004**: Given a request from an unauthenticated user, when they call `POST /api/v1/products`, then the system returns HTTP 401 Unauthorized.
- [ ] **AC-005**: Given a request from an authenticated user without the `Admin` role, when they call `POST /api/v1/products`, then the system returns HTTP 403 Forbidden.
- [ ] **AC-006**: Given a successful product creation, when the operation completes, then an audit entry is written with `Action = "ProductCreated"`, the product ID, and the Admin user ID.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | Create endpoint responds < 1s (p95) including audit write |
| Security | Only `Admin` role can invoke; enforced via authorization policy |
| Security | All input validated server-side via FluentValidation before DB write |
| Security | SQL injection prevented by EF Core parameterized queries |
| Security | Mass assignment prevention: only declared DTO properties mapped to entity |
| Scalability | GUID primary key avoids hotspot on clustered index |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-LOGIN-001 — JWT authentication middleware configured | Cross-Epic |
| DEP-002 | US-PROD-009 — Category must exist before assigning to product | Story |
| DEP-003 | SQL Server database with Products and Categories tables | Infrastructure |

---

## .NET Implementation Notes

### API Endpoint

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/v1/products` | `Admin` | Create a new product |

### Key Entities

```csharp
public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public byte[] RowVersion { get; set; } = [];
}
```

### Request/Response Models

```csharp
public record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    Guid CategoryId,
    string? ImageUrl);

public record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    Guid CategoryId,
    string CategoryName,
    string? ImageUrl,
    DateTime CreatedAt);
```

### FluentValidation Rules

```csharp
public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Product description is required")
            .MaximumLength(4000).WithMessage("Description must not exceed 4000 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero")
            .LessThanOrEqualTo(999999.99m).WithMessage("Price must not exceed 999,999.99");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required");

        RuleFor(x => x.ImageUrl)
            .Must(url => url is null || Uri.TryCreate(url, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttps))
            .When(x => x.ImageUrl is not null)
            .WithMessage("Image URL must be a valid HTTPS URL");
    }
}
```

### Authorization Policy

```csharp
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
```

### Vertical Slice Path

`Features/Products/CreateProduct/`

---

## Linked Artifacts

| Type | ID | Description |
|------|-----|------------|
| Design | ADR-PROD-001 | Vertical slice architecture for product catalog |
| Design | ADR-PROD-002 | Soft delete vs hard delete decision |
| Tests | TC-PROD-001 | Valid request creates product and returns 201 |
| Tests | TC-PROD-002 | Duplicate name+category returns 409 |
| Tests | TC-PROD-003 | Invalid fields return 400 with ProblemDetails |
| Tests | TC-PROD-004 | Unauthenticated request returns 401 |
| Tests | TC-PROD-005 | Non-admin role returns 403 |
| Tests | TC-PROD-006 | Audit entry written on successful create |

---

## Definition of Ready Checklist

- [x] Has unique ID (US-PROD-001)
- [x] Has ≥ 3 acceptance criteria (6 ACs)
- [x] Each AC is testable / automatable
- [x] Priority assigned (Critical)
- [x] Dependencies identified (3)
- [x] NFRs specified (6)
- [x] Story points estimated (5)
- [ ] Reviewed by Product Owner
