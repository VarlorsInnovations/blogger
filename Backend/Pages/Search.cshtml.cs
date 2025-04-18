using Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        await Task.CompletedTask;

        SearchResult =
        [
            new PostPreviewModel(1, "Talking about depressions", "This is about my past with depressions",
                ["depression"], DateTime.UtcNow),
            new PostPreviewModel(1, "Talking about depressions", "This is about my past with depressions",
                ["depression"], DateTime.UtcNow)
        ];
        
        return Page();
    }
}