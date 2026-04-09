#nullable enable

using FluentAssertions;
using MyProject.Domain.Entities;
using MyProject.Infrastructure.Repositories;

namespace MyProject.UnitTests.Features.Orders;

/// <summary>Unit tests for Orders repository data access methods.</summary>
[TestClass]
public class OrderRepositoryTests
{
    [TestMethod]
    public void OrderRepository_HasRequiredMethods()
    {
        // Arrange & Act
        var repositoryType = typeof(OrderRepository);
        var methods = repositoryType.GetMethods();

        // Assert
        methods.Should().Contain(m => m.Name == nameof(OrderRepository.GetByIdAsync));
        methods.Should().Contain(m => m.Name == nameof(OrderRepository.GetByUserIdAsync));
        methods.Should().Contain(m => m.Name == nameof(OrderRepository.GetAllAsync));
        methods.Should().Contain(m => m.Name == nameof(OrderRepository.GetByStatusAsync));
        methods.Should().Contain(m => m.Name == nameof(OrderRepository.AddAsync));
        methods.Should().Contain(m => m.Name == nameof(OrderRepository.UpdateAsync));
        methods.Should().Contain(m => m.Name == nameof(OrderRepository.DeleteAsync));
    }
}

/// <summary>Test utilities for Orders tests.</summary>
public static class OrderTestDataBuilder
{
    /// <summary>Creates a default test order.</summary>
    public static Order CreateTestOrder(
        Guid? id = null,
        string? userId = null,
        Guid? inventoryItemId = null,
        int quantityRequested = 5,
        string status = "Pending",
        string? itemName = "Test Item") => new()
    {
        Id = id ?? Guid.NewGuid(),
        UserId = userId ?? "test-user",
        InventoryItemId = inventoryItemId ?? Guid.NewGuid(),
        QuantityRequested = quantityRequested,
        Status = status,
        OrderedAt = DateTimeOffset.UtcNow,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
        InventoryItem = new InventoryItem { Id = inventoryItemId ?? Guid.NewGuid(), Name = itemName }
    };

    /// <summary>Creates a test order with user information.</summary>
    public static Order CreateTestOrderWithUser(
        string userName = "TestUser",
        string userEmail = "test@example.com") => new()
    {
        Id = Guid.NewGuid(),
        UserId = "test-user-id",
        InventoryItemId = Guid.NewGuid(),
        QuantityRequested = 5,
        Status = "Pending",
        OrderedAt = DateTimeOffset.UtcNow,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
        User = new() { Id = "test-user-id", UserName = userName, Email = userEmail },
        InventoryItem = new() { Id = Guid.NewGuid(), Name = "Test Item" }
    };
}

/// <summary>Data consistency tests for Orders entity.</summary>
[TestClass]
public class OrderDataConsistencyTests
{
    [TestMethod]
    public void Order_WithValidData_ShouldHaveAllRequiredProperties()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = "user-123";
        var inventoryItemId = Guid.NewGuid();

        // Act
        var order = new Order
        {
            Id = orderId,
            UserId = userId,
            InventoryItemId = inventoryItemId,
            QuantityRequested = 5,
            Status = "Pending",
            OrderedAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        // Assert
        order.Id.Should().Be(orderId);
        order.UserId.Should().Be(userId);
        order.InventoryItemId.Should().Be(inventoryItemId);
        order.QuantityRequested.Should().Be(5);
        order.Status.Should().Be("Pending");
        order.OrderedAt.Should().NotBe(default);
        order.CreatedAt.Should().NotBe(default);
        order.UpdatedAt.Should().NotBe(default);
    }

    [TestMethod]
    public void Order_StatusTransitions_AreValid()
    {
        // Arrange
        var order = OrderTestDataBuilder.CreateTestOrder(status: "Pending");

        // Act & Assert - Valid transition
        order.Status = "Fulfilled";
        order.Status.Should().Be("Fulfilled");

        // Act & Assert - Another valid transition
        order.Status = "Cancelled";
        order.Status.Should().Be("Cancelled");
    }

    [TestMethod]
    public void Order_WithNullInventoryItem_ShouldStillBeCreatedSuccessfully()
    {
        // Arrange & Act
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = "user-123",
            InventoryItemId = Guid.NewGuid(),
            QuantityRequested = 5,
            Status = "Pending",
            OrderedAt = DateTimeOffset.UtcNow,
            InventoryItem = null
        };

        // Assert
        order.InventoryItem.Should().BeNull();
        order.InventoryItemId.Should().NotBe(Guid.Empty);
    }

    [TestMethod]
    public void Order_WithZeroQuantity_IsAllowedAtEntityLevel()
    {
        // Arrange & Act
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = "user-123",
            InventoryItemId = Guid.NewGuid(),
            QuantityRequested = 0,  // Entity doesn't prevent this
            Status = "Pending",
            OrderedAt = DateTimeOffset.UtcNow
        };

        // Assert - Entity allows it (validation happens at command handler level)
        order.QuantityRequested.Should().Be(0);
    }

    [TestMethod]
    public void Order_FulfilledDate_CanBeNull()
    {
        // Arrange & Act
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = "user-123",
            InventoryItemId = Guid.NewGuid(),
            QuantityRequested = 5,
            Status = "Pending",
            OrderedAt = DateTimeOffset.UtcNow,
            FulfilledAt = null  // Should be nullable
        };

        // Assert
        order.FulfilledAt.Should().BeNull();
    }
}
