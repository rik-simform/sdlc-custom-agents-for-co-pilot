#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;
using MyProject.Infrastructure.Data;

namespace MyProject.Infrastructure.Repositories;

/// <summary>EF Core implementation of <see cref="IInventoryRepository"/>.</summary>
public class InventoryRepository(AppDbContext context, ILogger<InventoryRepository> logger) : IInventoryRepository
{
    public async Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            return await context.InventoryItems
                .Include(i => i.Creator)
                .FirstOrDefaultAsync(i => i.Id == id, ct)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException) { throw; }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to retrieve inventory item {ItemId}", id);
            throw;
        }
    }

    public async Task<InventoryItem?> GetBySkuAsync(string sku, CancellationToken ct = default)
    {
        try
        {
            return await context.InventoryItems
                .FirstOrDefaultAsync(i => i.Sku == sku, ct)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException) { throw; }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to retrieve inventory item by SKU {Sku}", sku);
            throw;
        }
    }

    public async Task<IEnumerable<InventoryItem>> GetAllAsync(
        string? category = null,
        string? searchTerm = null,
        CancellationToken ct = default)
    {
        try
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
        catch (OperationCanceledException) { throw; }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to retrieve inventory items");
            throw;
        }
    }

    public async Task<IEnumerable<InventoryItem>> GetItemsNeedingReorderAsync(CancellationToken ct = default)
    {
        try
        {
            return await context.InventoryItems
                .Where(i => i.IsActive && i.QuantityInStock <= i.ReorderLevel)
                .OrderBy(i => i.QuantityInStock)
                .ToListAsync(ct)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException) { throw; }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to retrieve items needing reorder");
            throw;
        }
    }

    public async Task AddAsync(InventoryItem item, CancellationToken ct = default)
    {
        try
        {
            await context.InventoryItems.AddAsync(item, ct).ConfigureAwait(false);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        catch (OperationCanceledException) { throw; }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to add inventory item {ItemId}", item.Id);
            throw;
        }
    }

    public async Task UpdateAsync(InventoryItem item, CancellationToken ct = default)
    {
        try
        {
            context.InventoryItems.Update(item);
            await context.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        catch (OperationCanceledException) { throw; }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to update inventory item {ItemId}", item.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            await context.InventoryItems
                .Where(i => i.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(i => i.IsActive, false)
                    .SetProperty(i => i.UpdatedAt, DateTimeOffset.UtcNow), ct)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException) { throw; }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to delete inventory item {ItemId}", id);
            throw;
        }
    }
}