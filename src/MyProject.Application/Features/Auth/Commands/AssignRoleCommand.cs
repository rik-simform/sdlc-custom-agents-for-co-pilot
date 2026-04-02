#nullable enable

using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MyProject.Application.Common;
using MyProject.Domain.Entities;

namespace MyProject.Application.Features.Auth.Commands;

/// <summary>
/// Command to assign a role to a user. Only Admin users can invoke this.
/// </summary>
/// <param name="UserId">The target user's ID.</param>
/// <param name="Role">The role to assign.</param>
/// <param name="ActorId">The Admin user performing the assignment.</param>
public record AssignRoleCommand(string UserId, string Role, string ActorId)
    : IRequest<Result<string>>;

/// <summary>
/// Handles <see cref="AssignRoleCommand"/>: validates the role exists, the target user exists,
/// and assigns the role idempotently.
/// </summary>
public class AssignRoleCommandHandler(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<AssignRoleCommandHandler> logger)
    : IRequestHandler<AssignRoleCommand, Result<string>>
{
    /// <summary>
    /// Assigns the specified role to the target user.
    /// </summary>
    public async Task<Result<string>> Handle(AssignRoleCommand request, CancellationToken ct)
    {
        // AC-004: Validate that the role exists
        if (!await roleManager.RoleExistsAsync(request.Role))
        {
            return Result<string>.Fail($"Role '{request.Role}' does not exist");
        }

        var user = await userManager.FindByIdAsync(request.UserId);
        if (user is null)
        {
            return Result<string>.Fail($"User '{request.UserId}' not found");
        }

        // AC-005: Idempotent — if user already has the role, return success
        if (await userManager.IsInRoleAsync(user, request.Role))
        {
            logger.LogInformation(
                "Role assignment idempotent: User {TargetUserId} already has role {Role}. Actor: {ActorId}",
                request.UserId, request.Role, request.ActorId);

            return Result<string>.Ok($"User already has role '{request.Role}'");
        }

        var result = await userManager.AddToRoleAsync(user, request.Role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            logger.LogWarning("Role assignment failed for user {TargetUserId}: {Errors}", request.UserId, errors);
            return Result<string>.Fail(errors);
        }

        // AC-007: Audit log entry
        logger.LogInformation(
            "RoleAssigned: User {TargetUserId} assigned role {Role} by Admin {ActorId}",
            request.UserId, request.Role, request.ActorId);

        return Result<string>.Ok($"Role '{request.Role}' assigned to user '{request.UserId}'");
    }
}
