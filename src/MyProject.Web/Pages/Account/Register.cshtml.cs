#nullable enable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyProject.Web.Services;

namespace MyProject.Web.Pages.Account;

public class RegisterModel(AuthApiService authApi) : PageModel
{
    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid) return Page();

        var (data, error) = await authApi.RegisterAsync(
            new RegisterRequest(Input.Email, Input.Password, Input.FirstName, Input.LastName), ct);

        if (error is not null)
        {
            ErrorMessage = error;
            return Page();
        }

        SuccessMessage = $"Account created for {data!.Email}. You can now sign in.";
        return Page();
    }

    public class RegisterInput
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}
