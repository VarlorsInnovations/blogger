using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Backend.Pages.Admin;

[Authorize]
public class CreatePostModel : PageModel
{
    private readonly ILogger<CreatePostModel> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly SignInManager<User> _signInManager;
    
    public List<string> Tags { get; private set; } = new();
    
    public List<RelatedPostPossibilities> RelatedPostPossibilities { get; private set; } = new();
    
    public CreatePostModel(
        SignInManager<User> signInManager, 
        ApplicationDbContext dbContext, 
        ILogger<CreatePostModel> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        Tags = await _dbContext.Tags.Select(x => x.Content).ToListAsync();
        RelatedPostPossibilities = await _dbContext.Posts
            .Select(x => new RelatedPostPossibilities(x.Id, x.Title, x.Summary))
            .ToListAsync();
        
        return Page();
    }
}

public sealed class RelatedPostPossibilities
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Summary { get; set; }

    public RelatedPostPossibilities(int id, string title, string summary)
    {
        Id = id;
        Title = title;
        Summary = summary;
    }
}
