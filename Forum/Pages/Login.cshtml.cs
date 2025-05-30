namespace Forum.Pages;

using Forum.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

public sealed class LoginModel : PageModel
{
    private readonly ILogger<LoginModel> _logger;
    private readonly DatabaseService _database;

    [BindProperty]
    public required InputModel Input { get; set; }

    public LoginModel(ILogger<LoginModel> logger, DatabaseService database)
    {
        _logger = logger;
        _database = database;
    }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userResult = await _database.GetUserAsync(Input.Email);
        if (userResult.IsFailed) return Page();

        var user = userResult.Value;

        if (Util.Equals(user.Password, Util.HashPassword(Input.Password)))
        {
            TempData[Constants.TempLoginFailed] = false;

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
        [MinLength(8)]
        [MaxLength(100)]
        public required string Password { get; set; }
    }
}