#nullable enable

using MediatR;
using Microsoft.Extensions.Logging;
using MyProject.Application.Common;
using MyProject.Domain.Interfaces;

namespace MyProject.Application.Features.Auth.Commands;

/// <summary>
/// Command to revoke a refresh token (logout).
/// </summary>
/// <param name="RefreshToken">The refresh token to revoke.</param>
public record RevokeTokenCommand(string RefreshToken) : IRequest<Result<bool>>;

/// <summary>
/// Handles revoking a refresh token for logout functionality.
/// </summary>
public class RevokeTokenCommandHandler(
    ITokenService tokenService,
    IRefreshTokenRepository refreshTokenRepository,
    ILogger<RevokeTokenCommandHandler> logger)
    : IRequestHandler<RevokeTokenCommand, Result<bool>>
{
    /// <summary>
    /// Revokes the specified refresh token.
    /// </summary>
    /// <param name="request">The revoke command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result indicating success or failure.</returns>
    public async Task<Result<bool>> Handle(RevokeTokenCommand request, CancellationToken ct)
    {
        var tokenHash = tokenService.HashToken(request.RefreshToken);
        var existingToken = await refreshTokenRepository.GetByTokenHashAsync(tokenHash, ct);

        if (existingToken is null || !existingToken.IsActive)
        {
            // Return success even if token not found to prevent enumeration
            return Result<bool>.Ok(true);
        }

        await refreshTokenRepository.RevokeAsync(existingToken, ct);

        logger.LogInformation("Refresh token revoked for user {UserId}", existingToken.UserId);

        return Result<bool>.Ok(true);
    }
}
