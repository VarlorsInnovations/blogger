using Backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Backend.Pages.Admin;

[Authorize]
public class CreatePostModel : PageModel
{
    private readonly ILogger<CreatePostModel> _logger;
    private readonly ApplicationDbContext _dbContext;

    public List<string> Tags { get; private set; } = new();
    
    public CreatePostModel(ApplicationDbContext dbContext, ILogger<CreatePostModel> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        Tags = await _dbContext.Tags.Select(x => x.Content).ToListAsync();
        return Page();
    }
}


