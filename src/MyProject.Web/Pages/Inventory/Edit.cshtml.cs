#nullable enable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyProject.Web.Services;

namespace MyProject.Web.Pages.Inventory;

public class EditModel(InventoryApiService inventoryApi, TokenService tokenService) : PageModel
{
    [BindProperty]
    public EditInput Input { get; set; } = new();

    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct)
    {
        if (!tokenService.IsAuthenticated())
            return RedirectToPage("/Account/Login");

        if (!tokenService.IsAdmin())
        {
            ErrorMessage = "You do not have permission to edit inventory items.";
            return RedirectToPage("/Account/AccessDenied");
        }

        inventoryApi.SetBearerToken(tokenService.GetAccessToken()!);
        var (item, error) = await inventoryApi.GetByIdAsync(id, ct);

        if (error is not null || item is null)
        {
            ErrorMessage = error ?? "Item not found.";
            return Page();
        }

        Input = new EditInput
        {
            Id       = item.Id,
            Sku      = item.Sku,
            Name     = item.Name,
            Description = item.Description,
            Category = item.Category,
            QuantityInStock = item.QuantityInStock,
            ReorderLevel    = item.ReorderLevel,
            UnitPrice       = item.UnitPrice,
            Location        = item.Location,
            IsActive        = item.IsActive
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken ct)
    {
        if (!tokenService.IsAuthenticated())
            return RedirectToPage("/Account/Login");

        if (!tokenService.IsAdmin())
            return RedirectToPage("/Account/AccessDenied");

        if (!ModelState.IsValid) return Page();

        inventoryApi.SetBearerToken(tokenService.GetAccessToken()!);

        var (_, error) = await inventoryApi.UpdateAsync(id, new UpdateInventoryItemRequest(
            Input.Name, Input.Description, Input.Category,
            Input.QuantityInStock, Input.ReorderLevel, Input.UnitPrice,
            Input.Location, Input.IsActive), ct);

        if (error is not null)
        {
            ErrorMessage = error;
            return Page();
        }

        return RedirectToPage("/Inventory/Index");
    }

    public class EditInput
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required, MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required, Range(0, int.MaxValue)]
        public int QuantityInStock { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; }

        [Required, Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal UnitPrice { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
