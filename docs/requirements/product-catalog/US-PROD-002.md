# US-PROD-002: Update Product (Admin Only)

**Type**: Functional
**Priority**: Critical
**Story Points**: 5
**Source**: EPIC-PROD / PRD Section 6
**Status**: Ready

---

## User Story

**As an** Admin user
**I want to** update an existing product's details with concurrency protection
**So that** product information stays accurate and concurrent edits do not silently overwrite each other

---

## Acceptance Criteria

- [ ] **AC-001**: Given an authenticated Admin user with a valid request body including the current `RowVersion`, when they call `PUT /api/v1/products/{id}`, then the system returns HTTP 200 with the updated product including a new `RowVersion` and `UpdatedAt` timestamp.
- [ ] **AC-002**: Given a request with a stale `RowVersion` (another user modified the product after this user fetched it), when the Admin submits, then the system returns HTTP 409 Conflict with a ProblemDetails response explaining the concurrency conflict.
- [ ] **AC-003**: Given a request with a non-existent product ID, when the Admin submits, then the system returns HTTP 404 Not Found.
- [ ] **AC-004**: Given a request with invalid fields, when the Admin submits, then the system returns HTTP 400 with ProblemDetails listing all validation errors.
- [ ] **AC-005**: Given a request from an unauthenticated user or a user without the `Admin` role, when they call `PUT /api/v1/products/{id}`, then the system returns HTTP 401 or HTTP 403 respectively.
- [ ] **AC-006**: Given a successful update, when the operation completes, then an audit entry is written with `Action = "ProductUpdated"`, the product ID, the Admin user ID, and a snapshot of changed fields.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | Update endpoint responds < 1s (p95) including audit write |
| Security | Only `Admin` role can invoke; enforced via authorization policy |
| Security | Concurrency token prevents lost updates (OWASP A08 — Software Integrity) |
| Security | Input validated server-side; parameterized queries via EF Core |
| Reliability | `DbUpdateConcurrencyException` caught and mapped to HTTP 409 |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-PROD-001 — Product entity and table must exist | Story |
| DEP-002 | US-LOGIN-001 — JWT authentication middleware | Cross-Epic |
| DEP-003 | US-PROD-009 — Category must exist if CategoryId changes | Story |

---

## .NET Implementation Notes

### API Endpoint

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| PUT | `/api/v1/products/{id:guid}` | `Admin` | Update an existing product |

### Request Model

```csharp
public record UpdateProductRequest(
    string Name,
    string Description,
    decimal Price,
    Guid CategoryId,
    string? ImageUrl,
    byte[] RowVersion);
```

### Concurrency Handling

```csharp
// Entity configuration
builder.Property(p => p.RowVersion)
    .IsRowVersion();

// Handler — catch concurrency exception
try
{
    await dbContext.SaveChangesAsync(cancellationToken);
}
catch (DbUpdateConcurrencyException)
{
    return Result<ProductResponse>.Conflict(
        "The product was modified by another user. Please reload and try again.");
}
```

### FluentValidation Rules

```csharp
public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
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

        RuleFor(x => x.RowVersion)
            .NotEmpty().WithMessage("RowVersion is required for concurrency control");
    }
}
```

### Vertical Slice Path

`Features/Products/UpdateProduct/`

---

## Linked Artifacts

| Type | ID | Description |
|------|-----|------------|
| Design | ADR-PROD-003 | Optimistic concurrency strategy |
| Tests | TC-PROD-007 | Valid update returns 200 with new RowVersion |
| Tests | TC-PROD-008 | Stale RowVersion returns 409 Conflict |
| Tests | TC-PROD-009 | Non-existent product returns 404 |
| Tests | TC-PROD-010 | Invalid fields return 400 |
| Tests | TC-PROD-011 | Unauthorized/forbidden returns 401/403 |
| Tests | TC-PROD-012 | Audit entry written on successful update |

---

## Definition of Ready Checklist

- [x] Has unique ID (US-PROD-002)
- [x] Has ≥ 3 acceptance criteria (6 ACs)
- [x] Each AC is testable / automatable
- [x] Priority assigned (Critical)
- [x] Dependencies identified (3)
- [x] NFRs specified (5)
- [x] Story points estimated (5)
- [ ] Reviewed by Product Owner
