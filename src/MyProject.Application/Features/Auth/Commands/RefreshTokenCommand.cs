#nullable enable

using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MyProject.Application.Common;
using MyProject.Application.Features.Auth.DTOs;
using MyProject.Domain.Entities;
using MyProject.Domain.Interfaces;

namespace MyProject.Application.Features.Auth.Commands;

/// <summary>
/// Command to refresh an access token using a valid refresh token.
/// Implements one-time-use token rotation per AC-005.
/// </summary>
/// <param name="RefreshToken">The current refresh token value.</param>
/// <param name="DeviceInfo">Optional device information from the request.</param>
public record RefreshTokenCommand(string RefreshToken, string? DeviceInfo = null)
    : IRequest<Result<LoginResponse>>;

/// <summary>
/// Handles refresh token rotation: validates the existing token, revokes it,
/// and issues new JWT + refresh tokens.
/// </summary>
public class RefreshTokenCommandHandler(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService,
    IRefreshTokenRepository refreshTokenRepository,
    ILogger<RefreshTokenCommandHandler> logger)
    : IRequestHandler<RefreshTokenCommand, Result<LoginResponse>>
{
    /// <summary>
    /// Validates the refresh token and issues new tokens with rotation.
    /// </summary>
    /// <param name="request">The refresh token command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing new tokens or an error.</returns>
    public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var tokenHash = tokenService.HashToken(request.RefreshToken);
        var existingToken = await refreshTokenRepository.GetByTokenHashAsync(tokenHash, ct);

        if (existingToken is null || !existingToken.IsActive)
        {
            logger.LogWarning("Invalid or expired refresh token presented");
            return Result<LoginResponse>.Fail("Invalid or expired refresh token");
        }

        var user = await userManager.FindByIdAsync(existingToken.UserId);
        if (user is null)
        {
            logger.LogWarning("Refresh token references non-existent user {UserId}", existingToken.UserId);
            return Result<LoginResponse>.Fail("Invalid or expired refresh token");
        }

        // AC-005: Revoke old token (one-time use / rotation)
        await refreshTokenRepository.RevokeAsync(existingToken, ct);

        var roles = await userManager.GetRolesAsync(user);

        // Issue new tokens
        var (accessToken, expiresAt) = tokenService.GenerateAccessToken(user, roles);
        var newRefreshTokenValue = tokenService.GenerateRefreshToken();
        var newRefreshTokenHash = tokenService.HashToken(newRefreshTokenValue);

        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = newRefreshTokenHash,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
            DeviceInfo = request.DeviceInfo
        };

        await refreshTokenRepository.AddAsync(newRefreshToken, ct);

        logger.LogInformation("Refresh token rotated for user {UserId}", user.Id);

        return Result<LoginResponse>.Ok(
            new LoginResponse(accessToken, newRefreshTokenValue, expiresAt));
    }
}
