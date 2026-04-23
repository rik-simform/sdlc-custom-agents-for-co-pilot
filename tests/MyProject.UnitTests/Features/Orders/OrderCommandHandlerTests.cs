#nullable enable

using FluentAssertions;
using Moq;
using MyProject.Application.Features.Orders.Commands;
using MyProject.Application.Features.Orders.DTOs;
using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;

namespace MyProject.UnitTests.Features.Orders;

/// <summary>Unit tests for Orders command handlers.</summary>
[TestClass]
public class OrderCommandHandlerTests
{
    private Mock<IOrderRepository> _mockOrderRepository = null!;
    private Mock<IInventoryRepository> _mockInventoryRepository = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockInventoryRepository = new Mock<IInventoryRepository>();
    }

    #region CreateOrderCommand Tests

    [TestMethod]
    public async Task CreateOrderCommandHandler_WithValidRequest_CreatesOrderSuccessfully()
    {
        // Arrange
        var userId = "user-123";
        var itemId = Guid.NewGuid();
        var inventoryItem = new InventoryItem
        {
            Id = itemId,
            Name = "Laptop",
            QuantityInStock = 10
        };

        _mockInventoryRepository
            .Setup(r => r.GetByIdAsync(itemId, default))
            .ReturnsAsync(inventoryItem);

        var command = new CreateOrderCommand(userId, itemId, 2, "Urgent");
        var handler = new CreateOrderCommandHandler(_mockOrderRepository.Object, _mockInventoryRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Status.Should().Be("Pending");
        result.Value.ItemName.Should().Be("Laptop");
        result.Value.QuantityRequested.Should().Be(2);
        result.Value.Notes.Should().Be("Urgent");

        _mockOrderRepository.Verify(r => r.AddAsync(It.IsAny<Order>(), default), Times.Once);
    }

    [TestMethod]
    public async Task CreateOrderCommandHandler_WithNonexistentInventoryItem_ReturnsFail()
    {
        // Arrange
        var userId = "user-123";
        var itemId = Guid.NewGuid();

        _mockInventoryRepository
            .Setup(r => r.GetByIdAsync(itemId, default))
            .ReturnsAsync((InventoryItem?)null);

        var command = new CreateOrderCommand(userId, itemId, 5, null);
        var handler = new CreateOrderCommandHandler(_mockOrderRepository.Object, _mockInventoryRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
        _mockOrderRepository.Verify(r => r.AddAsync(It.IsAny<Order>(), default), Times.Never);
    }

    [TestMethod]
    public async Task CreateOrderCommandHandler_WithInsufficientStock_ReturnsFail()
    {
        // Arrange
        var userId = "user-123";
        var itemId = Guid.NewGuid();
        var inventoryItem = new InventoryItem
        {
            Id = itemId,
            Name = "Laptop",
            QuantityInStock = 3  // Only 3 in stock
        };

        _mockInventoryRepository
            .Setup(r => r.GetByIdAsync(itemId, default))
            .ReturnsAsync(inventoryItem);

        var command = new CreateOrderCommand(userId, itemId, 5, null);  // Request 5
        var handler = new CreateOrderCommandHandler(_mockOrderRepository.Object, _mockInventoryRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Insufficient");
        _mockOrderRepository.Verify(r => r.AddAsync(It.IsAny<Order>(), default), Times.Never);
    }

    [TestMethod]
    public async Task CreateOrderCommandHandler_WithInvalidQuantity_ReturnsFail()
    {
        // Arrange
        var userId = "user-123";
        var itemId = Guid.NewGuid();
        var inventoryItem = new InventoryItem { Id = itemId, Name = "Laptop", QuantityInStock = 100 };

        _mockInventoryRepository
            .Setup(r => r.GetByIdAsync(itemId, default))
            .ReturnsAsync(inventoryItem);

        var command = new CreateOrderCommand(userId, itemId, 0, null);  // Invalid: 0
        var handler = new CreateOrderCommandHandler(_mockOrderRepository.Object, _mockInventoryRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Quantity");
        _mockOrderRepository.Verify(r => r.AddAsync(It.IsAny<Order>(), default), Times.Never);
    }

    #endregion

    #region CancelOrderCommand Tests

    [TestMethod]
    public async Task CancelOrderCommandHandler_WithValidPendingOrder_CancelsSuccessfully()
    {
        // Arrange
        var userId = "user-123";
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            UserId = userId,
            Status = "Pending",
            InventoryItemId = Guid.NewGuid(),
            QuantityRequested = 5,
            OrderedAt = DateTimeOffset.UtcNow
        };

        _mockOrderRepository
            .Setup(r => r.GetByIdAsync(orderId, default))
            .ReturnsAsync(order);

        var command = new CancelOrderCommand(orderId, userId);
        var handler = new CancelOrderCommandHandler(_mockOrderRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be("Cancelled");
        _mockOrderRepository.Verify(r => r.UpdateAsync(It.IsAny<Order>(), default), Times.Once);
    }

    [TestMethod]
    public async Task CancelOrderCommandHandler_WithNonexistentOrder_ReturnsFail()
    {
        // Arrange
        var userId = "user-123";
        var orderId = Guid.NewGuid();

        _mockOrderRepository
            .Setup(r => r.GetByIdAsync(orderId, default))
            .ReturnsAsync((Order?)null);

        var command = new CancelOrderCommand(orderId, userId);
        var handler = new CancelOrderCommandHandler(_mockOrderRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
        _mockOrderRepository.Verify(r => r.UpdateAsync(It.IsAny<Order>(), default), Times.Never);
    }

    [TestMethod]
    public async Task CancelOrderCommandHandler_WithUnauthorizedUser_ReturnsFail()
    {
        // Arrange
        var userId = "user-123";
        var otherUserId = "user-456";
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            UserId = userId,  // Different user
            Status = "Pending",
            InventoryItemId = Guid.NewGuid(),
            QuantityRequested = 5,
            OrderedAt = DateTimeOffset.UtcNow
        };

        _mockOrderRepository
            .Setup(r => r.GetByIdAsync(orderId, default))
            .ReturnsAsync(order);

        var command = new CancelOrderCommand(orderId, otherUserId);  // Different user
        var handler = new CancelOrderCommandHandler(_mockOrderRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Unauthorized");
        _mockOrderRepository.Verify(r => r.UpdateAsync(It.IsAny<Order>(), default), Times.Never);
    }

    [TestMethod]
    public async Task CancelOrderCommandHandler_WithNonPendingOrder_ReturnsFail()
    {
        // Arrange
        var userId = "user-123";
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            UserId = userId,
            Status = "Fulfilled",  // Already fulfilled
            InventoryItemId = Guid.NewGuid(),
            QuantityRequested = 5,
            OrderedAt = DateTimeOffset.UtcNow
        };

        _mockOrderRepository
            .Setup(r => r.GetByIdAsync(orderId, default))
            .ReturnsAsync(order);

        var command = new CancelOrderCommand(orderId, userId);
        var handler = new CancelOrderCommandHandler(_mockOrderRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Cannot cancel");
        _mockOrderRepository.Verify(r => r.UpdateAsync(It.IsAny<Order>(), default), Times.Never);
    }

    #endregion

    #region FulfillOrderCommand Tests

    [TestMethod]
    public async Task FulfillOrderCommandHandler_WithValidPendingOrder_FulfillsSuccessfully()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            UserId = "user-123",
            Status = "Pending",
            InventoryItemId = Guid.NewGuid(),
            QuantityRequested = 5,
            OrderedAt = DateTimeOffset.UtcNow,
            InventoryItem = new InventoryItem { Name = "Laptop" },
            User = new ApplicationUser { UserName = "Alice", Email = "alice@test.com" }
        };

        _mockOrderRepository
            .Setup(r => r.GetByIdAsync(orderId, default))
            .ReturnsAsync(order);

        var command = new FulfillOrderCommand(orderId);
        var handler = new FulfillOrderCommandHandler(_mockOrderRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be("Fulfilled");
        result.Value.FulfilledAt.Should().NotBeNull();
        result.Value.UserName.Should().Be("Alice");
        result.Value.ItemName.Should().Be("Laptop");
        _mockOrderRepository.Verify(r => r.UpdateAsync(It.IsAny<Order>(), default), Times.Once);
    }

    [TestMethod]
    public async Task FulfillOrderCommandHandler_WithNonexistentOrder_ReturnsFail()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _mockOrderRepository
            .Setup(r => r.GetByIdAsync(orderId, default))
            .ReturnsAsync((Order?)null);

        var command = new FulfillOrderCommand(orderId);
        var handler = new FulfillOrderCommandHandler(_mockOrderRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
        _mockOrderRepository.Verify(r => r.UpdateAsync(It.IsAny<Order>(), default), Times.Never);
    }

    [TestMethod]
    public async Task FulfillOrderCommandHandler_WithCancelledOrder_ReturnsFail()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            UserId = "user-123",
            Status = "Cancelled",
            InventoryItemId = Guid.NewGuid(),
            QuantityRequested = 5,
            OrderedAt = DateTimeOffset.UtcNow
        };

        _mockOrderRepository
            .Setup(r => r.GetByIdAsync(orderId, default))
            .ReturnsAsync(order);

        var command = new FulfillOrderCommand(orderId);
        var handler = new FulfillOrderCommandHandler(_mockOrderRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("cancelled");
        _mockOrderRepository.Verify(r => r.UpdateAsync(It.IsAny<Order>(), default), Times.Never);
    }

    [TestMethod]
    public async Task FulfillOrderCommandHandler_WithAlreadyFulfilledOrder_ReturnsFail()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            UserId = "user-123",
            Status = "Fulfilled",
            InventoryItemId = Guid.NewGuid(),
            QuantityRequested = 5,
            OrderedAt = DateTimeOffset.UtcNow,
            FulfilledAt = DateTimeOffset.UtcNow.AddMinutes(-10)
        };

        _mockOrderRepository
            .Setup(r => r.GetByIdAsync(orderId, default))
            .ReturnsAsync(order);

        var command = new FulfillOrderCommand(orderId);
        var handler = new FulfillOrderCommandHandler(_mockOrderRepository.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already been fulfilled");
        _mockOrderRepository.Verify(r => r.UpdateAsync(It.IsAny<Order>(), default), Times.Never);
    }

    #endregion
}
