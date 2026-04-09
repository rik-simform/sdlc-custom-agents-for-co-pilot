#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;
using MyProject.Infrastructure.Data;

namespace MyProject.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation for managing refresh tokens.
/// </summary>
public class RefreshTokenRepository(AppDbContext context, ILogger<RefreshTokenRepository> logger) : IRefreshTokenRepository
{
    /// <inheritdoc />
    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default)
    {
        try
        {
            return await context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash && !rt.IsRevoked, ct);
        }
        catch (OperationCanceledException) { throw; }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to retrieve refresh token by hash");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        try
        {
            await context.RefreshTokens.AddAsync(refreshToken, ct);
            await context.SaveChangesAsync(ct);
        }
        catch (OperationCanceledException) { throw; }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to add refresh token for user {UserId}", refreshToken.UserId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task RevokeAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        try
        {
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTimeOffset.UtcNow;
            context.RefreshTokens.Update(refreshToken);
            await context.SaveChangesAsync(ct);
        }
        catch (OperationCanceledException) { throw; }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to revoke refresh token for user {UserId}", refreshToken.UserId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task RevokeAllForUserAsync(string userId, CancellationToken ct = default)
    {
        try
        {
            var tokens = await context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync(ct);

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTimeOffset.UtcNow;
            }

            await context.SaveChangesAsync(ct);
        }
        catch (OperationCanceledException) { throw; }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to revoke all refresh tokens for user {UserId}", userId);
            throw;
        }
    }
}
