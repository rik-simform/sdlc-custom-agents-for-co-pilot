#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyProject.Web.Services;

namespace MyProject.Web.Pages.Orders;

public class MyOrdersModel(OrderApiService orderApi, TokenService tokenService) : PageModel
{
    public PaginatedOrderResponse? OrdersPage { get; private set; }
    public IReadOnlyList<OrderDto> Orders { get; private set; } = [];
    public string? ErrorMessage { get; private set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        if (!tokenService.IsAuthenticated())
            return RedirectToPage("/Account/Login");

        orderApi.SetBearerToken(tokenService.GetAccessToken()!);
        var (ordersPage, error) = await orderApi.GetMyOrdersAsync(CurrentPage, PageSize, ct);

        if (error is not null)
        {
            ErrorMessage = error;
            return Page();
        }

        OrdersPage = ordersPage;
        Orders = ordersPage?.Items ?? [];
        return Page();
    }

    public async Task<IActionResult> OnPostCancelAsync(Guid orderId, CancellationToken ct)
    {
        if (!tokenService.IsAuthenticated())
            return RedirectToPage("/Account/Login");

        orderApi.SetBearerToken(tokenService.GetAccessToken()!);
        var (order, error) = await orderApi.CancelOrderAsync(orderId, ct);

        if (error is not null)
        {
            TempData["Error"] = error;
        }
        else
        {
            TempData["Success"] = "Order cancelled successfully!";
        }

        return RedirectToPage();
    }
}
