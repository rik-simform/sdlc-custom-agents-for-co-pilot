#nullable enable

using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MyProject.Application.Features.Auth.Commands;
using MyProject.Domain.Entities;

namespace MyProject.UnitTests.Features.Auth;

/// <summary>
/// Unit tests for <see cref="AssignRoleCommandHandler"/> covering US-RBAC-006 acceptance criteria.
/// </summary>
[TestClass]
public class AssignRoleCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
    private readonly ILogger<AssignRoleCommandHandler> _logger;
    private readonly AssignRoleCommandHandler _sut;

    public AssignRoleCommandHandlerTests()
    {
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var roleStore = new Mock<IRoleStore<IdentityRole>>();
        _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
            roleStore.Object, null!, null!, null!, null!);

        _logger = NullLogger<AssignRoleCommandHandler>.Instance;
        _sut = new AssignRoleCommandHandler(_mockUserManager.Object, _mockRoleManager.Object, _logger);
    }

    /// <summary>
    /// AC-003: Valid role assignment returns success.
    /// </summary>
    [TestMethod]
    public async Task Handle_ValidAssignment_ReturnsSuccess()
    {
        // Arrange
        var targetUser = new ApplicationUser { Id = "user-1", Email = "user@test.com" };
        _mockRoleManager.Setup(r => r.RoleExistsAsync("Admin")).ReturnsAsync(true);
        _mockUserManager.Setup(u => u.FindByIdAsync("user-1")).ReturnsAsync(targetUser);
        _mockUserManager.Setup(u => u.IsInRoleAsync(targetUser, "Admin")).ReturnsAsync(false);
        _mockUserManager.Setup(u => u.AddToRoleAsync(targetUser, "Admin"))
            .ReturnsAsync(IdentityResult.Success);

        var command = new AssignRoleCommand("user-1", "Admin", "admin-actor");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain("Admin");
    }

    /// <summary>
    /// AC-004: Non-existent role returns failure.
    /// </summary>
    [TestMethod]
    public async Task Handle_NonExistentRole_ReturnsFailure()
    {
        // Arrange
        _mockRoleManager.Setup(r => r.RoleExistsAsync("SuperAdmin")).ReturnsAsync(false);

        var command = new AssignRoleCommand("user-1", "SuperAdmin", "admin-actor");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("SuperAdmin");
        result.Error.Should().Contain("does not exist");
    }

    /// <summary>
    /// AC-004: Non-existent user returns failure.
    /// </summary>
    [TestMethod]
    public async Task Handle_NonExistentUser_ReturnsFailure()
    {
        // Arrange
        _mockRoleManager.Setup(r => r.RoleExistsAsync("Admin")).ReturnsAsync(true);
        _mockUserManager.Setup(u => u.FindByIdAsync("nonexistent")).ReturnsAsync((ApplicationUser?)null);

        var command = new AssignRoleCommand("nonexistent", "Admin", "admin-actor");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }

    /// <summary>
    /// AC-005: Idempotent — re-assigning same role returns success without error.
    /// </summary>
    [TestMethod]
    public async Task Handle_UserAlreadyHasRole_ReturnsSuccessIdempotent()
    {
        // Arrange
        var targetUser = new ApplicationUser { Id = "user-1", Email = "user@test.com" };
        _mockRoleManager.Setup(r => r.RoleExistsAsync("Admin")).ReturnsAsync(true);
        _mockUserManager.Setup(u => u.FindByIdAsync("user-1")).ReturnsAsync(targetUser);
        _mockUserManager.Setup(u => u.IsInRoleAsync(targetUser, "Admin")).ReturnsAsync(true);

        var command = new AssignRoleCommand("user-1", "Admin", "admin-actor");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockUserManager.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    /// AC-003: Verify AddToRoleAsync is called for new role assignments.
    /// </summary>
    [TestMethod]
    public async Task Handle_ValidAssignment_CallsAddToRole()
    {
        // Arrange
        var targetUser = new ApplicationUser { Id = "user-2", Email = "user2@test.com" };
        _mockRoleManager.Setup(r => r.RoleExistsAsync("User")).ReturnsAsync(true);
        _mockUserManager.Setup(u => u.FindByIdAsync("user-2")).ReturnsAsync(targetUser);
        _mockUserManager.Setup(u => u.IsInRoleAsync(targetUser, "User")).ReturnsAsync(false);
        _mockUserManager.Setup(u => u.AddToRoleAsync(targetUser, "User"))
            .ReturnsAsync(IdentityResult.Success);

        var command = new AssignRoleCommand("user-2", "User", "admin-actor");

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _mockUserManager.Verify(u => u.AddToRoleAsync(targetUser, "User"), Times.Once);
    }
}
