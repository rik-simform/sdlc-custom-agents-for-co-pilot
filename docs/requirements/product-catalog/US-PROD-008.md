# US-PROD-008: Product Image URL Validation

**Type**: Functional
**Priority**: Medium
**Story Points**: 2
**Source**: EPIC-PROD / PRD Section 6
**Status**: Ready

---

## User Story

**As an** Admin user
**I want to** have the product image URL validated for correct format and HTTPS protocol
**So that** the catalog only contains safe, well-formed image references and customers are not exposed to insecure content

---

## Acceptance Criteria

- [ ] **AC-001**: Given a product create/update request with a valid HTTPS URL for `ImageUrl` (e.g., `https://cdn.example.com/images/product.jpg`), when the Admin submits, then the URL is accepted and stored.
- [ ] **AC-002**: Given a product create/update request with an HTTP (non-HTTPS) URL, when the Admin submits, then the system returns HTTP 400 with `"Image URL must use HTTPS"`.
- [ ] **AC-003**: Given a product create/update request with a malformed URL (e.g., `not-a-url`, `ftp://file.jpg`), when the Admin submits, then the system returns HTTP 400 with `"Image URL must be a valid HTTPS URL"`.
- [ ] **AC-004**: Given a product create/update request with `ImageUrl` set to `null` or omitted, when the Admin submits, then the product is created/updated without an image (field is optional).
- [ ] **AC-005**: Given a product create/update request with an `ImageUrl` longer than 2048 characters, when the Admin submits, then the system returns HTTP 400 with `"Image URL must not exceed 2048 characters"`.
- [ ] **AC-006**: Given a product create/update request with an `ImageUrl` containing JavaScript (e.g., `javascript:alert(1)`), when the Admin submits, then the system returns HTTP 400 (XSS prevention via scheme validation).

---

## Non-Functional Requirements

| Category | Requirement |
|----------|------------|
| Security | Only HTTPS scheme accepted — prevents mixed content and MITM (OWASP A02) |
| Security | URL scheme validation prevents `javascript:` and `data:` URI attacks (OWASP A03/XSS) |
| Security | URL is not fetched server-side at creation time — prevents SSRF (OWASP A10) |
| Performance | Validation is format-only; no HTTP HEAD request to verify URL reachability |
| Data Integrity | Max 2048 chars for URL — safe for SQL NVARCHAR and browser URL bars |

---

## Dependencies

| ID | Dependency | Type |
|----|-----------|------|
| DEP-001 | US-PROD-001 / US-PROD-002 — Create/Update validators exist | Story |

---

## .NET Implementation Notes

### Shared Validation Rule (reused in Create and Update validators)

```csharp
public static class ImageUrlValidationRules
{
    public static IRuleBuilderOptions<T, string?> ValidImageUrl<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .MaximumLength(2048).WithMessage("Image URL must not exceed 2048 characters")
            .Must(url => url is null || (Uri.TryCreate(url, UriKind.Absolute, out var uri)
                && uri.Scheme == Uri.UriSchemeHttps))
            .WithMessage("Image URL must be a valid HTTPS URL");
    }
}
```

### Usage in Validators

```csharp
// In CreateProductRequestValidator and UpdateProductRequestValidator
RuleFor(x => x.ImageUrl)
    .ValidImageUrl();
```

### Vertical Slice Path

Cross-cutting — implemented in `Features/Products/Shared/ImageUrlValidationRules.cs`

---

## Linked Artifacts

| Type | ID | Description |
|------|-----|------------|
| Tests | TC-PROD-043 | Valid HTTPS URL accepted |
| Tests | TC-PROD-044 | HTTP URL rejected with 400 |
| Tests | TC-PROD-045 | Malformed URL rejected with 400 |
| Tests | TC-PROD-046 | Null/omitted ImageUrl accepted |
| Tests | TC-PROD-047 | URL over 2048 chars rejected |
| Tests | TC-PROD-048 | javascript: URI rejected |

---

## Definition of Ready Checklist

- [x] Has unique ID (US-PROD-008)
- [x] Has ≥ 3 acceptance criteria (6 ACs)
- [x] Each AC is testable / automatable
- [x] Priority assigned (Medium)
- [x] Dependencies identified (1)
- [x] NFRs specified (5)
- [x] Story points estimated (2)
- [ ] Reviewed by Product Owner
