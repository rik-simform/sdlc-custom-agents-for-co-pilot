#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyProject.Web.Services;

namespace MyProject.Web.Pages.Inventory;

public class IndexModel(InventoryApiService inventoryApi, TokenService tokenService) : PageModel
{
    public IReadOnlyList<InventoryItemResponse> Items { get; private set; } = [];
    public IReadOnlyList<InventoryItemResponse> LowStockItems { get; private set; } = [];
    public string? ErrorMessage { get; private set; }
    public string? Filter { get; private set; }

    public async Task<IActionResult> OnGetAsync(string? filter, CancellationToken ct)
    {
        if (!tokenService.IsAuthenticated())
            return RedirectToPage("/Account/Login");

        inventoryApi.SetBearerToken(tokenService.GetAccessToken()!);
        Filter = filter;

        var (lowStock, _) = await inventoryApi.GetLowStockAsync(ct);
        LowStockItems = lowStock ?? [];

        IReadOnlyList<InventoryItemResponse>? items;
        string? error;

        if (filter == "lowstock")
            (items, error) = (lowStock, null);
        else
            (items, error) = await inventoryApi.GetAllAsync(ct);

        if (error is not null)
        {
            ErrorMessage = error;
            return Page();
        }

        Items = items ?? [];
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id, CancellationToken ct)
    {
        if (!tokenService.IsAuthenticated())
            return RedirectToPage("/Account/Login");

        inventoryApi.SetBearerToken(tokenService.GetAccessToken()!);
        var error = await inventoryApi.DeleteAsync(id, ct);

        if (error is not null)
            TempData["Error"] = error;

        return RedirectToPage();
    }
}
