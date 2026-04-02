#nullable enable

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MyProject.Application.Features.Inventory.Commands;
using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;

namespace MyProject.UnitTests.Features.Inventory;

/// <summary>
/// Unit tests for <see cref="CreateInventoryItemCommandHandler"/> covering US-RBAC-001 acceptance criteria.
/// </summary>
[TestClass]
public class CreateInventoryItemCommandHandlerTests
{
    private readonly Mock<IInventoryRepository> _mockRepo;
    private readonly ILogger<CreateInventoryItemCommandHandler> _logger;
    private readonly CreateInventoryItemCommandHandler _sut;

    public CreateInventoryItemCommandHandlerTests()
    {
        _mockRepo = new Mock<IInventoryRepository>();
        _logger = NullLogger<CreateInventoryItemCommandHandler>.Instance;
        _sut = new CreateInventoryItemCommandHandler(_mockRepo.Object, _logger);
    }

    /// <summary>
    /// AC-001: Given a valid request, creates the item with server-generated Id and CreatedAt.
    /// </summary>
    [TestMethod]
    public async Task Handle_ValidCommand_ReturnsCreatedItem()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetBySkuAsync("SKU-001", It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        var command = new CreateInventoryItemCommand(
            Sku: "SKU-001", Name: "Widget", Description: "A widget",
            Category: "Parts", QuantityInStock: 100, ReorderLevel: 10,
            UnitPrice: 9.99m, Location: "Warehouse A", UserId: "admin-user-id");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Id.Should().NotBe(Guid.Empty);
        result.Value.Sku.Should().Be("SKU-001");
        result.Value.Name.Should().Be("Widget");
        result.Value.Description.Should().Be("A widget");
        result.Value.Category.Should().Be("Parts");
        result.Value.QuantityInStock.Should().Be(100);
        result.Value.ReorderLevel.Should().Be(10);
        result.Value.UnitPrice.Should().Be(9.99m);
        result.Value.Location.Should().Be("Warehouse A");
        result.Value.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    /// <summary>
    /// AC-005: CreatedBy is set to the user ID from the command (extracted from JWT sub claim).
    /// </summary>
    [TestMethod]
    public async Task Handle_ValidCommand_SetsCreatedByToUserId()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetBySkuAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        var command = new CreateInventoryItemCommand(
            Sku: "SKU-002", Name: "Gadget", Description: null,
            Category: "Electronics", QuantityInStock: 50, ReorderLevel: 5,
            UnitPrice: 24.99m, Location: null, UserId: "admin-guid-123");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.CreatedBy.Should().Be("admin-guid-123");
    }

    /// <summary>
    /// AC-001: Verify the item is persisted via the repository.
    /// </summary>
    [TestMethod]
    public async Task Handle_ValidCommand_PersistsItemViaRepository()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetBySkuAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        var command = new CreateInventoryItemCommand(
            Sku: "SKU-003", Name: "Bolt", Description: "Standard bolt",
            Category: "Hardware", QuantityInStock: 1000, ReorderLevel: 200,
            UnitPrice: 0.50m, Location: "Bin-42", UserId: "admin-1");

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _mockRepo.Verify(r => r.AddAsync(
            It.Is<InventoryItem>(i =>
                i.Sku == "SKU-003" &&
                i.Name == "Bolt" &&
                i.CreatedBy == "admin-1"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Duplicate SKU returns failure result.
    /// </summary>
    [TestMethod]
    public async Task Handle_DuplicateSku_ReturnsFailure()
    {
        // Arrange
        var existing = new InventoryItem { Sku = "SKU-DUP", Name = "Existing" };
        _mockRepo.Setup(r => r.GetBySkuAsync("SKU-DUP", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var command = new CreateInventoryItemCommand(
            Sku: "SKU-DUP", Name: "Duplicate", Description: null,
            Category: "Parts", QuantityInStock: 10, ReorderLevel: 2,
            UnitPrice: 5.00m, Location: null, UserId: "admin-1");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("SKU-DUP");
    }

    /// <summary>
    /// Duplicate SKU does not persist anything to the repository.
    /// </summary>
    [TestMethod]
    public async Task Handle_DuplicateSku_DoesNotPersist()
    {
        // Arrange
        var existing = new InventoryItem { Sku = "SKU-DUP", Name = "Existing" };
        _mockRepo.Setup(r => r.GetBySkuAsync("SKU-DUP", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var command = new CreateInventoryItemCommand(
            Sku: "SKU-DUP", Name: "Duplicate", Description: null,
            Category: "Parts", QuantityInStock: 10, ReorderLevel: 2,
            UnitPrice: 5.00m, Location: null, UserId: "admin-1");

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
