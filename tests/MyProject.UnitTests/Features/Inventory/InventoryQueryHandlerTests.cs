#nullable enable

using FluentAssertions;
using Moq;
using MyProject.Application.Features.Inventory.Queries;
using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;

namespace MyProject.UnitTests.Features.Inventory;

/// <summary>
/// Unit tests for inventory query handlers covering US-RBAC-004 and US-RBAC-005 acceptance criteria.
/// </summary>
[TestClass]
public class InventoryQueryHandlerTests
{
    private readonly Mock<IInventoryRepository> _mockRepo;

    public InventoryQueryHandlerTests()
    {
        _mockRepo = new Mock<IInventoryRepository>();
    }

    // ─── US-RBAC-004: GetAll ────────────────────────────────────────

    /// <summary>
    /// AC-001: Returns all active inventory items.
    /// </summary>
    [TestMethod]
    public async Task GetAll_ReturnsActiveItems()
    {
        // Arrange
        var items = new List<InventoryItem>
        {
            new() { Id = Guid.NewGuid(), Sku = "SKU-1", Name = "Item 1", Category = "Parts", IsActive = true, CreatedBy = "admin-1" },
            new() { Id = Guid.NewGuid(), Sku = "SKU-2", Name = "Item 2", Category = "Electronics", IsActive = true, CreatedBy = "admin-1" }
        };
        _mockRepo.Setup(r => r.GetAllAsync(null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        var sut = new GetInventoryItemsQueryHandler(_mockRepo.Object);
        var query = new GetInventoryItemsQuery();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    /// <summary>
    /// AC-003: Category filter is passed to repository.
    /// </summary>
    [TestMethod]
    public async Task GetAll_WithCategoryFilter_PassesCategoryToRepository()
    {
        // Arrange
        var items = new List<InventoryItem>
        {
            new() { Id = Guid.NewGuid(), Sku = "SKU-1", Name = "Part A", Category = "Parts", IsActive = true, CreatedBy = "admin-1" }
        };
        _mockRepo.Setup(r => r.GetAllAsync("Parts", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        var sut = new GetInventoryItemsQueryHandler(_mockRepo.Object);
        var query = new GetInventoryItemsQuery(Category: "Parts");

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Category.Should().Be("Parts");
    }

    /// <summary>
    /// AC-004: Search filter is passed to repository.
    /// </summary>
    [TestMethod]
    public async Task GetAll_WithSearchFilter_PassesSearchToRepository()
    {
        // Arrange
        var items = new List<InventoryItem>
        {
            new() { Id = Guid.NewGuid(), Sku = "SKU-W", Name = "Widget", Category = "Parts", IsActive = true, CreatedBy = "admin-1" }
        };
        _mockRepo.Setup(r => r.GetAllAsync(null, "Widget", It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        var sut = new GetInventoryItemsQueryHandler(_mockRepo.Object);
        var query = new GetInventoryItemsQuery(SearchTerm: "Widget");

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Widget");
    }

    /// <summary>
    /// AC-006: Reorder query returns items where QuantityInStock ≤ ReorderLevel.
    /// </summary>
    [TestMethod]
    public async Task GetReorder_ReturnsItemsNeedingReorder()
    {
        // Arrange
        var items = new List<InventoryItem>
        {
            new() { Id = Guid.NewGuid(), Sku = "SKU-LOW", Name = "Low Stock",
                     Category = "Parts", QuantityInStock = 3, ReorderLevel = 10,
                     IsActive = true, CreatedBy = "admin-1" }
        };
        _mockRepo.Setup(r => r.GetItemsNeedingReorderAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        var sut = new GetItemsNeedingReorderQueryHandler(_mockRepo.Object);
        var query = new GetItemsNeedingReorderQuery();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().NeedsReorder.Should().BeTrue();
    }

    // ─── US-RBAC-005: GetById ────────────────────────────────────────

    /// <summary>
    /// AC-001: Valid ID returns full item details.
    /// </summary>
    [TestMethod]
    public async Task GetById_ExistingItem_ReturnsItemDetails()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = new InventoryItem
        {
            Id = itemId, Sku = "SKU-001", Name = "Widget",
            Description = "A widget", Category = "Parts",
            QuantityInStock = 100, ReorderLevel = 10, UnitPrice = 9.99m,
            Location = "Warehouse A", IsActive = true, CreatedBy = "admin-1",
            CreatedAt = DateTimeOffset.UtcNow
        };
        _mockRepo.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var sut = new GetInventoryItemByIdQueryHandler(_mockRepo.Object);
        var query = new GetInventoryItemByIdQuery(itemId);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(itemId);
        result.Sku.Should().Be("SKU-001");
        result.Name.Should().Be("Widget");
        result.Description.Should().Be("A widget");
        result.Category.Should().Be("Parts");
        result.QuantityInStock.Should().Be(100);
        result.ReorderLevel.Should().Be(10);
        result.UnitPrice.Should().Be(9.99m);
        result.Location.Should().Be("Warehouse A");
    }

    /// <summary>
    /// AC-003: Non-existent ID returns null.
    /// </summary>
    [TestMethod]
    public async Task GetById_NonExistentItem_ReturnsNull()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        var sut = new GetInventoryItemByIdQueryHandler(_mockRepo.Object);
        var query = new GetInventoryItemByIdQuery(itemId);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// AC-001: Returned response includes NeedsReorder flag.
    /// </summary>
    [TestMethod]
    public async Task GetById_LowStockItem_NeedsReorderIsTrue()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = new InventoryItem
        {
            Id = itemId, Sku = "SKU-LOW", Name = "Low Stock Item",
            Category = "Parts", QuantityInStock = 2, ReorderLevel = 10,
            IsActive = true, CreatedBy = "admin-1"
        };
        _mockRepo.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var sut = new GetInventoryItemByIdQueryHandler(_mockRepo.Object);
        var query = new GetInventoryItemByIdQuery(itemId);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.NeedsReorder.Should().BeTrue();
    }
}
