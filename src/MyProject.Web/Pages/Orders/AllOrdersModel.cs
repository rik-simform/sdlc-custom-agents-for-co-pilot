#nullable enable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyProject.Web.Services;

namespace MyProject.Web.Pages.Orders;

[Authorize(Roles = "Admin")]
public class AllOrdersModel(OrderApiService orderApi, TokenService tokenService) : PageModel
{
    public PaginatedAdminOrderResponse? OrdersPage { get; private set; }
    public IReadOnlyList<AdminOrderDto> Orders { get; private set; } = [];
    public string? ErrorMessage { get; private set; }

    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? UserFilter { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        if (!tokenService.IsAuthenticated())
            return RedirectToPage("/Account/Login");

        if (!tokenService.HasRole("Admin"))
            return Forbid();

        orderApi.SetBearerToken(tokenService.GetAccessToken()!);
        var (ordersPage, error) = await orderApi.GetAllOrdersAsync(CurrentPage, PageSize, Status, UserFilter, ct);

        if (error is not null)
        {
            ErrorMessage = error;
            return Page();
        }

        OrdersPage = ordersPage;
        Orders = ordersPage?.Items ?? [];
        return Page();
    }
}
