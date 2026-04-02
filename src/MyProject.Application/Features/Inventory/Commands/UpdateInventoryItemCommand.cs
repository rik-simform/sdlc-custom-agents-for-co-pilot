#nullable enable

using MediatR;
using Microsoft.Extensions.Logging;
using MyProject.Application.Common;
using MyProject.Application.Features.Inventory.DTOs;
using MyProject.Application.Features.Inventory.Queries;
using MyProject.Domain.Interfaces;

namespace MyProject.Application.Features.Inventory.Commands;

/// <summary>Command to update an existing inventory item.</summary>
public record UpdateInventoryItemCommand(
    Guid Id,
    string Name,
    string? Description,
    string Category,
    int QuantityInStock,
    int ReorderLevel,
    decimal UnitPrice,
    string? Location,
    bool IsActive,
    string UserId)
    : IRequest<Result<InventoryItemResponse>>;

/// <summary>Handles <see cref="UpdateInventoryItemCommand"/>.</summary>
public class UpdateInventoryItemCommandHandler(
    IInventoryRepository inventoryRepository,
    ILogger<UpdateInventoryItemCommandHandler> logger)
    : IRequestHandler<UpdateInventoryItemCommand, Result<InventoryItemResponse>>
{
    public async Task<Result<InventoryItemResponse>> Handle(UpdateInventoryItemCommand request, CancellationToken ct)
    {
        var item = await inventoryRepository.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
        if (item is null)
        {
            return Result<InventoryItemResponse>.Fail("Inventory item not found");
        }

        item.Name = request.Name;
        item.Description = request.Description;
        item.Category = request.Category;
        item.QuantityInStock = request.QuantityInStock;
        item.ReorderLevel = request.ReorderLevel;
        item.UnitPrice = request.UnitPrice;
        item.Location = request.Location;
        item.IsActive = request.IsActive;
        item.UpdatedBy = request.UserId;
        item.UpdatedAt = DateTimeOffset.UtcNow;

        await inventoryRepository.UpdateAsync(item, ct).ConfigureAwait(false);

        logger.LogInformation("Inventory item {Id} updated by user {UserId}", item.Id, request.UserId);

        return Result<InventoryItemResponse>.Ok(item.ToResponse());
    }
}
