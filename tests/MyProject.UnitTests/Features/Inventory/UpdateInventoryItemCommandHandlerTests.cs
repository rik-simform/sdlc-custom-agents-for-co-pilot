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
/// Unit tests for <see cref="UpdateInventoryItemCommandHandler"/> covering US-RBAC-002 acceptance criteria.
/// </summary>
[TestClass]
public class UpdateInventoryItemCommandHandlerTests
{
    private readonly Mock<IInventoryRepository> _mockRepo;
    private readonly ILogger<UpdateInventoryItemCommandHandler> _logger;
    private readonly UpdateInventoryItemCommandHandler _sut;

    public UpdateInventoryItemCommandHandlerTests()
    {
        _mockRepo = new Mock<IInventoryRepository>();
        _logger = NullLogger<UpdateInventoryItemCommandHandler>.Instance;
        _sut = new UpdateInventoryItemCommandHandler(_mockRepo.Object, _logger);
    }

    /// <summary>
    /// AC-001: Valid update returns success with UpdatedAt and UpdatedBy set.
    /// </summary>
    [TestMethod]
    public async Task Handle_ValidCommand_ReturnsUpdatedItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existing = new InventoryItem
        {
            Id = itemId, Sku = "SKU-001", Name = "Old Name",
            Category = "Parts", QuantityInStock = 10, ReorderLevel = 5,
            UnitPrice = 9.99m, CreatedBy = "admin-1", IsActive = true
        };
        _mockRepo.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var command = new UpdateInventoryItemCommand(
            itemId, "New Name", "Updated desc", "Electronics",
            50, 10, 19.99m, "Shelf-B", true, "admin-2");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("New Name");
        result.Value.UpdatedBy.Should().Be("admin-2");
        result.Value.UpdatedAt.Should().NotBeNull();
        result.Value.UpdatedAt!.Value.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    /// <summary>
    /// AC-001: Verify update is persisted via the repository.
    /// </summary>
    [TestMethod]
    public async Task Handle_ValidCommand_PersistsUpdateViaRepository()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existing = new InventoryItem
        {
            Id = itemId, Sku = "SKU-001", Name = "Old", Category = "Parts",
            QuantityInStock = 10, ReorderLevel = 5, UnitPrice = 5m,
            CreatedBy = "admin-1", IsActive = true
        };
        _mockRepo.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var command = new UpdateInventoryItemCommand(
            itemId, "New", null, "Hardware", 20, 8, 12m, null, true, "admin-1");

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _mockRepo.Verify(r => r.UpdateAsync(
            It.Is<InventoryItem>(i => i.Id == itemId && i.Name == "New"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// AC-002: Non-existent item returns failure.
    /// </summary>
    [TestMethod]
    public async Task Handle_ItemNotFound_ReturnsFailure()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        var command = new UpdateInventoryItemCommand(
            itemId, "Name", null, "Parts", 10, 5, 5m, null, true, "admin-1");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    /// <summary>
    /// AC-002: Non-existent item does not call UpdateAsync.
    /// </summary>
    [TestMethod]
    public async Task Handle_ItemNotFound_DoesNotPersist()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        var command = new UpdateInventoryItemCommand(
            Guid.NewGuid(), "Name", null, "Parts", 10, 5, 5m, null, true, "admin-1");

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
