namespace Forum.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public sealed class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public bool IsAuthenticated { get; private set; }

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        IsAuthenticated = Request.Cookies[Constants.AuthCookie] == bool.TrueString;
    }

    public IActionResult OnPostLogout()
    {
        Response.Cookies.Delete(Constants.AuthCookie);
        return Page();
    }
}