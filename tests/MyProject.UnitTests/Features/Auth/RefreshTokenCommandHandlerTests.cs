#nullable enable

using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MyProject.Application.Features.Auth.Commands;
using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;

namespace MyProject.UnitTests.Features.Auth;

/// <summary>
/// Unit tests for RefreshTokenCommandHandler covering token rotation (AC-005).
/// </summary>
[TestClass]
public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepo;
    private readonly RefreshTokenCommandHandler _sut;

    public RefreshTokenCommandHandlerTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _mockTokenService = new Mock<ITokenService>();
        _mockRefreshTokenRepo = new Mock<IRefreshTokenRepository>();

        _sut = new RefreshTokenCommandHandler(
            _mockUserManager.Object,
            _mockTokenService.Object,
            _mockRefreshTokenRepo.Object,
            NullLogger<RefreshTokenCommandHandler>.Instance);
    }

    /// <summary>
    /// AC-005: Valid refresh token issues new JWT and rotates the refresh token.
    /// </summary>
    [TestMethod]
    public async Task Handle_WithValidRefreshToken_ReturnsNewTokensAndRotates()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-1", Email = "test@example.com" };
        var existingToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = "user-1",
            TokenHash = "old-hash",
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(5),
            IsRevoked = false
        };
        var newExpiresAt = DateTimeOffset.UtcNow.AddMinutes(15);

        _mockTokenService.Setup(x => x.HashToken("old-refresh-token")).Returns("old-hash");
        _mockRefreshTokenRepo.Setup(x => x.GetByTokenHashAsync("old-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingToken);
        _mockUserManager.Setup(x => x.FindByIdAsync("user-1")).ReturnsAsync(user);
        _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
        _mockTokenService.Setup(x => x.GenerateAccessToken(user, It.IsAny<IList<string>>()))
            .Returns(("new-jwt", newExpiresAt));
        _mockTokenService.Setup(x => x.GenerateRefreshToken()).Returns("new-refresh-token");
        _mockTokenService.Setup(x => x.HashToken("new-refresh-token")).Returns("new-hash");

        // Act
        var result = await _sut.Handle(new RefreshTokenCommand("old-refresh-token"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.AccessToken.Should().Be("new-jwt");
        result.Value.RefreshToken.Should().Be("new-refresh-token");

        // Verify old token was revoked (rotation)
        _mockRefreshTokenRepo.Verify(x => x.RevokeAsync(existingToken, It.IsAny<CancellationToken>()), Times.Once);
        // Verify new token was stored
        _mockRefreshTokenRepo.Verify(x => x.AddAsync(
            It.Is<RefreshToken>(rt => rt.TokenHash == "new-hash"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Invalid refresh token returns error.
    /// </summary>
    [TestMethod]
    public async Task Handle_WithInvalidRefreshToken_ReturnsError()
    {
        // Arrange
        _mockTokenService.Setup(x => x.HashToken("invalid-token")).Returns("invalid-hash");
        _mockRefreshTokenRepo.Setup(x => x.GetByTokenHashAsync("invalid-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null);

        // Act
        var result = await _sut.Handle(new RefreshTokenCommand("invalid-token"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invalid or expired refresh token");
    }

    /// <summary>
    /// Expired refresh token returns error.
    /// </summary>
    [TestMethod]
    public async Task Handle_WithExpiredRefreshToken_ReturnsError()
    {
        // Arrange
        var expiredToken = new RefreshToken
        {
            TokenHash = "expired-hash",
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(-1),
            IsRevoked = false
        };

        _mockTokenService.Setup(x => x.HashToken("expired-token")).Returns("expired-hash");
        _mockRefreshTokenRepo.Setup(x => x.GetByTokenHashAsync("expired-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expiredToken);

        // Act
        var result = await _sut.Handle(new RefreshTokenCommand("expired-token"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Invalid or expired");
    }

    /// <summary>
    /// Revoked refresh token returns error.
    /// </summary>
    [TestMethod]
    public async Task Handle_WithRevokedRefreshToken_ReturnsError()
    {
        // Arrange
        var revokedToken = new RefreshToken
        {
            TokenHash = "revoked-hash",
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(5),
            IsRevoked = true
        };

        _mockTokenService.Setup(x => x.HashToken("revoked-token")).Returns("revoked-hash");
        _mockRefreshTokenRepo.Setup(x => x.GetByTokenHashAsync("revoked-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshToken?)null); // Repo filters out revoked tokens

        // Act
        var result = await _sut.Handle(new RefreshTokenCommand("revoked-token"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    /// <summary>
    /// Refresh token for non-existent user returns error.
    /// </summary>
    [TestMethod]
    public async Task Handle_WithTokenForDeletedUser_ReturnsError()
    {
        // Arrange
        var token = new RefreshToken
        {
            UserId = "deleted-user",
            TokenHash = "valid-hash",
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(5),
            IsRevoked = false
        };

        _mockTokenService.Setup(x => x.HashToken("some-token")).Returns("valid-hash");
        _mockRefreshTokenRepo.Setup(x => x.GetByTokenHashAsync("valid-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
        _mockUserManager.Setup(x => x.FindByIdAsync("deleted-user"))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _sut.Handle(new RefreshTokenCommand("some-token"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invalid or expired refresh token");
    }
}
