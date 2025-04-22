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