#nullable enable

using System.Security.Claims;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyProject.Application.Features.Auth.Commands;
using MyProject.Application.Features.Auth.DTOs;

namespace MyProject.Api.Endpoints;

/// <summary>
/// Defines authentication API endpoints using Minimal API.
/// </summary>
public static class AuthEndpoints
{
    /// <summary>
    /// Maps authentication routes to the application.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The route group builder.</returns>
    public static RouteGroupBuilder MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/auth")
            .WithTags("Authentication");

        group.MapPost("/register", Register)
            .AllowAnonymous()
            .Produces<RegisterResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("Register")
            .WithSummary("Register a new user account")
            .WithDescription("Creates a new user account with the provided credentials");

        group.MapPost("/login", Login)
            .AllowAnonymous()
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("Login")
            .WithSummary("Authenticate and issue tokens")
            .WithDescription("Authenticates user credentials and returns JWT access token and refresh token");

        group.MapPost("/refresh", Refresh)
            .AllowAnonymous()
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("RefreshToken")
            .WithSummary("Refresh access token")
            .WithDescription("Exchanges a valid refresh token for a new access token with token rotation");

        group.MapPost("/revoke", Revoke)
            .AllowAnonymous()
            .Produces(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("RevokeToken")
            .WithSummary("Revoke refresh token (logout)")
            .WithDescription("Revokes a refresh token to log out the user");

        group.MapPost("/roles/assign", AssignRole)
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("AssignRole")
            .WithSummary("Assign a role to a user")
            .WithDescription("Assigns a role to a target user. Only Admin users can invoke this endpoint.");

        return group;
    }

    /// <summary>
    /// POST /api/v1/auth/register — Register a new user account.
    /// </summary>
    private static async Task<IResult> Register(
        [FromBody] RegisterRequest request,
        IValidator<RegisterRequest> validator,
        IMediator mediator,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var command = new RegisterCommand(request.Email, request.Password, request.FirstName, request.LastName);
        var result = await mediator.Send(command, ct);

        if (!result.IsSuccess)
        {
            return Results.Problem(
                title: "Registration failed",
                detail: result.Error,
                statusCode: StatusCodes.Status400BadRequest);
        }

        var response = new RegisterResponse(result.Value!, request.Email);
        return Results.Created($"/api/v1/users/{result.Value}", response);
    }

    /// <summary>
    /// POST /api/v1/auth/login — Authenticate and issue tokens.
    /// AC-001: Returns JWT (15 min) + refresh token (7 days).
    /// AC-002/AC-003: Returns generic 401 for invalid credentials.
    /// </summary>
    private static async Task<IResult> Login(
        [FromBody] LoginRequest request,
        IValidator<LoginRequest> validator,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var deviceInfo = httpContext.Request.Headers.UserAgent.ToString();
        var command = new LoginCommand(request.Email, request.Password, deviceInfo);
        var result = await mediator.Send(command, ct);

        if (!result.IsSuccess)
        {
            return Results.Problem(
                title: "Authentication failed",
                detail: result.Error,
                statusCode: StatusCodes.Status401Unauthorized);
        }

        return Results.Ok(result.Value);
    }

    /// <summary>
    /// POST /api/v1/auth/refresh — Refresh access token with token rotation.
    /// AC-005: Old refresh token is revoked (one-time use).
    /// </summary>
    private static async Task<IResult> Refresh(
        [FromBody] RefreshTokenRequest request,
        IValidator<RefreshTokenRequest> validator,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var deviceInfo = httpContext.Request.Headers.UserAgent.ToString();
        var command = new RefreshTokenCommand(request.RefreshToken, deviceInfo);
        var result = await mediator.Send(command, ct);

        if (!result.IsSuccess)
        {
            return Results.Problem(
                title: "Token refresh failed",
                detail: result.Error,
                statusCode: StatusCodes.Status401Unauthorized);
        }

        return Results.Ok(result.Value);
    }

    /// <summary>
    /// POST /api/v1/auth/revoke — Revoke a refresh token (logout).
    /// </summary>
    private static async Task<IResult> Revoke(
        [FromBody] RevokeTokenRequest request,
        IValidator<RevokeTokenRequest> validator,
        IMediator mediator,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var command = new RevokeTokenCommand(request.RefreshToken);
        await mediator.Send(command, ct);

        return Results.Ok(new { Message = "Token revoked successfully" });
    }

    /// <summary>
    /// POST /api/v1/auth/roles/assign — Assign a role to a user (Admin only).
    /// </summary>
    private static async Task<IResult> AssignRole(
        [FromBody] AssignRoleRequest request,
        IValidator<AssignRoleRequest> validator,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var actorId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var command = new AssignRoleCommand(request.UserId, request.Role, actorId);
        var result = await mediator.Send(command, ct);

        if (!result.IsSuccess)
        {
            return Results.Problem(
                title: "Role assignment failed",
                detail: result.Error,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return Results.Ok(new { Message = result.Value });
    }
}
