using Backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Backend.Pages;

public sealed class PostPreviewModel
{
    public int Id { get; }

    public string Title { get; }

    public string Summary { get; }

    public List<string> Tags { get; }
    
    public DateTime CreatedAt { get; }

    public PostPreviewModel(
        int id,
        string title,
        string summary,
        List<string> tags,
        DateTime createdAt)
    {
        Id = id;
        Title = title;
        Summary = summary;
        Tags = tags;
        CreatedAt = createdAt;
    }
}

    
[AllowAnonymous]
public class PostsModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;

    public List<PostPreviewModel> Items { get; private set; }

    public PostsModel(ApplicationDbContext dbContext)
        => _dbContext = dbContext;
    
    public async Task<IActionResult> OnGetAsync()
    {
        Items = await _dbContext.Posts.Select(
                x => new PostPreviewModel(
                    x.Id, 
                    x.Title,
                    x.Summary,
                    x.Tags.Select(t => t.Tag.Content).ToList(), x.CreatedAt))
            .ToListAsync();
        
        return Page();
    }
}