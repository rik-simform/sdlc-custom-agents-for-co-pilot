#nullable enable

using Microsoft.EntityFrameworkCore;
using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;
using MyProject.Infrastructure.Data;

namespace MyProject.Infrastructure.Repositories;

/// <summary>EF Core implementation of <see cref="IInventoryRepository"/>.</summary>
public class InventoryRepository(AppDbContext context) : IInventoryRepository
{
    public async Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.InventoryItems
            .Include(i => i.Creator)
            .FirstOrDefaultAsync(i => i.Id == id, ct)
            .ConfigureAwait(false);

    public async Task<InventoryItem?> GetBySkuAsync(string sku, CancellationToken ct = default) =>
        await context.InventoryItems
            .FirstOrDefaultAsync(i => i.Sku == sku, ct)
            .ConfigureAwait(false);

    public async Task<IEnumerable<InventoryItem>> GetAllAsync(
        string? category = null,
        string? searchTerm = null,
        CancellationToken ct = default)
    {
        var query = context.InventoryItems
            .Where(i => i.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(i => i.Category == category);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(i =>
                i.Name.ToLower().Contains(term) ||
                i.Sku.ToLower().Contains(term) ||
                (i.Description != null && i.Description.ToLower().Contains(term)));
        }

        return await query.OrderBy(i => i.Name).ToListAsync(ct).ConfigureAwait(false);
    }

    public async Task<IEnumerable<InventoryItem>> GetItemsNeedingReorderAsync(CancellationToken ct = default) =>
        await context.InventoryItems
            .Where(i => i.IsActive && i.QuantityInStock <= i.ReorderLevel)
            .OrderBy(i => i.QuantityInStock)
            .ToListAsync(ct)
            .ConfigureAwait(false);

    public async Task AddAsync(InventoryItem item, CancellationToken ct = default)
    {
        await context.InventoryItems.AddAsync(item, ct).ConfigureAwait(false);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task UpdateAsync(InventoryItem item, CancellationToken ct = default)
    {
        context.InventoryItems.Update(item);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default) =>
        await context.InventoryItems
            .Where(i => i.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(i => i.IsActive, false)
                .SetProperty(i => i.UpdatedAt, DateTimeOffset.UtcNow), ct)
            .ConfigureAwait(false);
}
