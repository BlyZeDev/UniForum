namespace Forum.Pages;

using Microsoft.AspNetCore.Mvc.RazorPages;

public sealed class PrivacyModel : PageModel
{
    private readonly ILogger<PrivacyModel> _logger;

    public PrivacyModel(ILogger<PrivacyModel> logger)
    {
        _logger = logger;
    }
}

