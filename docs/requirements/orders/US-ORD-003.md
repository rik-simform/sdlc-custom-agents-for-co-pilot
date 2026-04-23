# REQ-CLASS: CROSS-CUTTING

## REQ-ORD-003: Admin Views All Orders In UI

**REQ-CLASS**: CROSS-CUTTING  
**INVEST**: I=pass N=pass V=pass E=pass S=pass T=pass  
**Type**: Functional  
**Priority**: Critical  
**Estimate**: 5 SP  
**Source**: User request (2026-04-09)  
**Status**: [DRAFT]

### User Story
**As an** admin  
**I want to** view all placed orders from all users in one UI screen  
**So that** I can monitor and manage order processing.

### Acceptance Criteria
- [ ] AC-001: Given an admin user, when opening All Orders, then a paginated list of all orders is displayed with user, item, quantity, status, and timestamps.
- [ ] AC-002: Given a non-admin user, when requesting admin order list API or UI, then access is denied with 403.
- [ ] AC-003: Given optional status and user filters, when applied, then results are correctly narrowed while preserving pagination.
- [ ] AC-004: Given no orders, when page renders, then an empty-state message appears without server error.

### Non-Functional Requirements
- Performance: p95 for admin list endpoint is < 1 second at page size <= 100.
- Security: Admin-only authorization is enforced by role policy and server checks.
- Scalability: Supports filterable query execution for at least 10,000 orders.

### Affected Modules
| Module | File | Change Type | Risk |
|---|---|---|---|
| API | src/MyProject.Api/Endpoints/OrderEndpoints.cs | Existing admin list route | Low |
| Application | src/MyProject.Application/Features/Orders/Queries/OrderQueries.cs | Filter behavior verification | Medium |
| Infrastructure | src/MyProject.Infrastructure/Repositories/OrderRepository.cs | Query semantics and indices usage | Medium |
| Web UI | src/MyProject.Web/Pages/Orders/AllOrdersModel.cs | Existing page behavior | Low |
| Web UI | src/MyProject.Web/Pages/Orders/AllOrders.cshtml | List and filter rendering | Low |

### Recommended NuGet Packages
**DEP-RECOMMENDATION**: PRESENT — No new packages required.

| DEP-ID | Package | Min Version | Purpose | Justification | Alternatives Considered | Licence | Already in Project? |
|---|---|---|---|---|---|---|---|
| DEP-001 | Microsoft.AspNetCore.Authentication.JwtBearer | >= 8.0.12 | Admin endpoint protection | Existing auth stack | Custom auth middleware (rejected: unnecessary) | MIT | Yes |
| DEP-002 | Microsoft.EntityFrameworkCore.Sqlite | >= 8.0.12 | Order query persistence | Existing data provider | Dapper (rejected: would duplicate data access style) | MIT | Yes |

### Other Dependencies
- DEP-103: Admin role claim mapping must remain aligned between token generation and UI checks.

### Assumptions
- ASM-003: Admin dashboard remains at /Orders/AllOrders.

### Linked Artifacts
- Design: ADR-ORDERS-001
- Tests: TC-ORD-008, TC-ORD-009, TC-ORD-010
- Implementation: PR-TBD
