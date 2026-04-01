#nullable enable

using FluentValidation;
using MyProject.Application.Features.Auth.DTOs;

namespace MyProject.Application.Features.Auth.Validators;

/// <summary>
/// Validates login request input.
/// </summary>
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    /// <summary>
    /// Initializes validation rules for LoginRequest.
    /// </summary>
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email address is required")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MaximumLength(128).WithMessage("Password must not exceed 128 characters");
    }
}

/// <summary>
/// Validates refresh token request input.
/// </summary>
public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    /// <summary>
    /// Initializes validation rules for RefreshTokenRequest.
    /// </summary>
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required");
    }
}

/// <summary>
/// Validates revoke token request input.
/// </summary>
public class RevokeTokenRequestValidator : AbstractValidator<RevokeTokenRequest>
{
    /// <summary>
    /// Initializes validation rules for RevokeTokenRequest.
    /// </summary>
    public RevokeTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required");
    }
}
