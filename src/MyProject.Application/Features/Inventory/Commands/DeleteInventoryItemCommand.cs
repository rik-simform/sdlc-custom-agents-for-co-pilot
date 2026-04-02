#nullable enable

using MediatR;
using Microsoft.Extensions.Logging;
using MyProject.Application.Common;
using MyProject.Domain.Interfaces;

namespace MyProject.Application.Features.Inventory.Commands;

/// <summary>Command to soft-delete an inventory item.</summary>
public record DeleteInventoryItemCommand(Guid Id, string UserId)
    : IRequest<Result<bool>>;

/// <summary>Handles <see cref="DeleteInventoryItemCommand"/>.</summary>
public class DeleteInventoryItemCommandHandler(
    IInventoryRepository inventoryRepository,
    ILogger<DeleteInventoryItemCommandHandler> logger)
    : IRequestHandler<DeleteInventoryItemCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteInventoryItemCommand request, CancellationToken ct)
    {
        var item = await inventoryRepository.GetByIdAsync(request.Id, ct).ConfigureAwait(false);
        if (item is null)
        {
            return Result<bool>.Fail("Inventory item not found");
        }

        await inventoryRepository.DeleteAsync(request.Id, ct).ConfigureAwait(false);

        logger.LogInformation("Inventory item {Id} deleted by user {UserId}", request.Id, request.UserId);

        return Result<bool>.Ok(true);
    }
}
