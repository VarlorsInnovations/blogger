using Backend.Data;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Backend.Pages;

public class SearchModel(ApplicationDbContext dbContext, VisitService visitService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }
    
    [BindProperty] 
    public List<PostPreviewModel>? SearchResult { get; private set; }

    [BindProperty] public List<PostPreviewModel> RecentPosts { get; private set; } = [];
    
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
                x.Tags.Select(t => t.Content).ToList(),
                x.CreatedAt))
            .ToListAsync();
        
        if (SearchTerm is null)
        {
            return Page();
        }

        SearchResult = await dbContext.Posts
            .Where(x => x.IsPublished)
            .Where(x => 
                x.Title.ToLower().Contains(SearchTerm.ToLower()) ||  
                x.Tags.Select(t => t.Content.ToLower()).Contains(SearchTerm.ToLower()))
            .Select(x => new PostPreviewModel(
                x.Id, 
                x.Title, 
                x.Summary,
                x.UrlIdentifier,
                x.Tags.Select(t => t.Content).ToList(), 
                x.CreatedAt))
            .ToListAsync();
        
        return Page();
    }
}