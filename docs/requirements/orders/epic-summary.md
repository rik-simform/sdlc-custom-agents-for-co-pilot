# REQ-CLASS: CROSS-CUTTING

# EPIC-ORD: Order Module Rewrite Summary

**Date**: 2026-04-09  
**Status**: Drafted for Product Owner review  
**Epic Owner**: Product Owner (TBD)

## Story Inventory

| ID | Title | Priority | SP | Status | Notes |
|---|---|---|---|---|---|
| US-ORD-001 | Customer Places Order | Critical | 5 | [DRAFT] | Core order creation |
| US-ORD-002 | Customer Views Own Orders In UI | High | 3 | [DRAFT] | UI visibility for user |
| US-ORD-003 | Admin Views All Orders In UI | Critical | 5 | [DRAFT] | Admin monitoring surface |
| US-ORD-004 | Admin Updates Order Status (API) | Critical | 5 | [DRAFT] | New API capability required |
| US-ORD-005 | Admin Updates Order Status In UI | High | 3 | [DRAFT] | UI workflow for status update |

## Totals

| Metric | Value |
|---|---|
| Total stories | 5 |
| Total story points | 21 |
| Critical stories | 3 |
| High stories | 2 |

## Scope Alignment To Request

- Customers can place orders.
- Admins can view all placed orders.
- Admins can update order statuses.
- Both customer and admin capabilities are visible in UI surfaces.

## Current Delivery Risk

| Risk | Level | Description |
|---|---|---|
| Status vocabulary mismatch | High | Current modules use Processing, Shipped, Delivered, Approved, and Fulfilled inconsistently. |
| Missing order status update endpoint in order API group | High | Existing update command is in inventory feature path and not exposed from order endpoints. |

## Backlog (Post-MVP)

| Candidate | Priority | Reason |
|---|---|---|
| User cancel pending order | Medium | Existing capability can be retained but not requested in rewrite scope. |
| Export orders CSV/PDF | Low | Operational convenience feature. |
| Status transition audit report | Medium | Compliance reporting. |
