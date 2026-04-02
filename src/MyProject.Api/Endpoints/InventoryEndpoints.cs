#nullable enable

using System.Security.Claims;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyProject.Application.Features.Inventory.Commands;
using MyProject.Application.Features.Inventory.DTOs;
using MyProject.Application.Features.Inventory.Queries;

namespace MyProject.Api.Endpoints;

/// <summary>Inventory management API endpoints — all routes require a valid JWT.</summary>
public static class InventoryEndpoints
{
    /// <summary>Maps all inventory routes onto the application.</summary>
    public static RouteGroupBuilder MapInventoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/inventory")
            .WithTags("Inventory Management")
            .RequireAuthorization();

        group.MapGet("/", GetAll)
            .WithName("GetAllInventoryItems")
            .WithSummary("List inventory items")
            .WithDescription("Returns all active items. Filter with ?category=X or ?search=Y.")
            .Produces<IEnumerable<InventoryItemResponse>>();

        group.MapGet("/reorder", GetReorder)
            .WithName("GetItemsNeedingReorder")
            .WithSummary("Items needing reorder")
            .WithDescription("Returns items where quantity ≤ reorder level.")
            .Produces<IEnumerable<InventoryItemResponse>>();

        group.MapGet("/{id:guid}", GetById)
            .WithName("GetInventoryItemById")
            .WithSummary("Get item by ID")
            .Produces<InventoryItemResponse>()
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        group.MapPost("/", Create)
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("CreateInventoryItem")
            .WithSummary("Create inventory item")
            .Produces<InventoryItemResponse>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden);

        group.MapPut("/{id:guid}", Update)
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("UpdateInventoryItem")
            .WithSummary("Update inventory item")
            .Produces<InventoryItemResponse>()
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden);

        group.MapDelete("/{id:guid}", Delete)
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("DeleteInventoryItem")
            .WithSummary("Delete (soft) inventory item")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden);

        return group;
    }

    private static async Task<IResult> GetAll(
        string? category,
        string? search,
        IMediator mediator,
        CancellationToken ct)
    {
        var items = await mediator.Send(new GetInventoryItemsQuery(category, search), ct);
        return Results.Ok(items);
    }

    private static async Task<IResult> GetReorder(IMediator mediator, CancellationToken ct)
    {
        var items = await mediator.Send(new GetItemsNeedingReorderQuery(), ct);
        return Results.Ok(items);
    }

    private static async Task<IResult> GetById(Guid id, IMediator mediator, CancellationToken ct)
    {
        var item = await mediator.Send(new GetInventoryItemByIdQuery(id), ct);
        return item is null
            ? Results.Problem(title: "Not found", detail: "Inventory item not found", statusCode: StatusCodes.Status404NotFound)
            : Results.Ok(item);
    }

    private static async Task<IResult> Create(
        [FromBody] CreateInventoryItemRequest request,
        IValidator<CreateInventoryItemRequest> validator,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await mediator.Send(
            new CreateInventoryItemCommand(request.Sku, request.Name, request.Description,
                request.Category, request.QuantityInStock, request.ReorderLevel,
                request.UnitPrice, request.Location, userId), ct);

        return result.IsSuccess
            ? Results.Created($"/api/v1/inventory/{result.Value!.Id}", result.Value)
            : Results.Problem(title: "Create failed", detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> Update(
        Guid id,
        [FromBody] UpdateInventoryItemRequest request,
        IValidator<UpdateInventoryItemRequest> validator,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await mediator.Send(
            new UpdateInventoryItemCommand(id, request.Name, request.Description,
                request.Category, request.QuantityInStock, request.ReorderLevel,
                request.UnitPrice, request.Location, request.IsActive, userId), ct);

        if (!result.IsSuccess)
        {
            var status = result.Error!.Contains("not found")
                ? StatusCodes.Status404NotFound
                : StatusCodes.Status400BadRequest;
            return Results.Problem(title: "Update failed", detail: result.Error, statusCode: status);
        }

        return Results.Ok(result.Value);
    }

    private static async Task<IResult> Delete(
        Guid id,
        IMediator mediator,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await mediator.Send(new DeleteInventoryItemCommand(id, userId), ct);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.Problem(title: "Delete failed", detail: result.Error, statusCode: StatusCodes.Status404NotFound);
    }
}
