#nullable enable

using MyProject.Domain.Entities;

namespace MyProject.Domain.Interfaces;

/// <summary>
/// Service for generating and validating JWT access tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT access token for the specified user and roles.
    /// </summary>
    /// <param name="user">The authenticated user.</param>
    /// <param name="roles">The user's assigned roles.</param>
    /// <returns>The JWT token string and its expiration time.</returns>
    (string Token, DateTimeOffset ExpiresAt) GenerateAccessToken(ApplicationUser user, IList<string> roles);

    /// <summary>
    /// Generates a cryptographically secure random refresh token.
    /// </summary>
    /// <returns>The plaintext refresh token value.</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Computes a SHA-256 hash of the given token value for secure storage.
    /// </summary>
    /// <param name="token">The plaintext token.</param>
    /// <returns>The SHA-256 hash as a hex string.</returns>
    string HashToken(string token);
}
