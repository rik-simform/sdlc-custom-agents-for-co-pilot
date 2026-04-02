#nullable enable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;

namespace MyProject.Api;

/// <summary>
/// Custom authorization middleware result handler that returns ProblemDetails
/// for 401 and 403 responses per RFC 9457.
/// </summary>
public class CustomAuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    /// <summary>
    /// Handles authorization results, returning ProblemDetails JSON for denied requests.
    /// </summary>
    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Challenged)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = "Authentication is required to access this resource.",
                Status = StatusCodes.Status401Unauthorized
            });
            return;
        }

        if (authorizeResult.Forbidden)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "Forbidden",
                Detail = "You do not have permission to perform this action.",
                Status = StatusCodes.Status403Forbidden
            });
            return;
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
