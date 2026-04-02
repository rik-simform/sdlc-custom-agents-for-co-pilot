#nullable enable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyProject.Web.Services;

namespace MyProject.Web.Pages.Inventory;

public class CreateModel(InventoryApiService inventoryApi, TokenService tokenService) : PageModel
{
    [BindProperty]
    public CreateInput Input { get; set; } = new();

    public string? ErrorMessage { get; private set; }

    public IActionResult OnGet()
    {
        if (!tokenService.IsAuthenticated())
            return RedirectToPage("/Account/Login");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!tokenService.IsAuthenticated())
            return RedirectToPage("/Account/Login");

        if (!ModelState.IsValid) return Page();

        inventoryApi.SetBearerToken(tokenService.GetAccessToken()!);

        var (_, error) = await inventoryApi.CreateAsync(new CreateInventoryItemRequest(
            Input.Sku, Input.Name, Input.Description, Input.Category,
            Input.QuantityInStock, Input.ReorderLevel, Input.UnitPrice, Input.Location), ct);

        if (error is not null)
        {
            ErrorMessage = error;
            return Page();
        }

        return RedirectToPage("/Inventory/Index");
    }

    public class CreateInput
    {
        [Required, MaxLength(50)]
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
    }
}
