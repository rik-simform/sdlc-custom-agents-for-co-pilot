#nullable enable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyProject.Web.Services;

namespace MyProject.Web.Pages.Account;

public class LoginModel(AuthApiService authApi, TokenService tokenService) : PageModel
{
    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        if (tokenService.IsAuthenticated())
            return RedirectToPage("/Inventory/Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            var (data, error) = await authApi.LoginAsync(
                new LoginRequest(Input.Email, Input.Password), ct);

            if (error is not null)
            {
                ErrorMessage = $"Login failed: {error}";
                return Page();
            }

            if (data is null)
            {
                ErrorMessage = "Login failed: No data returned from API";
                return Page();
            }

            tokenService.StoreTokens(data.AccessToken, data.RefreshToken, Input.Email, data.Roles);
            return RedirectToPage("/Inventory/Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login error: {ex.Message}";
            return Page();
        }
    }

    public class LoginInput
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
