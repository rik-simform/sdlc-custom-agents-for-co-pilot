#nullable enable

using FluentValidation;
using MyProject.Application.Features.Inventory.DTOs;

namespace MyProject.Application.Features.Inventory.Validators;

/// <summary>Validates <see cref="CreateOrderRequest"/>.</summary>
public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .NotEmpty().WithMessage("Inventory item ID is required");

        RuleFor(x => x.QuantityRequested)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(10000).WithMessage("Quantity cannot exceed 10,000 units");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters");
    }
}

/// <summary>Validates the update order status request.</summary>
public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    private static readonly string[] ValidStatuses = { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };

    public UpdateOrderStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(x => ValidStatuses.Contains(x))
            .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters");
    }
}
