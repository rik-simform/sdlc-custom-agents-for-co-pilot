#nullable enable

using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MyProject.Application.Common;
using MyProject.Domain.Entities;

namespace MyProject.Application.Features.Auth.Commands;

/// <summary>
/// Command to register a new user account.
/// </summary>
/// <param name="Email">The user's email address.</param>
/// <param name="Password">The user's chosen password.</param>
/// <param name="FirstName">The user's first name.</param>
/// <param name="LastName">The user's last name.</param>
public record RegisterCommand(string Email, string Password, string FirstName, string LastName)
    : IRequest<Result<string>>;

/// <summary>
/// Handles user registration: creates the Identity user and assigns the default role.
/// </summary>
public class RegisterCommandHandler(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<RegisterCommandHandler> logger)
    : IRequestHandler<RegisterCommand, Result<string>>
{
    private const string DefaultRole = "User";

    /// <summary>
    /// Registers a new user and assigns the default role.
    /// </summary>
    /// <param name="request">The registration command.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The new user's ID on success, or an error message on failure.</returns>
    public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken ct)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return Result<string>.Fail("A user with this email already exists");
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            logger.LogWarning("Registration failed for {Email}: {Errors}", request.Email, errors);
            return Result<string>.Fail(errors);
        }

        // Ensure the default role exists before assigning
        if (!await roleManager.RoleExistsAsync(DefaultRole))
        {
            await roleManager.CreateAsync(new IdentityRole(DefaultRole));
        }

        await userManager.AddToRoleAsync(user, DefaultRole);

        logger.LogInformation("User {Email} registered successfully with ID {UserId}", request.Email, user.Id);

        return Result<string>.Ok(user.Id);
    }
}
