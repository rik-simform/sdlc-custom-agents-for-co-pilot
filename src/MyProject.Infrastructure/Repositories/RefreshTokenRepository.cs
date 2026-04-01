#nullable enable

using Microsoft.EntityFrameworkCore;
using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;
using MyProject.Infrastructure.Data;

namespace MyProject.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation for managing refresh tokens.
/// </summary>
public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    /// <inheritdoc />
    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default)
    {
        return await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash && !rt.IsRevoked, ct);
    }

    /// <inheritdoc />
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        await context.RefreshTokens.AddAsync(refreshToken, ct);
        await context.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task RevokeAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTimeOffset.UtcNow;
        context.RefreshTokens.Update(refreshToken);
        await context.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task RevokeAllForUserAsync(string userId, CancellationToken ct = default)
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
}
