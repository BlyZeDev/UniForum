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
        TempData[Constants.AlertMessage] = "Der Login ist fehlgeschlagen";

        if (!ModelState.IsValid) return Page();

        var userResult = await _database.GetUserAsync(Input.Email);
        if (userResult.IsFailed) return Page();

        if (Util.Equals(userResult.Value.Password, Util.HashPassword(Input.Password)))
        {
            TempData[Constants.AlertMessage] = null;

            Response.Cookies.Append(Constants.AuthCookie, bool.TrueString, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return RedirectToPage("/Index");
        }

        return Page();
    }

    public sealed class InputModel
    {
        [Required(ErrorMessage = "Dieses Feld ist nicht optional.")]
        [EmailAddress(ErrorMessage = "Keine gültige E-Mail Adresse")]
        [Display(Name = "E-Mail Adresse")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Dieses Feld ist nicht optional.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Die Mindestlänge beträgt 8 Zeichen.")]
        [MaxLength(100, ErrorMessage = "Die Maximallänge beträgt 100 Zeichen.")]
        [Display(Name = "Passwort")]
        public required string Password { get; set; }
    }
}