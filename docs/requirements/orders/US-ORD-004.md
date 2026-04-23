# REQ-CLASS: CROSS-CUTTING

## REQ-ORD-004: Admin Updates Order Status (API)

**REQ-CLASS**: CROSS-CUTTING  
**INVEST**: I=pass N=pass V=pass E=pass S=pass T=pass  
**Type**: Functional  
**Priority**: Critical  
**Estimate**: 5 SP  
**Source**: User request (2026-04-09)  
**Status**: [DRAFT]

### User Story
**As an** admin  
**I want to** update order statuses through a secured API  
**So that** I can move orders through fulfillment states.

### Acceptance Criteria
- [ ] AC-001: Given an admin user, when submitting a valid status update for an existing order, then the API returns HTTP 200 with updated status and update timestamp.
- [ ] AC-002: Given a non-admin user, when calling status update endpoint, then API returns HTTP 403.
- [ ] AC-003: Given an unknown order ID, when calling status update endpoint, then API returns HTTP 404.
- [ ] AC-004: Given invalid target status or invalid status transition, when submitted, then API returns HTTP 400.
- [ ] AC-005: Given update to terminal status Fulfilled, then FulfilledAt is set when first fulfilled and remains non-null.

### Non-Functional Requirements
- Performance: p95 status update endpoint latency is < 500 ms.
- Security: Endpoint is admin-only and logs actor identity for audit.
- Scalability: Supports burst updates from operations users without data corruption.

### Affected Modules
| Module | File | Change Type | Risk |
|---|---|---|---|
| API | src/MyProject.Api/Endpoints/OrderEndpoints.cs | New route for status update required | High |
| Application | src/MyProject.Application/Features/Orders/Commands/OrderCommands.cs | New command handler for status transitions | High |
| Domain | src/MyProject.Domain/Entities/Order.cs | Canonical status vocabulary alignment | High |
| Infrastructure | src/MyProject.Infrastructure/Repositories/OrderRepository.cs | Update persistence path | Medium |

### Recommended NuGet Packages
**DEP-RECOMMENDATION**: PRESENT — No new packages required.

| DEP-ID | Package | Min Version | Purpose | Justification | Alternatives Considered | Licence | Already in Project? |
|---|---|---|---|---|---|---|---|
| DEP-001 | MediatR | >= 12.4.1 | Command dispatch | Existing CQRS baseline | Service method only (rejected: breaks established pattern) | Apache-2.0 | Yes |
| DEP-002 | FluentValidation | >= 11.11.0 | Update request validation | Existing validation stack | Manual guard clauses only (rejected: consistency risk) | Apache-2.0 | Yes |

### Other Dependencies
- DEP-104: Authorization policies include Admin role and are enforced in API mapping.

### Assumptions
- ASM-004: Canonical statuses are Pending, Processing, Fulfilled, Cancelled.

### Linked Artifacts
- Design: ADR-ORDERS-001
- Tests: TC-ORD-011, TC-ORD-012, TC-ORD-013, TC-ORD-014
- Implementation: PR-TBD
