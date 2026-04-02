#nullable enable

using MediatR;
using Microsoft.Extensions.Logging;
using MyProject.Application.Common;
using MyProject.Application.Features.Inventory.DTOs;
using MyProject.Application.Features.Inventory.Queries;
using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;

namespace MyProject.Application.Features.Inventory.Commands;

/// <summary>Command to create a new inventory item.</summary>
public record CreateInventoryItemCommand(
    string Sku,
    string Name,
    string? Description,
    string Category,
    int QuantityInStock,
    int ReorderLevel,
    decimal UnitPrice,
    string? Location,
    string UserId)
    : IRequest<Result<InventoryItemResponse>>;

/// <summary>Handles <see cref="CreateInventoryItemCommand"/>.</summary>
public class CreateInventoryItemCommandHandler(
    IInventoryRepository inventoryRepository,
    ILogger<CreateInventoryItemCommandHandler> logger)
    : IRequestHandler<CreateInventoryItemCommand, Result<InventoryItemResponse>>
{
    public async Task<Result<InventoryItemResponse>> Handle(CreateInventoryItemCommand request, CancellationToken ct)
    {
        var existing = await inventoryRepository.GetBySkuAsync(request.Sku, ct).ConfigureAwait(false);
        if (existing is not null)
        {
            return Result<InventoryItemResponse>.Fail($"An item with SKU '{request.Sku}' already exists");
        }

        var item = new InventoryItem
        {
            Sku = request.Sku,
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            QuantityInStock = request.QuantityInStock,
            ReorderLevel = request.ReorderLevel,
            UnitPrice = request.UnitPrice,
            Location = request.Location,
            CreatedBy = request.UserId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await inventoryRepository.AddAsync(item, ct).ConfigureAwait(false);

        logger.LogInformation("Inventory item {Sku} created by user {UserId}", item.Sku, request.UserId);

        return Result<InventoryItemResponse>.Ok(item.ToResponse());
    }
}
