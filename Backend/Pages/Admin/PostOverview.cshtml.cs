using Backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Backend.Pages.Admin;

[Authorize]
public class PostOverviewModel : PageModel
{
    private readonly ILogger<PostOverviewModel> _logger;
    private readonly ApplicationDbContext _dbContext;

    public List<PostPreviewModel> Posts { get; private set; }

    public PostOverviewModel(ILogger<PostOverviewModel> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        Posts = await _dbContext.Posts.Select(x => new PostPreviewModel(
            x.Id,
            x.Title,
            x.Summary,
            x.UrlIdentifier,
            x.Tags.Select(t => t.Content).ToList(),
            x.CreatedAt)).ToListAsync();

        return Page();
    }
}