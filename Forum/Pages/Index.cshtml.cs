namespace Forum.Pages;

using Microsoft.AspNetCore.Mvc.RazorPages;

public sealed class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }
}