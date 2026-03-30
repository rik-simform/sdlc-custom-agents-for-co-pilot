# US-PROD-009: Product Category Management (Admin)

**Type**: Functional
**Priority**: High
**Story Points**: 5
**Source**: EPIC-PROD / PRD Section 6
**Status**: Ready

---

## User Story

**As an** Admin user
**I want to** create, update, delete, and list product categories
**So that** products can be organized into meaningful groups for customers to browse and filter

---

## Acceptance Criteria

### Create Category

- [ ] **AC-001**: Given an authenticated Admin user with a valid category name, when they call `POST /api/v1/categories`, then the system returns HTTP 201 with the created category including `Id`, `Name`, `CreatedAt`, and a `Location` header.
- [ ] **AC-002**: Given a category name that already exists (case-insensitive), when the Admin submits, then the system returns HTTP 409 Conflict.

### Update Category

- [ ] **AC-003**: Given an authenticated Admin user with a valid category ID and new name, when they call `PUT /api/v1/categories/{id}`, then the system returns HTTP 200 with the updated category.
- [ ] **AC-004**: Given a non-existent category ID, when the Admin submits an update, then the system returns HTTP 404 Not Found.

### Delete Category

- [ ] **AC-005**: Given a category with no associated products, when the Admin calls `DELETE /api/v1/categories/{id}`, then the category is removed and the system returns HTTP 204.
- [ ] **AC-006**: Given a category with associated products, when the Admin calls `DELETE /api/v1/categories/{id}`, then the system returns HTTP 409 Conflict with `"Cannot delete category with associated products. Reassign or remove products first."`.

### List Categories

- [ ] **AC-007**: Given any user (authenticated or not), when they call `GET /api/v1/categories`, then the system returns HTTP 200 with all categories ordered alphabetically by name.

### Authorization

- [ ] **AC-008**: Given a request from a non-admin user to `POST`, `PUT`, or `DELETE` category endpoints, then the system returns HTTP 403 Forbidden. `GET` is public.

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Performance | Category list < 100ms (p95) — small dataset, cached aggressively |
| Security | Write endpoints require `Admin` role; read is public |
| Security | Category name validated: max 100 chars, not empty, no HTML injection |
| Data Integrity | Foreign key constraint prevents orphaned product→category references |
| Caching | Category list cached with output cache; invalidated on any category write |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-LOGIN-001 — JWT authentication middleware | Cross-Epic |
| DEP-002 | SQL Server database with Categories table | Infrastructure |

---

## .NET Implementation Notes

### API Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/v1/categories` | `Admin` | Create a category |
| PUT | `/api/v1/categories/{id:guid}` | `Admin` | Update a category |
| DELETE | `/api/v1/categories/{id:guid}` | `Admin` | Delete a category (only if no products) |
| GET | `/api/v1/categories` | Anonymous | List all categories |

### Key Entity

```csharp
public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = [];
}
```

### Request/Response Models

```csharp
public record CreateCategoryRequest(string Name);
public record UpdateCategoryRequest(string Name);
public record CategoryResponse(Guid Id, string Name, DateTime CreatedAt, int ProductCount);
```

### FluentValidation Rules

```csharp
public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required")
            .MaximumLength(100).WithMessage("Category name must not exceed 100 characters");
    }
}
```

### Delete Guard

```csharp
var hasProducts = await dbContext.Products
    .AnyAsync(p => p.CategoryId == id, cancellationToken);

if (hasProducts)
    return Result.Conflict("Cannot delete category with associated products.");
```

### Vertical Slice Path

```
Features/Categories/
├── CreateCategory/
├── UpdateCategory/
├── DeleteCategory/
└── ListCategories/
```

---

## Linked Artifacts

| Type | ID | Description |
|------|-----|------------|
| Design | ADR-PROD-005 | Category management and referential integrity |
| Tests | TC-PROD-049 | Create category returns 201 |
| Tests | TC-PROD-050 | Duplicate category name returns 409 |
| Tests | TC-PROD-051 | Update category returns 200 |
| Tests | TC-PROD-052 | Update non-existent category returns 404 |
| Tests | TC-PROD-053 | Delete empty category returns 204 |
| Tests | TC-PROD-054 | Delete category with products returns 409 |
| Tests | TC-PROD-055 | List categories returns all, alphabetically |
| Tests | TC-PROD-056 | Non-admin write returns 403; GET is public |

---

## Definition of Ready Checklist

- [x] Has unique ID (US-PROD-009)
- [x] Has ≥ 3 acceptance criteria (8 ACs)
- [x] Each AC is testable / automatable
- [x] Priority assigned (High)
- [x] Dependencies identified (2)
- [x] NFRs specified (5)
- [x] Story points estimated (5)
- [ ] Reviewed by Product Owner
