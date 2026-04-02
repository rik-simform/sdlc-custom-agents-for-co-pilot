#nullable enable

using FluentAssertions;
using FluentValidation;
using MyProject.Application.Features.Auth.DTOs;
using MyProject.Application.Features.Auth.Validators;
using MyProject.Application.Features.Inventory.DTOs;
using MyProject.Application.Features.Inventory.Validators;

namespace MyProject.UnitTests.Features;

/// <summary>
/// Unit tests for RBAC-related validators covering US-RBAC-001 AC-002, US-RBAC-002 AC-003,
/// and US-RBAC-006 validation requirements.
/// </summary>
[TestClass]
public class RbacValidatorTests
{
    // ─── CreateInventoryItemRequest validation (US-RBAC-001 AC-002) ──────────

    [TestMethod]
    public async Task CreateValidator_MissingName_HasError()
    {
        var validator = new CreateInventoryItemRequestValidator();
        var request = new CreateInventoryItemRequest("SKU-1", "", null, "Parts", 10, 5, 9.99m, null);

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [TestMethod]
    public async Task CreateValidator_NegativePrice_HasError()
    {
        var validator = new CreateInventoryItemRequestValidator();
        var request = new CreateInventoryItemRequest("SKU-1", "Item", null, "Parts", 10, 5, -1m, null);

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UnitPrice");
    }

    [TestMethod]
    public async Task CreateValidator_NegativeQuantity_HasError()
    {
        var validator = new CreateInventoryItemRequestValidator();
        var request = new CreateInventoryItemRequest("SKU-1", "Item", null, "Parts", -5, 5, 9.99m, null);

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "QuantityInStock");
    }

    [TestMethod]
    public async Task CreateValidator_ValidRequest_NoErrors()
    {
        var validator = new CreateInventoryItemRequestValidator();
        var request = new CreateInventoryItemRequest("SKU-001", "Widget", "Desc", "Parts", 10, 5, 9.99m, "Shelf A");

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    // ─── UpdateInventoryItemRequest validation (US-RBAC-002 AC-003) ──────────

    [TestMethod]
    public async Task UpdateValidator_MissingName_HasError()
    {
        var validator = new UpdateInventoryItemRequestValidator();
        var request = new UpdateInventoryItemRequest("", null, "Parts", 10, 5, 9.99m, null, true);

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [TestMethod]
    public async Task UpdateValidator_NegativePrice_HasError()
    {
        var validator = new UpdateInventoryItemRequestValidator();
        var request = new UpdateInventoryItemRequest("Item", null, "Parts", 10, 5, -1m, null, true);

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UnitPrice");
    }

    [TestMethod]
    public async Task UpdateValidator_ValidRequest_NoErrors()
    {
        var validator = new UpdateInventoryItemRequestValidator();
        var request = new UpdateInventoryItemRequest("Widget", "Desc", "Parts", 10, 5, 9.99m, "Shelf A", true);

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }

    // ─── AssignRoleRequest validation (US-RBAC-006) ──────────────────────────

    [TestMethod]
    public async Task AssignRoleValidator_MissingUserId_HasError()
    {
        var validator = new AssignRoleRequestValidator();
        var request = new AssignRoleRequest("", "Admin");

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    [TestMethod]
    public async Task AssignRoleValidator_MissingRole_HasError()
    {
        var validator = new AssignRoleRequestValidator();
        var request = new AssignRoleRequest("user-id", "");

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Role");
    }

    [TestMethod]
    public async Task AssignRoleValidator_ValidRequest_NoErrors()
    {
        var validator = new AssignRoleRequestValidator();
        var request = new AssignRoleRequest("user-123", "Admin");

        var result = await validator.ValidateAsync(request);

        result.IsValid.Should().BeTrue();
    }
}
