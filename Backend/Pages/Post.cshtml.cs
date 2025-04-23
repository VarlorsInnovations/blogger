using Backend.Data;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Backend.Pages;

public sealed class PostViewModel(
    int id,
    string title,
    string summary,
    List<string> tags,
    DateTime createdAt,
    List<ContentPart> parts,
    List<PostPreviewModel> relatedPosts)
{
    public int Id { get; } = id;

    public string Title { get; } = title;

    public string Summary { get; } = summary;

    public List<string> Tags { get; } = tags;

    public DateTime CreatedAt { get; } = createdAt;

    public List<ContentPart> Parts { get; } = parts;

    public List<PostPreviewModel> RelatedPosts { get; } = relatedPosts;
}

[AllowAnonymous]
public class PostModel(ApplicationDbContext dbContext, VisitService visitService) : PageModel
{
    [BindProperty] public PostViewModel? Post { get; private set; }
    
    [BindProperty] public List<PostPreviewModel> RecentPosts { get; private set; } 
    
    public async Task<IActionResult> OnGetAsync(string? id)
    {
        if (id is null)
        {
            return NotFound();
        }
        
        Post? post = await dbContext.Posts
            .Include(post => post.Parts)
            .Include(post => post.Tags)
            .Include(post => post.Relations)
            .ThenInclude(post => post.Tags)
            .Where(x => x.IsPublished)
            .FirstOrDefaultAsync(x => x.UrlIdentifier == id);
        
        RecentPosts = await dbContext.Posts
            .Where(x => x.IsPublished)
            .Select(x => new PostPreviewModel(
                x.Id, 
                x.Title, 
                x.Summary, 
                x.UrlIdentifier, 
                x.Tags.Select(t => t.Content).ToList(),
                x.CreatedAt)).ToListAsync();
        
        await visitService.AddSiteAsync(this.HttpContext, post);
        
        if (post is null || !post.IsPublished)
        {
            return Page();
        }
        
        List<PostPreviewModel> relations = post.Relations.Select(x => new PostPreviewModel(
            x.Id,
            x.Title,
            x.Summary,
            x.UrlIdentifier,
            x.Tags.Select(t => t.Content).ToList(),
            x.CreatedAt)).ToList();
        
        Post = new PostViewModel(
            post.Id,
            post.Title, 
            post.Summary,
            post.Tags.Select(x => x.Content).ToList(),
            post.CreatedAt, 
            post.Parts,
            relations);
        
        return Page();
    }
}