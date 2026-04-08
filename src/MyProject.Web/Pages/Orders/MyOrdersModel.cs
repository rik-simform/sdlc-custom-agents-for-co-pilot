#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyProject.Web.Services;

namespace MyProject.Web.Pages.Orders;

public class MyOrdersModel(OrderApiService orderApi, TokenService tokenService) : PageModel
{
    public IReadOnlyList<OrderResponse> Orders { get; private set; } = [];
    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        if (!tokenService.IsAuthenticated())
            return RedirectToPage("/Account/Login");

        orderApi.SetBearerToken(tokenService.GetAccessToken()!);
        var (orders, error) = await orderApi.GetMyOrdersAsync(ct);

        if (error is not null)
        {
            ErrorMessage = error;
            return Page();
        }

        Orders = orders ?? [];
        return Page();
    }
}