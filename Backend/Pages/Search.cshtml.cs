using Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Backend.Pages;

public class SearchModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }
    
    [BindProperty] 
    public List<PostPreviewModel>? SearchResult { get; private set; }
    
    public SearchModel(ApplicationDbContext dbContext) => _dbContext = dbContext;
    
    public async Task<IActionResult> OnGetAsync()
    {
        if (SearchTerm is null)
        {
            return Page();
        }

        SearchResult = await _dbContext.Posts
            .Select(x => new PostPreviewModel(x.Id, x.Title, x.Summary, x.Tags.Select(t => t.Tag.Content).ToList(), x.CreatedAt))
            .Where(x => x.Title.ToLower().Contains(SearchTerm.ToLower()))
            .Where(x => x.Tags.Select(t => t.ToLower()).Contains(SearchTerm.ToLower()))
            .ToListAsync();
        
        return Page();
    }
}