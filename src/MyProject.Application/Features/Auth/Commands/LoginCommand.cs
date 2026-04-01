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
/// Command to authenticate a user with email and password.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
/// <param name="DeviceInfo">Optional device information from the request.</param>
public record LoginCommand(string Email, string Password, string? DeviceInfo = null)
    : IRequest<Result<LoginResponse>>;

/// <summary>
/// Handles user login authentication, JWT generation, and refresh token creation.
/// </summary>
public class LoginCommandHandler(
    UserManager<ApplicationUser> userManager,
    ITokenService tokenService,
    IRefreshTokenRepository refreshTokenRepository,
    ILogger<LoginCommandHandler> logger)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    /// <summary>
    /// Authenticates the user and issues JWT + refresh tokens.
    /// </summary>
    /// <param name="request">The login command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A result containing the login response or an error.</returns>
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        // AC-002 / AC-003: Use generic error message for both invalid email and wrong password
        // to prevent credential enumeration. Consistent timing prevents timing attacks.
        const string invalidCredentialsMessage = "Invalid email or password";

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            // AC-003: Still check a dummy password to ensure consistent response time
            // This prevents timing attacks that detect whether an email exists
            await userManager.CheckPasswordAsync(new ApplicationUser(), request.Password);
            logger.LogWarning("Login attempt for non-existent email");
            return Result<LoginResponse>.Fail(invalidCredentialsMessage);
        }

        var isValidPassword = await userManager.CheckPasswordAsync(user, request.Password);
        if (!isValidPassword)
        {
            logger.LogWarning("Failed login attempt for user {UserId}", user.Id);
            return Result<LoginResponse>.Fail(invalidCredentialsMessage);
        }

        var roles = await userManager.GetRolesAsync(user);

        // AC-001: Generate JWT access token (15 min expiry) and opaque refresh token (7 day expiry)
        var (accessToken, expiresAt) = tokenService.GenerateAccessToken(user, roles);
        var refreshTokenValue = tokenService.GenerateRefreshToken();
        var refreshTokenHash = tokenService.HashToken(refreshTokenValue);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
            DeviceInfo = request.DeviceInfo
        };

        await refreshTokenRepository.AddAsync(refreshToken, ct);

        // AC-009 (Security): Never log tokens or passwords
        logger.LogInformation("User {UserId} authenticated successfully", user.Id);

        return Result<LoginResponse>.Ok(
            new LoginResponse(accessToken, refreshTokenValue, expiresAt));
    }
}
