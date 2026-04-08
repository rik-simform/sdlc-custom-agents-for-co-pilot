#nullable enable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyProject.Web.Services;

namespace MyProject.Web.Pages.Orders;

[Authorize(Roles = "Admin")]
public class AllOrdersModel(OrderApiService orderApi, TokenService tokenService) : PageModel
{
    public IReadOnlyList<OrderDetailResponse> Orders { get; private set; } = [];
    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        if (!tokenService.IsAuthenticated())
            return RedirectToPage("/Account/Login");

        if (!tokenService.HasRole("Admin"))
            return Forbid();

        orderApi.SetBearerToken(tokenService.GetAccessToken()!);
        var (orders, error) = await orderApi.GetAllOrdersAsync(ct);

        if (error is not null)
        {
            ErrorMessage = error;
            return Page();
        }

        Orders = orders ?? [];
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateOrderStatusAsync(Guid orderId, string newStatus, string? notes, CancellationToken ct)
    {
        if (!tokenService.IsAuthenticated())
            return RedirectToPage("/Account/Login");

        if (!tokenService.HasRole("Admin"))
            return Forbid();

        orderApi.SetBearerToken(tokenService.GetAccessToken()!);
        var (order, error) = await orderApi.UpdateOrderStatusAsync(
            orderId,
            new UpdateOrderStatusRequest(newStatus, notes),
            ct);

        if (error is not null)
        {
            TempData["Error"] = error;
        }
        else
        {
            TempData["Success"] = $"Order status updated to {newStatus}";
        }

        return RedirectToPage();
    }
}
