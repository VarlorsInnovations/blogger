using System.Diagnostics;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Backend.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel(ILogger<ErrorModel> logger, VisitService visitService) : PageModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public async Task<IActionResult> OnGet()
    {
        await visitService.AddSiteAsync(this.HttpContext);
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return Page();
    }
}