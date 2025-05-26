namespace Forum.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

public sealed class LoginModel : PageModel
{
    private readonly ILogger<LoginModel> _logger;

    [BindProperty]
    public required InputModel Input { get; set; }

    public LoginModel(ILogger<LoginModel> logger) => _logger = logger;

    public void OnGet()
    {

    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Input.Email == "test@example.com" && Input.Password == "Password123")
        {
            TempData[Constants.TempLoginFailed] = true;

            Response.Cookies.Append(Constants.AuthCookie, bool.TrueString, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return RedirectToPage("/Index");
        }

        TempData[Constants.TempLoginFailed] = true;
        ModelState.AddModelError("", "Ungültige Anmeldedaten.");
        return Page();
    }

    public sealed class InputModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}