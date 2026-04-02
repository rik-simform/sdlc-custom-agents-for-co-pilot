#nullable enable

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyProject.Domain.Entities;
using MyProject.Infrastructure.Data;

namespace MyProject.Api;

/// <summary>
/// Seeds roles, default users, and sample inventory items in Development.
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Applies pending migrations and seeds initial data. Logs errors instead of crashing the host.
    /// </summary>
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Apply any pending migrations (creates DB if it doesn't exist)
            var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying {Count} pending migration(s)...", pendingMigrations.Count());
                await db.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully.");
            }

            await SeedRolesAsync(roleManager, logger);
            await SeedUsersAsync(userManager, logger);
            await SeedInventoryItemsAsync(db, userManager, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database. " +
                "Ensure the connection string is correct and the database server is running. " +
                "If this is the first run, create migrations with: " +
                "dotnet ef migrations add InitialCreate --project src/MyProject.Infrastructure --startup-project src/MyProject.Api");
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
    {
        foreach (var role in new[] { "Admin", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                if (result.Succeeded)
                    logger.LogInformation("Role '{Role}' created.", role);
                else
                    logger.LogWarning("Failed to create role '{Role}': {Errors}", role,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, ILogger logger)
    {
        await CreateUserIfNotExistsAsync(userManager, logger,
            email: "admin@inventory.com",
            firstName: "Admin", lastName: "User",
            password: "Admin@123", role: "Admin");

        await CreateUserIfNotExistsAsync(userManager, logger,
            email: "user@inventory.com",
            firstName: "Test", lastName: "User",
            password: "User@123", role: "User");
    }

    private static async Task CreateUserIfNotExistsAsync(
        UserManager<ApplicationUser> userManager, ILogger logger,
        string email, string firstName, string lastName, string password, string role)
    {
        if (await userManager.FindByEmailAsync(email) is not null)
            return;

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role);
            logger.LogInformation("Seeded user '{Email}' with role '{Role}'.", email, role);
        }
        else
        {
            logger.LogWarning("Failed to seed user '{Email}': {Errors}", email,
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    private static async Task SeedInventoryItemsAsync(
        AppDbContext db, UserManager<ApplicationUser> userManager, ILogger logger)
    {
        if (await db.InventoryItems.AnyAsync())
            return;

        var admin = await userManager.FindByEmailAsync("admin@inventory.com");
        if (admin is null)
        {
            logger.LogWarning("Cannot seed inventory items: admin user not found.");
            return;
        }

        db.InventoryItems.AddRange(
            new InventoryItem { Sku = "LAPTOP-001", Name = "Dell Latitude 5420",    Description = "14-inch business laptop",        Category = "Electronics", QuantityInStock = 15, ReorderLevel = 5,  UnitPrice = 899.99m,   Location = "Warehouse A - Shelf 1", CreatedBy = admin.Id },
            new InventoryItem { Sku = "MOUSE-001",  Name = "Logitech MX Master 3",  Description = "Wireless ergonomic mouse",        Category = "Electronics", QuantityInStock = 3,  ReorderLevel = 10, UnitPrice = 99.99m,    Location = "Warehouse A - Shelf 2", CreatedBy = admin.Id },
            new InventoryItem { Sku = "DESK-001",   Name = "Standing Desk Electric", Description = "Height-adjustable desk",         Category = "Furniture",   QuantityInStock = 8,  ReorderLevel = 3,  UnitPrice = 599.99m,   Location = "Warehouse B - Shelf 1", CreatedBy = admin.Id },
            new InventoryItem { Sku = "CHAIR-001",  Name = "Herman Miller Aeron",   Description = "Ergonomic office chair",          Category = "Furniture",   QuantityInStock = 12, ReorderLevel = 5,  UnitPrice = 1299.99m,  Location = "Warehouse B - Shelf 2", CreatedBy = admin.Id },
            new InventoryItem { Sku = "PHONE-001",  Name = "iPhone 14 Pro",         Description = "256GB Space Black",               Category = "Electronics", QuantityInStock = 25, ReorderLevel = 10, UnitPrice = 999.99m,   Location = "Warehouse A - Shelf 3", CreatedBy = admin.Id }
        );

        await db.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} sample inventory items.", 5);
    }
}
