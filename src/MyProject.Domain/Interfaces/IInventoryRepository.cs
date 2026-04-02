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
