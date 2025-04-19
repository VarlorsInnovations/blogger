using Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Backend.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<IndexModel> _logger;

    [BindProperty] public List<PostPreviewModel> RecentPosts { get; private set; }

    [BindProperty] public List<PostPreviewModel> MostViewedPosts { get; private set; }
    
    public IndexModel(
        ApplicationDbContext dbContext,
        ILogger<IndexModel> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        RecentPosts = await _dbContext.Posts
            .Where(x => x.CreatedAt >= DateTime.UtcNow.AddDays(-10))
            .Select(x => new PostPreviewModel(x.Id, x.Title, x.Summary, x.Tags.Select(y => y.Tag.Content).ToList(), x.CreatedAt))
            .ToListAsync();
        
        // todo: impl. most viewed posts (rating system)
        
        MostViewedPosts =
        [
            new PostPreviewModel(1, "My experience with depressions", "my summary", ["depression"], DateTime.UtcNow),
            new PostPreviewModel(2, "Tips for parents", "my summary2", ["info"], DateTime.UtcNow),
        ];
        
        await Task.CompletedTask;
        return Page();
    }
}