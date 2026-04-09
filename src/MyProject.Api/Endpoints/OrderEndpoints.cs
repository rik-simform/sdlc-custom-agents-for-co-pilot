#nullable enable

using System.Security.Claims;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyProject.Application.Features.Orders.Commands;
using MyProject.Application.Features.Orders.DTOs;
using MyProject.Application.Features.Orders.Queries;

namespace MyProject.Api.Endpoints;

/// <summary>Order management API endpoints.</summary>
public static class OrderEndpoints
{
    /// <summary>Maps all order routes onto the application.</summary>
    public static RouteGroupBuilder MapOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/orders")
            .WithTags("Order Management")
            .RequireAuthorization();

        group.MapPost("/", CreateOrder)
            .WithName("CreateOrder")
            .WithSummary("User places a new order");

        group.MapGet("/my-orders", GetMyOrders)
            .WithName("GetMyOrders")
            .WithSummary("View my order history");

        group.MapGet("/{orderId:guid}", GetOrderById)
            .WithName("GetOrderById")
            .WithSummary("Get order details");

        group.MapPut("/{orderId:guid}/cancel", CancelOrder)
            .WithName("CancelOrder")
            .WithSummary("Cancel an order");

        group.MapGet("/", GetAllOrders)
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("GetAllOrders")
            .WithSummary("Admin: View all orders");

        return group;
    }

    private static async Task<IResult> CreateOrder(
        [FromBody] CreateOrderRequest request,
        IValidator<CreateOrderRequest> validator,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await mediator.Send(
            new CreateOrderCommand(
                UserId: userId,
                InventoryItemId: request.InventoryItemId,
                QuantityRequested: request.QuantityRequested,
                Notes: request.Notes),
            ct);

        if (!result.IsSuccess)
        {
            var status = result.Error!.Contains("not found")
                ? StatusCodes.Status404NotFound
                : StatusCodes.Status400BadRequest;
            return Results.Problem(title: "Order creation failed", detail: result.Error, statusCode: status);
        }

        return Results.Created($"/api/v1/orders/{result.Value!.Id}", result.Value);
    }

    private static async Task<IResult> GetMyOrders(
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
            return Results.Problem(title: "Invalid pagination", statusCode: StatusCodes.Status400BadRequest);

        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await mediator.Send(
            new GetUserOrdersQuery(
                UserId: userId,
                Page: page,
                PageSize: pageSize),
            ct);

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> GetOrderById(
        Guid orderId,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = httpContext.User.IsInRole("Admin");

        var result = await mediator.Send(
            new GetOrderByIdQuery(OrderId: orderId),
            ct);

        if (result.Value is null)
            return Results.Problem(title: "Not found", statusCode: StatusCodes.Status404NotFound);

        if (!isAdmin && result.Value.UserId != userId)
            return Results.Problem(title: "Forbidden", statusCode: StatusCodes.Status403Forbidden);

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> CancelOrder(
        Guid orderId,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await mediator.Send(
            new CancelOrderCommand(OrderId: orderId, UserId: userId),
            ct);

        if (!result.IsSuccess)
        {
            var status = result.Error!.Contains("not found")
                ? StatusCodes.Status404NotFound
                : StatusCodes.Status400BadRequest;
            return Results.Problem(title: "Cancel failed", detail: result.Error, statusCode: status);
        }

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> GetAllOrders(
        IMediator mediator,
        CancellationToken ct,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? status = null,
        [FromQuery] string? userFilter = null)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
            return Results.Problem(title: "Invalid pagination", statusCode: StatusCodes.Status400BadRequest);

        var result = await mediator.Send(
            new GetAllOrdersQuery(
                Page: page,
                PageSize: pageSize,
                Status: status,
                UserFilter: userFilter),
            ct);

        return Results.Ok(result.Value);
    }
}
