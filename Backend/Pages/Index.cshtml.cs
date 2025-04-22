using Backend.Data;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Backend.Pages;

public class IndexModel(
    ApplicationDbContext dbContext,
    ILogger<IndexModel> logger,
    VisitService visitService)
    : PageModel
{
    private readonly ILogger<IndexModel> _logger = logger;

    [BindProperty] public List<PostPreviewModel> RecentPosts { get; private set; }

    [BindProperty] public List<PostPreviewModel> MostViewedPosts { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        await visitService.AddSiteAsync(this.HttpContext);

        RecentPosts = await dbContext.Posts
            .Where(x => x.IsPublished)
            .Where(x => x.CreatedAt >= DateTime.UtcNow.AddDays(-10))
            .Select(x => new PostPreviewModel(
                x.Id, 
                x.Title, 
                x.Summary, 
                x.UrlIdentifier,
                x.Tags.Select(y => y.Content).ToList(), x.CreatedAt))
            .ToListAsync();


        var grouping = await dbContext.Visits
            .Include(x => x.Post)
                .ThenInclude(x => x!.Tags)
            .Where(x => x.Post != null)
            .Where(x => x.Post!.IsPublished)
            .GroupBy(x => x.Post)
            .Select(x => new { Post = x.Key, Count = x.Count() })
            .OrderBy(x => x.Count)
            .Take(3)
            .Select(x => x.Post)
            .ToListAsync();

        MostViewedPosts = grouping.Select(x => 
            new PostPreviewModel(
                x.Id,
                x.Title,
                x.Summary,
                x.UrlIdentifier,
                x.Tags?.Select(y => y.Content).ToList() ?? [],
                x.CreatedAt))
            .ToList();
        
        return Page();
    }
}