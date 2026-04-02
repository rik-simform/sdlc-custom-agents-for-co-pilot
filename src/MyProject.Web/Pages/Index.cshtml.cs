using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyProject.Web.Services;

namespace MyProject.Web.Pages;

public class IndexModel(TokenService tokenService) : PageModel
{
    public IActionResult OnGet() =>
        tokenService.IsAuthenticated()
            ? RedirectToPage("/Inventory/Index")
            : RedirectToPage("/Account/Login");
}
