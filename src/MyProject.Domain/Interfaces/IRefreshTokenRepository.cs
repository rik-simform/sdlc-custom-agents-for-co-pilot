#nullable enable

using MyProject.Domain.Entities;

namespace MyProject.Domain.Interfaces;

/// <summary>
/// Repository for managing refresh tokens in the data store.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Retrieves an active refresh token by its hash.
    /// </summary>
    /// <param name="tokenHash">The SHA-256 hash of the token.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The refresh token if found and active; otherwise null.</returns>
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);

    /// <summary>
    /// Adds a new refresh token to the data store.
    /// </summary>
    /// <param name="refreshToken">The refresh token entity.</param>
    /// <param name="ct">Cancellation token.</param>
    Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default);

    /// <summary>
    /// Revokes a refresh token by marking it as revoked.
    /// </summary>
    /// <param name="refreshToken">The refresh token to revoke.</param>
    /// <param name="ct">Cancellation token.</param>
    Task RevokeAsync(RefreshToken refreshToken, CancellationToken ct = default);

    /// <summary>
    /// Revokes all refresh tokens for a specific user.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    Task RevokeAllForUserAsync(string userId, CancellationToken ct = default);
}
