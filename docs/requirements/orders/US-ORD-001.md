# REQ-CLASS: CROSS-CUTTING

## REQ-ORD-001: Customer Places Order

**REQ-CLASS**: CROSS-CUTTING  
**INVEST**: I=pass N=pass V=pass E=pass S=pass T=pass  
**Type**: Functional  
**Priority**: Critical  
**Estimate**: 5 SP  
**Source**: User request (2026-04-09)  
**Status**: [DRAFT]

### User Story
**As a** customer  
**I want to** place an order for an inventory item  
**So that** I can request fulfillment from the system.

### Acceptance Criteria
- [ ] AC-001: Given an authenticated user, when they submit a valid order, then the API returns HTTP 201 with a created order ID and Pending status.
- [ ] AC-002: Given invalid quantity or invalid item ID, when submitted, then the API returns HTTP 400 with a validation or business error.
- [ ] AC-003: Given insufficient stock, when submitted, then the API returns HTTP 400 and no order is created.
- [ ] AC-004: Given order placement from the UI inventory page, when successful, then the user is redirected to My Orders with a success message.

### Non-Functional Requirements
- Performance: p95 for create order endpoint is < 1 second under normal load.
- Security: Only authenticated users can place orders, and UserId must be sourced from JWT claims.
- Scalability: Endpoint supports pagination-ready downstream retrieval with no unbounded read.

### Affected Modules
| Module | File | Change Type | Risk |
|---|---|---|---|
| API | src/MyProject.Api/Endpoints/OrderEndpoints.cs | Existing endpoint validation and response contract check | Medium |
| Application | src/MyProject.Application/Features/Orders/Commands/OrderCommands.cs | Business rule verification | Medium |
| Infrastructure | src/MyProject.Infrastructure/Repositories/OrderRepository.cs | Persistence path | Low |
| Web UI | src/MyProject.Web/Pages/Inventory/Index.cshtml.cs | Existing UI order placement flow | Low |

### Recommended NuGet Packages
**DEP-RECOMMENDATION**: PRESENT — No new packages required.

| DEP-ID | Package | Min Version | Purpose | Justification | Alternatives Considered | Licence | Already in Project? |
|---|---|---|---|---|---|---|---|
| DEP-001 | MediatR | >= 12.4.1 | Command handling | Existing CQRS pattern | Custom mediator (rejected: unnecessary) | Apache-2.0 | Yes |
| DEP-002 | FluentValidation | >= 11.11.0 | Input validation | Existing validators | DataAnnotations (rejected: less expressive) | Apache-2.0 | Yes |

### Other Dependencies
- DEP-101: JWT claims include NameIdentifier for authenticated users.

### Assumptions
- ASM-001: Inventory stock validation remains synchronous in command handler.

### Linked Artifacts
- Design: ADR-ORDERS-001
- Tests: TC-ORD-001, TC-ORD-002, TC-ORD-003, TC-ORD-004
- Implementation: PR-TBD
