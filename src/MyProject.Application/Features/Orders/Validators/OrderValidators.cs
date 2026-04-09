#nullable enable

using FluentValidation;
using MyProject.Application.Features.Orders.DTOs;

namespace MyProject.Application.Features.Orders.Validators;

/// <summary>Validates <see cref="CreateOrderRequest"/>.</summary>
public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.InventoryItemId)
            .NotEmpty().WithMessage("Inventory item ID is required");

        RuleFor(x => x.QuantityRequested)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(999).WithMessage("Quantity must not exceed 999");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters");
    }
}
