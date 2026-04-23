# REQ-CLASS: CROSS-CUTTING

## REQ-ORD-005: Admin Updates Order Status In UI

**REQ-CLASS**: CROSS-CUTTING  
**INVEST**: I=pass N=pass V=pass E=pass S=pass T=pass  
**Type**: Functional  
**Priority**: High  
**Estimate**: 3 SP  
**Source**: User request (2026-04-09)  
**Status**: [DRAFT]

### User Story
**As an** admin  
**I want to** update an order status from the admin UI  
**So that** I do not need external tools to complete order management.

### Acceptance Criteria
- [ ] AC-001: Given an admin on All Orders page, when they choose a new status and submit, then the UI calls the status update API and shows success feedback.
- [ ] AC-002: Given a failed update, when API returns error, then UI shows descriptive error without losing filters or current page context.
- [ ] AC-003: Given a status change, when update succeeds, then refreshed row displays the new status badge and fulfilled timestamp where applicable.
- [ ] AC-004: Given a non-admin or unauthenticated user, when page action is attempted, then update controls are not available and request is rejected.

### Non-Functional Requirements
- Performance: UI update round-trip completes in < 2 seconds for normal network conditions.
- Security: UI must never bypass server authorization for status updates.
- Scalability: UI supports repeated updates on large pages without full-page instability.

### Affected Modules
| Module | File | Change Type | Risk |
|---|---|---|---|
| Web UI | src/MyProject.Web/Pages/Orders/AllOrders.cshtml | Add status update controls | High |
| Web UI | src/MyProject.Web/Pages/Orders/AllOrdersModel.cs | Add post handler for update action | High |
| Web UI | src/MyProject.Web/Services/InventoryApiService.cs | Add typed method for update status endpoint | Medium |
| API | src/MyProject.Api/Endpoints/OrderEndpoints.cs | API contract dependency | Medium |

### Recommended NuGet Packages
**DEP-RECOMMENDATION**: PRESENT — No new packages required.

| DEP-ID | Package | Min Version | Purpose | Justification | Alternatives Considered | Licence | Already in Project? |
|---|---|---|---|---|---|---|---|
| DEP-001 | ASP.NET Core Razor Pages stack | net8.0 | UI workflow | Existing web front-end baseline | SPA rewrite (rejected: out of scope) | MIT | Yes |

### Other Dependencies
- DEP-105: API route for status update must be available and stable before UI delivery.

### Assumptions
- ASM-005: Admin page keeps filter and pagination query state after postback.

### Linked Artifacts
- Design: ADR-ORDERS-001
- Tests: TC-ORD-015, TC-ORD-016, TC-ORD-017
- Implementation: PR-TBD
