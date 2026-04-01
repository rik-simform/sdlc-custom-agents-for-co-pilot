#nullable enable

using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MyProject.Application.Features.Auth.Commands;
using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;

namespace MyProject.UnitTests.Features.Auth;

/// <summary>
/// Unit tests for LoginCommandHandler covering all authentication acceptance criteria.
/// </summary>
[TestClass]
public class LoginCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepo;
    private readonly ILogger<LoginCommandHandler> _logger;
    private readonly LoginCommandHandler _sut;

    public LoginCommandHandlerTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _mockTokenService = new Mock<ITokenService>();
        _mockRefreshTokenRepo = new Mock<IRefreshTokenRepository>();
        _logger = NullLogger<LoginCommandHandler>.Instance;

        _sut = new LoginCommandHandler(
            _mockUserManager.Object,
            _mockTokenService.Object,
            _mockRefreshTokenRepo.Object,
            _logger);
    }

    /// <summary>
    /// AC-001: Valid credentials return JWT access token and refresh token.
    /// </summary>
    [TestMethod]
    public async Task Handle_WithValidCredentials_ReturnsTokens()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-1", Email = "test@example.com", UserName = "test@example.com" };
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(15);

        _mockUserManager.Setup(x => x.FindByEmailAsync("test@example.com"))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, "ValidPass123!"))
            .ReturnsAsync(true);
        _mockUserManager.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });
        _mockTokenService.Setup(x => x.GenerateAccessToken(user, It.IsAny<IList<string>>()))
            .Returns(("jwt-token-value", expiresAt));
        _mockTokenService.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token-value");
        _mockTokenService.Setup(x => x.HashToken("refresh-token-value"))
            .Returns("hashed-refresh-token");

        var command = new LoginCommand("test@example.com", "ValidPass123!");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AccessToken.Should().Be("jwt-token-value");
        result.Value.RefreshToken.Should().Be("refresh-token-value");
        result.Value.ExpiresAt.Should().Be(expiresAt);

        _mockRefreshTokenRepo.Verify(x => x.AddAsync(
            It.Is<RefreshToken>(rt => rt.UserId == "user-1" && rt.TokenHash == "hashed-refresh-token"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// AC-002: Incorrect password returns generic "Invalid email or password" error.
    /// </summary>
    [TestMethod]
    public async Task Handle_WithInvalidPassword_ReturnsGenericError()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-1", Email = "test@example.com" };

        _mockUserManager.Setup(x => x.FindByEmailAsync("test@example.com"))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, "WrongPassword"))
            .ReturnsAsync(false);

        var command = new LoginCommand("test@example.com", "WrongPassword");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invalid email or password");
    }

    /// <summary>
    /// AC-003: Non-existent email returns same generic error as wrong password (no credential enumeration).
    /// </summary>
    [TestMethod]
    public async Task Handle_WithNonExistentEmail_ReturnsGenericError()
    {
        // Arrange
        _mockUserManager.Setup(x => x.FindByEmailAsync("nonexistent@example.com"))
            .ReturnsAsync((ApplicationUser?)null);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var command = new LoginCommand("nonexistent@example.com", "AnyPassword123!");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invalid email or password");

        // Verify dummy password check was performed (timing attack prevention)
        _mockUserManager.Verify(x => x.CheckPasswordAsync(
            It.IsAny<ApplicationUser>(), "AnyPassword123!"), Times.Once);
    }

    /// <summary>
    /// AC-001: Refresh token is stored in repository on successful login.
    /// </summary>
    [TestMethod]
    public async Task Handle_WithValidCredentials_StoresRefreshToken()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-1", Email = "test@example.com" };

        _mockUserManager.Setup(x => x.FindByEmailAsync("test@example.com"))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, "ValidPass123!"))
            .ReturnsAsync(true);
        _mockUserManager.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());
        _mockTokenService.Setup(x => x.GenerateAccessToken(user, It.IsAny<IList<string>>()))
            .Returns(("token", DateTimeOffset.UtcNow.AddMinutes(15)));
        _mockTokenService.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh");
        _mockTokenService.Setup(x => x.HashToken("refresh"))
            .Returns("hashed");

        // Act
        await _sut.Handle(new LoginCommand("test@example.com", "ValidPass123!"), CancellationToken.None);

        // Assert
        _mockRefreshTokenRepo.Verify(x => x.AddAsync(
            It.Is<RefreshToken>(rt =>
                rt.UserId == "user-1" &&
                rt.TokenHash == "hashed" &&
                rt.ExpiresAt > DateTimeOffset.UtcNow.AddDays(6)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verify that device info is stored with the refresh token.
    /// </summary>
    [TestMethod]
    public async Task Handle_WithDeviceInfo_StoresDeviceInfoOnRefreshToken()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-1", Email = "test@example.com" };

        _mockUserManager.Setup(x => x.FindByEmailAsync("test@example.com"))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.CheckPasswordAsync(user, "ValidPass123!"))
            .ReturnsAsync(true);
        _mockUserManager.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());
        _mockTokenService.Setup(x => x.GenerateAccessToken(user, It.IsAny<IList<string>>()))
            .Returns(("token", DateTimeOffset.UtcNow.AddMinutes(15)));
        _mockTokenService.Setup(x => x.GenerateRefreshToken()).Returns("refresh");
        _mockTokenService.Setup(x => x.HashToken("refresh")).Returns("hashed");

        // Act
        await _sut.Handle(new LoginCommand("test@example.com", "ValidPass123!", "Mozilla/5.0"), CancellationToken.None);

        // Assert
        _mockRefreshTokenRepo.Verify(x => x.AddAsync(
            It.Is<RefreshToken>(rt => rt.DeviceInfo == "Mozilla/5.0"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
