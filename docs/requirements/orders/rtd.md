# REQ-CLASS: CROSS-CUTTING

# Requirements Traceability Document — Orders Module

**Generated**: 2026-04-09  
**Author**: SDLC Requirements Engineer Agent  
**Version**: 1.0

## Pre-Analysis Summary

**REQ-CLASS**: CROSS-CUTTING

### Affected Modules

| Area | Affected File / Module | Impact Level (None/Low/Medium/High) | Description |
|---|---|---|---|
| Existing endpoints | src/MyProject.Api/Endpoints/OrderEndpoints.cs | High | Missing admin status update route in order API group. |
| Entities / data model | src/MyProject.Domain/Entities/Order.cs | High | Status value definitions are inconsistent across modules. |
| Services / handlers | src/MyProject.Application/Features/Orders/Commands/OrderCommands.cs | High | No status update command in Orders feature path. |
| Auth / authorization | src/MyProject.Api/Program.cs | Medium | Admin policy exists but must be applied to new route. |
| Configuration | src/MyProject.Api/appsettings.json | None | No new config keys required for baseline scope. |
| DI registration | src/MyProject.Application/DependencyInjection.cs | Low | New handler auto-registered if in same assembly. |
| Middleware / pipeline | src/MyProject.Api/Program.cs | None | Existing auth middleware order is valid. |
| Tests | tests/MyProject.UnitTests/Features/Orders | Medium | No tests yet for status update endpoint and UI action flow. |
| Migrations | src/MyProject.Infrastructure/Migrations | Low | Existing orders table supports status column updates. |

### Ambiguities

| AMB-ID | Area | Ambiguity Description | Risk Level (Low/Med/High/Critical) | Business Impact if Unresolved | Proposed Resolution |
|---|---|---|---|---|---|
| AMB-001 | API Contract | Canonical status values differ across code paths (Processing, Shipped, Delivered, Approved, Fulfilled). | High | Admin update may set values that break UI filters and reports. | Standardize to Pending, Processing, Fulfilled, Cancelled for this epic. |
| AMB-002 | Architecture | Update command exists under Inventory feature namespace instead of Orders feature path. | High | Duplicate logic and divergent behavior over time. | Move or re-implement update command in Orders feature and expose via OrderEndpoints. |
| AMB-003 | Testing | UI automation approach for admin status update is not defined. | Medium | Regression risk in UI actions and authorization behavior. | Add focused UI tests in sprint plan using existing test stack. |

### Blockers (Must Resolve Before Implementation)

| BLK-ID | Description | Owner | Required By |
|---|---|---|---|
| BLK-001 | Confirm canonical order statuses for API, validators, and UI badges. | Product Owner + Tech Lead | Sprint planning |
| BLK-002 | Confirm status update endpoint location and contract in Order API. | Tech Lead | Sprint planning |

### Decision: Proceed or Hold?

- HOLD — Stories are drafted with [DRAFT] status until BLK-001 and BLK-002 are approved.

## 1. Summary of Ambiguities

AMB-001 and AMB-002 are high-risk and directly affect implementation behavior.
AMB-003 is medium-risk and can proceed with planned test assumptions.

## 2. Impact Analysis on Current Architecture and Flows

| Layer | Area | File / Module | Impact |
|---|---|---|---|
| API | Endpoint contract | src/MyProject.Api/Endpoints/OrderEndpoints.cs | New status update route and auth policy required |
| Application | Command handlers | src/MyProject.Application/Features/Orders/Commands/OrderCommands.cs | Add update status command and transition logic |
| Domain | Status model | src/MyProject.Domain/Entities/Order.cs | Align status vocabulary in documentation and code |
| Infrastructure | Repository updates | src/MyProject.Infrastructure/Repositories/OrderRepository.cs | Existing UpdateAsync supports requirement |
| Web | Admin page actions | src/MyProject.Web/Pages/Orders/AllOrdersModel.cs | Add post action for status updates |
| Web | Admin page UI | src/MyProject.Web/Pages/Orders/AllOrders.cshtml | Add status selector/action and feedback |
| Tests | Unit/integration/UI | tests/MyProject.UnitTests/Features/Orders | Add new coverage for update status scenario |

## 3. Clarifying Questions (Grouped by Area)

### API Contract

- CQ-001: Should status update endpoint be PUT /api/v1/orders/{orderId}/status?
  Risk if unanswered: High.
  Would change design: Yes.
  Would override current flow: Yes.
  Suggested default if stakeholder is unavailable: Yes, use PUT /api/v1/orders/{orderId}/status.

### Security

- CQ-002: Should every admin status update be audit logged with user identity and previous status?
  Risk if unanswered: Medium.
  Would change design: Yes.
  Would override current flow: No.
  Suggested default if stakeholder is unavailable: Log update actor, order ID, old/new status.

### Testing

- CQ-003: Is UI automation required in this sprint or can manual verification satisfy DoD initially?
  Risk if unanswered: Medium.
  Would change design: No.
  Would override current flow: No.
  Suggested default if stakeholder is unavailable: Include at least one automated UI smoke test.

## 4. Proposed Resolutions and Design Notes

| AMB-ID | Proposed Resolution | Rationale | ADR Reference |
|---|---|---|---|
| AMB-001 | Canonical status set to Pending, Processing, Fulfilled, Cancelled. | Aligns with existing domain comments and current order command behavior. | ADR-ORDERS-001 |
| AMB-002 | Implement status update in Orders feature and map endpoint in OrderEndpoints. | Keeps order logic in one bounded context path. | ADR-ORDERS-001 |
| AMB-003 | Add planned tests TC-ORD-015..017 for UI update behavior. | Reduces regression risk for admin actions. | ADR-ORDERS-001 |

## 5. User Stories with Acceptance Criteria

See:
- US-ORD-001
- US-ORD-002
- US-ORD-003
- US-ORD-004
- US-ORD-005

## 6. Assumptions and Constraints

| ID | Text | Category | Impact if Wrong |
|---|---|---|---|
| ASM-001 | Inventory stock validation remains in create order command handler. | Architecture | Order placement may bypass stock checks. |
| ASM-002 | Existing routing paths for My Orders and All Orders remain stable. | UI | Navigation and links may break. |
| ASM-003 | JWT role claim is consistently populated for Admin users. | Security | Admin pages may deny valid users or allow invalid ones. |
| ASM-004 | FulfilledAt is set only when status is Fulfilled. | Data model | Reports may show incorrect completion timestamps. |

## 7. Blockers

| BLK-ID | Description | Owner | Required By |
|---|---|---|---|
| BLK-001 | Confirm canonical status vocabulary. | Product Owner + Tech Lead | Sprint planning |
| BLK-002 | Confirm final status update endpoint contract. | Tech Lead | Sprint planning |

## 8. Requirements Traceability Matrix

See traceability artifact in docs/requirements/orders/traceability-matrix.md.

## 9. Recommended Dependencies

**DEP-RECOMMENDATION**: PRESENT

| DEP-ID | Package | Min Version | Purpose | Justification | Alternatives Considered | Licence | Security Notes | Add to Project? |
|---|---|---|---|---|---|---|---|---|
| DEP-001 | MediatR | >= 12.4.1 | CQRS request dispatch | Already used throughout application layer | Custom mediator (rejected: duplicate complexity) | Apache-2.0 | None known | Already present |
| DEP-002 | FluentValidation | >= 11.11.0 | Request validation | Already used for order create request | DataAnnotations (rejected: less expressive for complex rules) | Apache-2.0 | None known | Already present |
| DEP-003 | Microsoft.AspNetCore.Authentication.JwtBearer | >= 8.0.12 | API authentication and role checks | Existing security baseline | Cookie-only auth (rejected: API uses bearer token flows) | MIT | None known | Already present |
| DEP-004 | Microsoft.EntityFrameworkCore.Sqlite | >= 8.0.12 | Persistence provider | Existing repository implementation uses EF Core | Dapper (rejected: would create mixed data patterns) | MIT | None known | Already present |
