using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Backend.Pages;

public class About(ILogger<About> logger, VisitService visitService) : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        await visitService.AddSiteAsync(this.HttpContext);
        return Page();
    }
}