# EPIC-ORD: Order Management System — Epic Summary

**Date**: 2026-04-08
**Status**: Ready for Sprint Planning
**Epic Owner**: Product Owner (pending assignment)

---

## Story Inventory

| ID | Title | Priority | SP | Sprint | Dependencies |
|----|-------|----------|----|--------|-------------|
| US-ORD-001 | User Browses Inventory and Places Order | Critical | 5 | Sprint 1 | US-LOGIN-001, US-RBAC-004 |
| US-ORD-002 | User Views Their Order History | Critical | 3 | Sprint 1 | US-ORD-001 |
| US-ORD-003 | User Cancels Pending Order | High | 3 | Sprint 1 | US-ORD-001 |
| US-ORD-004 | Admin Views All Orders Dashboard | Critical | 5 | Sprint 1 | US-LOGIN-001, US-RBAC-006 |
| US-ORD-005 | Admin Filters Orders by Status and User | High | 3 | Sprint 1 | US-ORD-004 |

---

## Totals

| Metric | Value |
|--------|-------|
| **Total Stories** | 5 |
| **Total Story Points** | 19 |
| **Total Acceptance Criteria** | 28 |
| **Total Test Cases** | 28 |
| **Critical Stories** | 3 (13 SP) |
| **High Stories** | 2 (6 SP) |

---

## Recommended Sprint Allocation

### Sprint 1 — Core Order Management (19 SP)

| Story | SP | Rationale |
|-------|----|-----------|
| US-ORD-001 | 5 | Foundation — users must be able to place orders |
| US-ORD-004 | 5 | Admin visibility — compliance and order management |
| US-ORD-002 | 3 | User history — track purchases and order status |
| US-ORD-005 | 3 | Admin filtering — support order management workflows |
| US-ORD-003 | 3 | User cancellation — support remorse period / procurement flexibility |

**Sprint Goal**: Complete order placement workflow for users and read-only admin dashboard with filtering — users can place and track orders, admins have visibility.

---

## Backlog (Future Sprints)

| Feature | Priority | SP | Description |
|---------|----------|----|----|
| US-ORD-006 | Medium | 5 | Admin updates order status (Pending → Processing → Fulfilled) |
| US-ORD-007 | Medium | 8 | Backorder handling when inventory unavailable |
| US-ORD-008 | Low | 5 | Email notifications on order status change |
| US-ORD-009 | Low | 3 | Order export to CSV/PDF |

---

## Feature Highlights

✅ **Users place orders** from browse inventory, specify quantity, add optional notes  
✅ **Order tracking** — users see their order history with status and date  
✅ **Admin dashboard** — read-only view of all orders, paginated, filterable  
✅ **Authorization** — users only see/manage own orders; admins see all  
✅ **Database** — new Orders table tracking user, item, quantity, status, timestamps  
✅ **Full stack** — API endpoints, Web UI (Razor Pages), comprehensive tests  

---

## Success Criteria

| Criterion | Target | Achieved |
|-----------|--------|----------|
| Users can place orders | 100% | ❌ |
| Users see order history | 100% | ❌ |
| Users can cancel pending orders | 100% | ❌ |
| Admins see all orders | 100% | ❌ |
| Admin filtering by status | 100% | ❌ |
| All endpoints secured by role | 100% | ❌ |
| Test coverage (Orders feature) | 80%+ | ❌ |
| API response times < 500ms (p95) | 95% | ❌ |

