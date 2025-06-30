namespace Forum.Pages;

using Forum.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public sealed class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly DatabaseService _database;

    private IAsyncEnumerator<EntryDto>? deferredEntries;

    [BindProperty]
    public required string Title { get; set; }

    [BindProperty]
    public required string Text { get; set; }

    public bool IsAuthenticated => Request.Cookies.ContainsKey(Constants.EmailAuthCookie);

    public IndexModel(ILogger<IndexModel> logger, DatabaseService database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<IActionResult> OnGetEntriesAsync()
    {
        deferredEntries ??= _database.EnumerateEntriesAsync().GetAsyncEnumerator();

        var entries = new List<object>(10);
        for (int i = 0; i < 10; i++)
        {
            var isDataAvailable = await deferredEntries.MoveNextAsync();
            if (!isDataAvailable)
            {
                deferredEntries = null;
                break;
            }

            entries.Add(deferredEntries.Current);
        }

        return new JsonResult(entries);
    }

    public async Task<IActionResult> OnPostCreateEntryAsync()
    {
        TempData[Constants.AlertMessage] = "Beitrag konnte nicht erstellt werden";

        if (!ModelState.IsValid) return Page();
        if (!IsAuthenticated) return Page();

        var result = await _database.CreateEntryAsync(Request.Cookies[Constants.EmailAuthCookie] ?? "", Title, Text);
        if (result.IsFailed) return Page();

        TempData[Constants.AlertMessage] = "Beitrag erfolgreich erstellt";
        return RedirectToPage();
    }

    public IActionResult OnPostLogout()
    {
        Response.Cookies.Delete(Constants.EmailAuthCookie);
        Response.Cookies.Delete(Constants.UsernameAuthCookie);

        return RedirectToPage();
    }
}