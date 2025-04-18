using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Backend.Pages;

public sealed class PostViewModel
{
    public int Id { get; }

    public string Title { get; }
    
    public List<string> Tags { get; }

    public DateTime CreatedAt { get; }
    
    public List<ContentPart> Parts { get; }

    public PostViewModel(
        int id,
        string title,
        List<string> tags,
        DateTime createdAt,
        List<ContentPart> parts)
    {
        Id = id;
        Title = title;
        Tags = tags;
        CreatedAt = createdAt;
        Parts = parts;
    }
}

[AllowAnonymous]
public class PostModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;
    
    public PostModel(ApplicationDbContext dbContext) => _dbContext = dbContext;
    
    [BindProperty]
    public PostViewModel? Post { get; private set; }
    
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }
        
        // Post = await _dbContext.Posts.FirstOrDefaultAsync(x => x.Id == id);
        Post = new PostViewModel(
            1, 
            "Talking about depressions", 
            ["Depression"], 
            DateTime.UtcNow,
            [
                new ContentPart() { Id = 1, Content = "In this post i take you on a journey and share my experience with depression.", Type=ContentPartType.Heading1}
            ]);
        
        if (Post is null) 
        {
            return NotFound();
        }

        return Page();
    }
}