# REQ-CLASS: CROSS-CUTTING

## REQ-ORD-002: Customer Views Own Orders In UI

**REQ-CLASS**: CROSS-CUTTING  
**INVEST**: I=pass N=pass V=pass E=pass S=pass T=pass  
**Type**: Functional  
**Priority**: High  
**Estimate**: 3 SP  
**Source**: User request (2026-04-09)  
**Status**: [DRAFT]

### User Story
**As a** customer  
**I want to** view my own orders and their statuses in the UI  
**So that** I can track progress without contacting support.

### Acceptance Criteria
- [ ] AC-001: Given an authenticated user, when they open My Orders, then the page displays only their orders with status and dates.
- [ ] AC-002: Given pagination parameters, when the user navigates pages, then the API and UI return consistent page metadata and rows.
- [ ] AC-003: Given unauthenticated access, when My Orders is requested, then the user is redirected to login.
- [ ] AC-004: Given a visible status badge, when order statuses are shown, then labels and colors follow one canonical status vocabulary.

### Non-Functional Requirements
- Performance: p95 for My Orders retrieval is < 500 ms for page size <= 100.
- Security: User can view only own orders, enforced server side by claim-based filtering.
- Scalability: Supports at least 10,000 orders through pagination without full table scans.

### Affected Modules
| Module | File | Change Type | Risk |
|---|---|---|---|
| API | src/MyProject.Api/Endpoints/OrderEndpoints.cs | Existing my-orders contract | Low |
| Application | src/MyProject.Application/Features/Orders/Queries/OrderQueries.cs | Query contract and mapping | Low |
| Web UI | src/MyProject.Web/Pages/Orders/MyOrdersModel.cs | Existing page model flow | Low |
| Web UI | src/MyProject.Web/Pages/Orders/MyOrders.cshtml | Status badge consistency | Medium |

### Recommended NuGet Packages
**DEP-RECOMMENDATION**: PRESENT — No new packages required.

| DEP-ID | Package | Min Version | Purpose | Justification | Alternatives Considered | Licence | Already in Project? |
|---|---|---|---|---|---|---|---|
| DEP-001 | Microsoft.AspNetCore.Authentication.JwtBearer | >= 8.0.12 | Secure API access | Existing auth stack | Cookie auth only (rejected: API uses bearer tokens) | MIT | Yes |

### Other Dependencies
- DEP-102: TokenService role and authentication checks in web project remain unchanged.

### Assumptions
- ASM-002: Current My Orders route remains /Orders/MyOrders and is linked from main navigation.

### Linked Artifacts
- Design: ADR-ORDERS-001
- Tests: TC-ORD-005, TC-ORD-006, TC-ORD-007
- Implementation: PR-TBD
