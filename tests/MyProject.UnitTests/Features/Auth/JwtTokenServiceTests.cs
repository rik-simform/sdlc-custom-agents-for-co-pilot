#nullable enable

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Options;
using MyProject.Domain.Entities;
using MyProject.Infrastructure.Services;

namespace MyProject.UnitTests.Features.Auth;

/// <summary>
/// Unit tests for JwtTokenService covering JWT generation (AC-004) and token hashing.
/// </summary>
[TestClass]
public class JwtTokenServiceTests
{
    private readonly JwtTokenService _sut;
    private readonly JwtSettings _settings;

    public JwtTokenServiceTests()
    {
        _settings = new JwtSettings
        {
            SigningKey = "ThisIsATestSigningKeyThatIsAtLeast32Characters!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            AccessTokenExpiryMinutes = 15
        };

        _sut = new JwtTokenService(Options.Create(_settings));
    }

    /// <summary>
    /// AC-004: JWT contains sub (user ID), email, roles, iat, exp claims.
    /// </summary>
    [TestMethod]
    public void GenerateAccessToken_ContainsRequiredClaims()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "user-123",
            Email = "test@example.com",
            UserName = "test@example.com"
        };
        var roles = new List<string> { "Admin", "User" };

        // Act
        var (token, _) = _sut.GenerateAccessToken(user, roles);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == "user-123");
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "test@example.com");
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Iat);
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Exp);
        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);

        var roleClaims = jwt.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        roleClaims.Should().Contain("Admin");
        roleClaims.Should().Contain("User");
    }

    /// <summary>
    /// AC-004: JWT is signed with HMAC-SHA256.
    /// </summary>
    [TestMethod]
    public void GenerateAccessToken_IsSignedWithHmacSha256()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-1", Email = "test@example.com" };

        // Act
        var (token, _) = _sut.GenerateAccessToken(user, new List<string>());

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        jwt.Header.Alg.Should().Be("HS256");
    }

    /// <summary>
    /// AC-001: Access token expires in configured time (15 minutes).
    /// </summary>
    [TestMethod]
    public void GenerateAccessToken_ExpiresAtConfiguredTime()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-1", Email = "test@example.com" };
        var beforeGeneration = DateTimeOffset.UtcNow;

        // Act
        var (_, expiresAt) = _sut.GenerateAccessToken(user, new List<string>());

        // Assert
        expiresAt.Should().BeCloseTo(
            beforeGeneration.AddMinutes(_settings.AccessTokenExpiryMinutes),
            TimeSpan.FromSeconds(5));
    }

    /// <summary>
    /// AC-004: JWT has correct issuer and audience.
    /// </summary>
    [TestMethod]
    public void GenerateAccessToken_HasCorrectIssuerAndAudience()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-1", Email = "test@example.com" };

        // Act
        var (token, _) = _sut.GenerateAccessToken(user, new List<string>());

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        jwt.Issuer.Should().Be("TestIssuer");
        jwt.Audiences.Should().Contain("TestAudience");
    }

    /// <summary>
    /// Refresh token is cryptographically random and sufficiently long.
    /// </summary>
    [TestMethod]
    public void GenerateRefreshToken_IsUniqueAndSufficientlyLong()
    {
        // Act
        var token1 = _sut.GenerateRefreshToken();
        var token2 = _sut.GenerateRefreshToken();

        // Assert
        token1.Should().NotBeNullOrEmpty();
        token2.Should().NotBeNullOrEmpty();
        token1.Should().NotBe(token2); // Cryptographically unique
        Convert.FromBase64String(token1).Length.Should().Be(64); // 64 bytes of randomness
    }

    /// <summary>
    /// NFR: Token hash is SHA-256 and deterministic.
    /// </summary>
    [TestMethod]
    public void HashToken_ProducesDeterministicSha256Hash()
    {
        // Act
        var hash1 = _sut.HashToken("test-token");
        var hash2 = _sut.HashToken("test-token");
        var hash3 = _sut.HashToken("different-token");

        // Assert
        hash1.Should().Be(hash2); // Deterministic
        hash1.Should().NotBe(hash3); // Different input → different hash
        hash1.Should().HaveLength(64); // SHA-256 hex = 64 chars
    }
}
