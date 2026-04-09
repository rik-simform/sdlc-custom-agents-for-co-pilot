#nullable enable

using FluentAssertions;
using Moq;
using MyProject.Application.Features.Orders.DTOs;
using MyProject.Application.Features.Orders.Queries;
using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;

namespace MyProject.UnitTests.Features.Orders;

/// <summary>Unit tests for Orders query handlers.</summary>
[TestClass]
public class OrderQueryHandlerTests
{
    private Mock<IOrderRepository> _mockOrderRepository = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
    }

    #region GetUserOrdersQuery Tests

    [TestMethod]
    public async Task GetUserOrdersQueryHandler_WithValidUser_ReturnsPaginatedOrders()
    {
        // Arrange
        var userId = "user-123";
        var orders = new List<Order>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, QuantityRequested = 5, Status = "Pending", OrderedAt = DateTimeOffset.UtcNow, InventoryItem = new() { Name = "Laptop" } },
            new() { Id = Guid.NewGuid(), UserId = userId, QuantityRequested = 2, Status = "Fulfilled", OrderedAt = DateTimeOffset.UtcNow, InventoryItem = new() { Name = "Mouse" } }
        };

        _mockOrderRepository
            .Setup(r => r.GetByUserIdAsync(userId, 0, 10, default))
            .ReturnsAsync((orders, totalCount: 2));

        var query = new GetUserOrdersQuery(userId, 1, 10);
        var handler = new GetUserOrdersQueryHandler(_mockOrderRepository.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.TotalCount.Should().Be(2);
        result.Value.TotalPages.Should().Be(1);
        result.Value.CurrentPage.Should().Be(1);
    }

    [TestMethod]
    public async Task GetUserOrdersQueryHandler_WithNoOrders_ReturnsEmptyList()
    {
        // Arrange
        var userId = "user-123";

        _mockOrderRepository
            .Setup(r => r.GetByUserIdAsync(userId, 0, 10, default))
            .ReturnsAsync((Enumerable.Empty<Order>(), totalCount: 0));

        var query = new GetUserOrdersQuery(userId, 1, 10);
        var handler = new GetUserOrdersQueryHandler(_mockOrderRepository.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }

    [TestMethod]
    public async Task GetUserOrdersQueryHandler_WithPagination_CalculatesCorrectTotalPages()
    {
        // Arrange
        var userId = "user-123";
        var orders = Enumerable.Range(1, 10).Select(i => new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            QuantityRequested = 1,
            Status = "Pending",
            OrderedAt = DateTimeOffset.UtcNow,
            InventoryItem = new() { Name = $"Item {i}" }
        }).ToList();

        _mockOrderRepository
            .Setup(r => r.GetByUserIdAsync(userId, 10, 10, default))  // Page 2
            .ReturnsAsync((orders.Skip(10).Take(10), totalCount: 25));

        var query = new GetUserOrdersQuery(userId, 2, 10);
        var handler = new GetUserOrdersQueryHandler(_mockOrderRepository.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value!.TotalPages.Should().Be(3);  // 25 items / 10 per page = 3 pages
        result.Value.CurrentPage.Should().Be(2);
    }

    #endregion

    #region GetAllOrdersQuery Tests

    [TestMethod]
    public async Task GetAllOrdersQueryHandler_WithNoFilters_ReturnsAllOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new() { Id = Guid.NewGuid(), UserId = "user-1", QuantityRequested = 5, Status = "Pending", OrderedAt = DateTimeOffset.UtcNow, InventoryItem = new() { Name = "Item1" }, User = new() { UserName = "Alice", Email = "alice@test.com" } },
            new() { Id = Guid.NewGuid(), UserId = "user-2", QuantityRequested = 2, Status = "Fulfilled", OrderedAt = DateTimeOffset.UtcNow, InventoryItem = new() { Name = "Item2" }, User = new() { UserName = "Bob", Email = "bob@test.com" } }
        };

        _mockOrderRepository
            .Setup(r => r.GetAllAsync(0, 10, null, null, default))
            .ReturnsAsync((orders, totalCount: 2));

        var query = new GetAllOrdersQuery(1, 10, null, null);
        var handler = new GetAllOrdersQueryHandler(_mockOrderRepository.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.TotalCount.Should().Be(2);
    }

    [TestMethod]
    public async Task GetAllOrdersQueryHandler_WithStatusFilter_ReturnsFilteredOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new() { Id = Guid.NewGuid(), UserId = "user-1", QuantityRequested = 5, Status = "Pending", OrderedAt = DateTimeOffset.UtcNow, InventoryItem = new() { Name = "Item1" }, User = new() { UserName = "Alice" } }
        };

        _mockOrderRepository
            .Setup(r => r.GetAllAsync(0, 10, "Pending", null, default))
            .ReturnsAsync((orders, totalCount: 1));

        var query = new GetAllOrdersQuery(1, 10, "Pending", null);
        var handler = new GetAllOrdersQueryHandler(_mockOrderRepository.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value!.Items.Should().HaveCount(1);
        result.Value.Items.First().Status.Should().Be("Pending");
    }

    [TestMethod]
    public async Task GetAllOrdersQueryHandler_WithUserFilter_ReturnsFilteredOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new() { Id = Guid.NewGuid(), UserId = "user-1", QuantityRequested = 5, Status = "Pending", OrderedAt = DateTimeOffset.UtcNow, InventoryItem = new() { Name = "Item1" }, User = new() { UserName = "Alice", Email = "alice@test.com" } }
        };

        _mockOrderRepository
            .Setup(r => r.GetAllAsync(0, 10, null, "alice", default))
            .ReturnsAsync((orders, totalCount: 1));

        var query = new GetAllOrdersQuery(1, 10, null, "alice");
        var handler = new GetAllOrdersQueryHandler(_mockOrderRepository.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value!.Items.Should().HaveCount(1);
        result.Value.Items.First().UserName.Should().Be("Alice");
    }

    #endregion

    #region GetOrderByIdQuery Tests

    [TestMethod]
    public async Task GetOrderByIdQueryHandler_WithExistingOrder_ReturnsOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            UserId = "user-123",
            QuantityRequested = 5,
            Status = "Pending",
            OrderedAt = DateTimeOffset.UtcNow,
            InventoryItem = new() { Name = "Laptop" }
        };

        _mockOrderRepository
            .Setup(r => r.GetByIdAsync(orderId, default))
            .ReturnsAsync(order);

        var query = new GetOrderByIdQuery(orderId);
        var handler = new GetOrderByIdQueryHandler(_mockOrderRepository.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().Be(orderId);
        result.Value.ItemName.Should().Be("Laptop");
        result.Value.Status.Should().Be("Pending");
    }

    [TestMethod]
    public async Task GetOrderByIdQueryHandler_WithNonexistentOrder_ReturnsNull()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _mockOrderRepository
            .Setup(r => r.GetByIdAsync(orderId, default))
            .ReturnsAsync((Order?)null);

        var query = new GetOrderByIdQuery(orderId);
        var handler = new GetOrderByIdQueryHandler(_mockOrderRepository.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [TestMethod]
    public async Task GetOrderByIdQueryHandler_WhenInventoryItemIsNull_ReturnsUnknown()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            UserId = "user-123",
            QuantityRequested = 5,
            Status = "Pending",
            OrderedAt = DateTimeOffset.UtcNow,
            InventoryItem = null  // Null inventory item
        };

        _mockOrderRepository
            .Setup(r => r.GetByIdAsync(orderId, default))
            .ReturnsAsync(order);

        var query = new GetOrderByIdQuery(orderId);
        var handler = new GetOrderByIdQueryHandler(_mockOrderRepository.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value!.ItemName.Should().Be("Unknown");
    }

    #endregion
}
