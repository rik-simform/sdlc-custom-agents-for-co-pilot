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
/// <param name="Roles">The user's assigned roles (e.g., "Admin", "User").</param>
public record LoginResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt, IList<string> Roles);

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

/// <summary>
/// Request model for user registration.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's password.</param>
/// <param name="FirstName">The user's first name.</param>
/// <param name="LastName">The user's last name.</param>
public record RegisterRequest(string Email, string Password, string FirstName, string LastName);

/// <summary>
/// Response model for successful registration.
/// </summary>
/// <param name="UserId">The newly created user's ID.</param>
/// <param name="Email">The user's email address.</param>
public record RegisterResponse(string UserId, string Email);

/// <summary>
/// Request model for assigning a role to a user.
/// </summary>
/// <param name="UserId">The target user's ID.</param>
/// <param name="Role">The role name to assign.</param>
public record AssignRoleRequest(string UserId, string Role);
