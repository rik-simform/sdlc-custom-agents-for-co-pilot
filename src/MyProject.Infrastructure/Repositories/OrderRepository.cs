using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;
using MyProject.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Infrastructure.Repositories
{
    /// <summary>Repository for Order operations.</summary>
    public class OrderRepository(AppDbContext context, ILogger<OrderRepository> logger) : IOrderRepository
    {
        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                return await context.Orders
                    .Include(o => o.User)
                    .Include(o => o.InventoryItem)
                    .FirstOrDefaultAsync(o => o.Id == id, ct)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) { throw; }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, "Failed to retrieve order {OrderId}", id);
                throw;
            }
        }

        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetByUserIdAsync(
            string userId,
            int skip = 0,
            int take = 10,
            CancellationToken ct = default)
        {
            try
            {
                var query = context.Orders
                    .Where(o => o.UserId == userId)
                    .Include(o => o.InventoryItem);

                var totalCount = await query.CountAsync(ct).ConfigureAwait(false);
                var items = await query
                    .OrderByDescending(o => o.OrderedAt)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync(ct)
                    .ConfigureAwait(false);

                return (items, totalCount);
            }
            catch (OperationCanceledException) { throw; }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, "Failed to retrieve orders for user {UserId}", userId);
                throw;
            }
        }

        public async Task<(IEnumerable<Order> Items, int TotalCount)> GetAllAsync(
            int skip = 0,
            int take = 10,
            string? status = null,
            string? userFilter = null,
            CancellationToken ct = default)
        {
            try
            {
                var query = context.Orders
                    .Include(o => o.User)
                    .Include(o => o.InventoryItem)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(status))
                    query = query.Where(o => o.Status == status);

                if (!string.IsNullOrWhiteSpace(userFilter))
                {
                    var filterLower = userFilter.ToLower();
                    query = query.Where(o =>
                        (o.User.UserName != null && o.User.UserName.ToLower().Contains(filterLower)) ||
                        (o.User.Email != null && o.User.Email.ToLower().Contains(filterLower)));
                }

                var totalCount = await query.CountAsync(ct).ConfigureAwait(false);
                var items = await query
                    .OrderByDescending(o => o.OrderedAt)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync(ct)
                    .ConfigureAwait(false);

                return (items, totalCount);
            }
            catch (OperationCanceledException) { throw; }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, "Failed to retrieve all orders");
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(string status, CancellationToken ct = default)
        {
            try
            {
                return await context.Orders
                    .Where(o => o.Status == status)
                    .Include(o => o.User)
                    .Include(o => o.InventoryItem)
                    .OrderBy(o => o.OrderedAt)
                    .ToListAsync(ct)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) { throw; }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, "Failed to retrieve orders with status {Status}", status);
                throw;
            }
        }

        public async Task AddAsync(Order order, CancellationToken ct = default)
        {
            try
            {
                await context.Orders.AddAsync(order, ct).ConfigureAwait(false);
                await context.SaveChangesAsync(ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException) { throw; }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Failed to add order {OrderId}", order.Id);
                throw;
            }
        }

        public async Task UpdateAsync(Order order, CancellationToken ct = default)
        {
            try
            {
                order.UpdatedAt = DateTimeOffset.UtcNow;
                context.Orders.Update(order);
                await context.SaveChangesAsync(ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException) { throw; }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Failed to update order {OrderId}", order.Id);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            try
            {
                await context.Orders
                    .Where(o => o.Id == id)
                    .ExecuteDeleteAsync(ct)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) { throw; }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Failed to delete order {OrderId}", id);
                throw;
            }
        }
    }
}
