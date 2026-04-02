#nullable enable

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyProject.Domain.Entities;

namespace MyProject.Infrastructure.Data;

/// <summary>
/// Application database context with ASP.NET Core Identity support.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    /// <summary>
    /// Gets or sets the refresh tokens DbSet.
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <summary>Gets or sets the inventory items DbSet.</summary>
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
