#nullable enable

namespace MyProject.Application.Features.Auth.DTOs;

/// <summary>
/// Request model for user login.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
public record LoginRequest(string Email, string Password);

/// <summary>
/// Response model returned on successful authentication.
/// </summary>
/// <param name="AccessToken">The JWT access token.</param>
/// <param name="RefreshToken">The opaque refresh token.</param>
/// <param name="ExpiresAt">When the access token expires.</param>
public record LoginResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);

/// <summary>
/// Request model for refreshing an access token.
/// </summary>
/// <param name="RefreshToken">The current refresh token.</param>
public record RefreshTokenRequest(string RefreshToken);

/// <summary>
/// Request model for revoking a refresh token (logout).
/// </summary>
/// <param name="RefreshToken">The refresh token to revoke.</param>
public record RevokeTokenRequest(string RefreshToken);
