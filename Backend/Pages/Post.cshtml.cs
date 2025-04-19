using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Backend.Pages;

public sealed class PostViewModel
{
    public int Id { get; }

    public string Title { get; }
    
    public List<string> Tags { get; }

    public DateTime CreatedAt { get; }
    
    public List<ContentPart> Parts { get; }
    
    public List<PostPreviewModel> RelatedPosts { get; }

    public PostViewModel(
        int id,
        string title,
        List<string> tags,
        DateTime createdAt,
        List<ContentPart> parts,
        List<PostPreviewModel> relatedPosts)
    {
        Id = id;
        Title = title;
        Tags = tags;
        CreatedAt = createdAt;
        Parts = parts;
        RelatedPosts = relatedPosts;
    }
}

[AllowAnonymous]
public class PostModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;
    
    public PostModel(ApplicationDbContext dbContext) => _dbContext = dbContext;

    [BindProperty] public PostViewModel? Post { get; private set; }
    
    public async Task<IActionResult> OnGetAsync(string? id)
    {
        if (id is null)
        {
            return NotFound();
        }
        
        Post? post = await _dbContext.Posts
            .Include(post => post.Parts)
            .Include(post => post.Tags)
            .FirstOrDefaultAsync(x => x.UrlIdentifier == id);
        
        if (post is null)
        {
            return Page();
        }
    
        Post = new PostViewModel(
            post.Id,
            post.Title, 
            post.Tags.Select(x => x.Content).ToList(),
            post.CreatedAt, 
            post.Parts,
            post.Relations.Select(x => new PostPreviewModel(
                x.Id, 
                x.Title, 
                x.Summary,
                x.UrlIdentifier,
                x.Tags.Select(t => t.Content).ToList(),
                x.CreatedAt)).ToList());
        
        return Page();
    }
}