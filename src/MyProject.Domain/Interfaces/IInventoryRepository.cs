#nullable enable

using MyProject.Domain.Entities;

namespace MyProject.Domain.Interfaces;

/// <summary>
/// Repository contract for inventory items.
/// </summary>
public interface IInventoryRepository
{
    Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<InventoryItem?> GetBySkuAsync(string sku, CancellationToken ct = default);
    Task<IEnumerable<InventoryItem>> GetAllAsync(string? category = null, string? searchTerm = null, CancellationToken ct = default);
    Task<IEnumerable<InventoryItem>> GetItemsNeedingReorderAsync(CancellationToken ct = default);
    Task AddAsync(InventoryItem item, CancellationToken ct = default);
    Task UpdateAsync(InventoryItem item, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

/// <summary>
/// Repository contract for orders.
/// </summary>
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Order>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Order>> GetByStatusAsync(string status, CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    Task UpdateAsync(Order order, CancellationToken ct = default);
}
