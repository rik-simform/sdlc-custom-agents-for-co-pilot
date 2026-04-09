using MyProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.Domain.Interfaces
{
    /// <summary>
    /// Repository contract for orders.
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>Gets an order by ID with related entities.</summary>
        Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);

        /// <summary>Gets paginated orders for a specific user, ordered by date descending.</summary>
        Task<(IEnumerable<Order> Items, int TotalCount)> GetByUserIdAsync(
            string userId,
            int skip = 0,
            int take = 10,
            CancellationToken ct = default);

        /// <summary>Gets all orders paginated, with optional status and user filtering.</summary>
        Task<(IEnumerable<Order> Items, int TotalCount)> GetAllAsync(
            int skip = 0,
            int take = 10,
            string? status = null,
            string? userFilter = null,
            CancellationToken ct = default);

        /// <summary>Gets orders filtered by status.</summary>
        Task<IEnumerable<Order>> GetByStatusAsync(string status, CancellationToken ct = default);

        /// <summary>Adds a new order to the repository.</summary>
        Task AddAsync(Order order, CancellationToken ct = default);

        /// <summary>Updates an existing order in the repository.</summary>
        Task UpdateAsync(Order order, CancellationToken ct = default);

        /// <summary>Deletes an order by ID.</summary>
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
