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

/// <summary>
/// Validates user registration request input.
/// </summary>
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    /// <summary>
    /// Initializes validation rules for RegisterRequest.
    /// </summary>
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email address is required")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .MaximumLength(128).WithMessage("Password must not exceed 128 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit")
            .Matches(@"[\W_]").WithMessage("Password must contain at least one special character");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");
    }
}

/// <summary>
/// Validates assign role request input.
/// </summary>
public class AssignRoleRequestValidator : AbstractValidator<AssignRoleRequest>
{
    /// <summary>
    /// Initializes validation rules for AssignRoleRequest.
    /// </summary>
    public AssignRoleRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .MaximumLength(50).WithMessage("Role must not exceed 50 characters");
    }
}
