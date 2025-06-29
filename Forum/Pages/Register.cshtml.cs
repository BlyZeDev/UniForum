namespace Forum.Pages;

using Forum.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

public sealed class RegisterModel : PageModel
{
    private readonly ILogger<RegisterModel> _logger;
    private readonly DatabaseService _database;

    [BindProperty]
    public required InputModel Input { get; set; }

    public RegisterModel(ILogger<RegisterModel> logger, DatabaseService database)
    {
        _logger = logger;
        _database = database;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        TempData[Constants.AlertMessage] = "Die Registrierung ist fehlgeschlagen";

        if (!ModelState.IsValid) return Page();

        var isEmailTaken = await _database.IsEmailTaken(Input.Email);
        if (isEmailTaken.IsFailed || isEmailTaken.Value) return Page();

        TempData[Constants.AlertMessage] = "Test Email not taken";
        return Page();
    }

    public sealed class InputModel
    {
        [Required(ErrorMessage = "Dieses Feld ist nicht optional.")]
        [EmailAddress(ErrorMessage = "Keine gültige E-Mail Adresse")]
        [Display(Name = "E-Mail Adresse")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Dieses Feld ist nicht optional.")]
        [MinLength(3, ErrorMessage = "Die Mindestlänge beträgt 3 Zeichen.")]
        [MaxLength(50, ErrorMessage = "Die Maximallänge beträgt 50 Zeichen.")]
        [Display(Name = "Benutzername")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Dieses Feld ist nicht optional.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Die Mindestlänge beträgt 8 Zeichen.")]
        [MaxLength(100, ErrorMessage = "Die Maximallänge beträgt 100 Zeichen.")]
        [Display(Name = "Passwort")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Dieses Feld ist nicht optional.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Die Mindestlänge beträgt 8 Zeichen.")]
        [MaxLength(100, ErrorMessage = "Die Maximallänge beträgt 100 Zeichen.")]
        [Compare(nameof(Password), ErrorMessage = "Die Passwörter stimmen nicht überein.")]
        [Display(Name = "Passwort bestätigen")]
        public required string ConfirmPassword { get; set; }
    }
}