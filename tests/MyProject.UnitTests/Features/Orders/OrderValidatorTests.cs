#nullable enable

using FluentAssertions;
using MyProject.Application.Features.Orders.DTOs;
using MyProject.Application.Features.Orders.Validators;

namespace MyProject.UnitTests.Features.Orders;

/// <summary>Unit tests for Orders request validators.</summary>
[TestClass]
public class OrderValidatorTests
{
    private readonly CreateOrderRequestValidator _createValidator = new();

    [TestMethod]
    public async Task CreateOrderRequestValidator_WithValidInput_Passes()
    {
        var request = new CreateOrderRequest(
            InventoryItemId: Guid.NewGuid(),
            QuantityRequested: 5,
            Notes: "Test order");

        var result = await _createValidator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public async Task CreateOrderRequestValidator_WithQuantityZero_Fails()
    {
        var request = new CreateOrderRequest(
            InventoryItemId: Guid.NewGuid(),
            QuantityRequested: 0);

        var result = await _createValidator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "QuantityRequested");
    }

    [TestMethod]
    public async Task CreateOrderRequestValidator_WithNegativeQuantity_Fails()
    {
        var request = new CreateOrderRequest(
            InventoryItemId: Guid.NewGuid(),
            QuantityRequested: -1);

        var result = await _createValidator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "QuantityRequested");
    }

    [TestMethod]
    public async Task CreateOrderRequestValidator_WithQuantityAbove999_Fails()
    {
        var request = new CreateOrderRequest(
            InventoryItemId: Guid.NewGuid(),
            QuantityRequested: 1000);

        var result = await _createValidator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "QuantityRequested");
    }

    [TestMethod]
    public async Task CreateOrderRequestValidator_WithQuantity999_Passes()
    {
        var request = new CreateOrderRequest(
            InventoryItemId: Guid.NewGuid(),
            QuantityRequested: 999);

        var result = await _createValidator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public async Task CreateOrderRequestValidator_WithLongNotes_Fails()
    {
        var request = new CreateOrderRequest(
            InventoryItemId: Guid.NewGuid(),
            QuantityRequested: 5,
            Notes: new string('a', 501));

        var result = await _createValidator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Notes");
    }

    [TestMethod]
    public async Task CreateOrderRequestValidator_WithEmptyInventoryId_Fails()
    {
        var request = new CreateOrderRequest(
            InventoryItemId: Guid.Empty,
            QuantityRequested: 5);

        var result = await _createValidator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "InventoryItemId");
    }
}
