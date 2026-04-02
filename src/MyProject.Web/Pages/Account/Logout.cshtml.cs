#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyProject.Web.Services;

namespace MyProject.Web.Pages.Account;

public class LogoutModel(AuthApiService authApi, TokenService tokenService) : PageModel
{
    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var refreshToken = tokenService.GetRefreshToken();
        if (refreshToken is not null)
            await authApi.RevokeAsync(refreshToken, ct);

        tokenService.Clear();
        return RedirectToPage("/Account/Login");
    }
}
