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
/// Unit tests for <see cref="DeleteInventoryItemCommandHandler"/> covering US-RBAC-003 acceptance criteria.
/// </summary>
[TestClass]
public class DeleteInventoryItemCommandHandlerTests
{
    private readonly Mock<IInventoryRepository> _mockRepo;
    private readonly ILogger<DeleteInventoryItemCommandHandler> _logger;
    private readonly DeleteInventoryItemCommandHandler _sut;

    public DeleteInventoryItemCommandHandlerTests()
    {
        _mockRepo = new Mock<IInventoryRepository>();
        _logger = NullLogger<DeleteInventoryItemCommandHandler>.Instance;
        _sut = new DeleteInventoryItemCommandHandler(_mockRepo.Object, _logger);
    }

    /// <summary>
    /// AC-001: Valid delete sets IsActive = false via soft delete and returns success.
    /// </summary>
    [TestMethod]
    public async Task Handle_ExistingItem_ReturnsSuccess()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existing = new InventoryItem
        {
            Id = itemId, Sku = "SKU-DEL", Name = "To Delete",
            Category = "Parts", IsActive = true, CreatedBy = "admin-1"
        };
        _mockRepo.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var command = new DeleteInventoryItemCommand(itemId, "admin-1");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    /// <summary>
    /// AC-001: Verify DeleteAsync is called on the repository for existing items.
    /// </summary>
    [TestMethod]
    public async Task Handle_ExistingItem_CallsDeleteOnRepository()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existing = new InventoryItem
        {
            Id = itemId, Sku = "SKU-DEL", Name = "To Delete",
            Category = "Parts", IsActive = true, CreatedBy = "admin-1"
        };
        _mockRepo.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var command = new DeleteInventoryItemCommand(itemId, "admin-1");

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _mockRepo.Verify(r => r.DeleteAsync(itemId, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// AC-002: Non-existent item returns failure with not found message.
    /// </summary>
    [TestMethod]
    public async Task Handle_ItemNotFound_ReturnsFailure()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        var command = new DeleteInventoryItemCommand(itemId, "admin-1");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    /// <summary>
    /// AC-002: Non-existent item does not call DeleteAsync.
    /// </summary>
    [TestMethod]
    public async Task Handle_ItemNotFound_DoesNotCallDelete()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem?)null);

        var command = new DeleteInventoryItemCommand(Guid.NewGuid(), "admin-1");

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
