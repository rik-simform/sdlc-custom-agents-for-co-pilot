# REQ-CLASS: CROSS-CUTTING

# Requirements Traceability Matrix — EPIC-ORD

Last Updated: 2026-04-09

## Forward Traceability

| Req ID | User Story | REQ-CLASS | Affected Modules | Design | Implementation | Test | Blockers | Status |
|---|---|---|---|---|---|---|---|---|
| REQ-ORD-001 | US-ORD-001 | CROSS-CUTTING | API, Application, Infrastructure, Web | ADR-ORDERS-001 | PR-TBD | TC-ORD-001..004 | BLK-002 | Draft |
| REQ-ORD-002 | US-ORD-002 | CROSS-CUTTING | API, Application, Web | ADR-ORDERS-001 | PR-TBD | TC-ORD-005..007 | BLK-001 | Draft |
| REQ-ORD-003 | US-ORD-003 | CROSS-CUTTING | API, Application, Infrastructure, Web | ADR-ORDERS-001 | PR-TBD | TC-ORD-008..010 | BLK-001 | Draft |
| REQ-ORD-004 | US-ORD-004 | CROSS-CUTTING | API, Application, Domain, Infrastructure | ADR-ORDERS-001 | PR-TBD | TC-ORD-011..014 | BLK-001, BLK-002 | Draft |
| REQ-ORD-005 | US-ORD-005 | CROSS-CUTTING | Web, API | ADR-ORDERS-001 | PR-TBD | TC-ORD-015..017 | BLK-002 | Draft |

## Backward Traceability

| Test ID | Test Name | Type | Req IDs | Status |
|---|---|---|---|---|
| TC-ORD-001 | PlaceOrder_WithValidRequest_ReturnsCreated | Integration | REQ-ORD-001 | Planned |
| TC-ORD-002 | PlaceOrder_WithInvalidQuantity_ReturnsBadRequest | Unit | REQ-ORD-001 | Planned |
| TC-ORD-003 | PlaceOrder_WithInsufficientStock_ReturnsBadRequest | Integration | REQ-ORD-001 | Planned |
| TC-ORD-004 | PlaceOrder_FromUI_RedirectsToMyOrders | UI | REQ-ORD-001 | Planned |
| TC-ORD-005 | MyOrders_AuthenticatedUser_SeesOwnOrdersOnly | Integration | REQ-ORD-002 | Planned |
| TC-ORD-006 | MyOrders_Pagination_ReturnsExpectedSlice | Integration | REQ-ORD-002 | Planned |
| TC-ORD-007 | MyOrders_Anonymous_RedirectsToLogin | UI | REQ-ORD-002 | Planned |
| TC-ORD-008 | AllOrders_Admin_SeesAllOrders | Integration | REQ-ORD-003 | Planned |
| TC-ORD-009 | AllOrders_NonAdmin_Forbidden | Integration | REQ-ORD-003 | Planned |
| TC-ORD-010 | AllOrders_Filtering_WorksWithPagination | Integration | REQ-ORD-003 | Planned |
| TC-ORD-011 | UpdateStatus_Admin_ValidTransition_ReturnsOk | Integration | REQ-ORD-004 | Planned |
| TC-ORD-012 | UpdateStatus_NonAdmin_Forbidden | Integration | REQ-ORD-004 | Planned |
| TC-ORD-013 | UpdateStatus_InvalidStatus_ReturnsBadRequest | Integration | REQ-ORD-004 | Planned |
| TC-ORD-014 | UpdateStatus_UnknownOrder_ReturnsNotFound | Integration | REQ-ORD-004 | Planned |
| TC-ORD-015 | AllOrdersUI_StatusUpdate_SuccessFeedback | UI | REQ-ORD-005 | Planned |
| TC-ORD-016 | AllOrdersUI_StatusUpdate_ErrorFeedback | UI | REQ-ORD-005 | Planned |
| TC-ORD-017 | AllOrdersUI_NonAdmin_NoStatusControls | UI | REQ-ORD-005 | Planned |
