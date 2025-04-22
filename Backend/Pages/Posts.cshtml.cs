using Backend.Data;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Backend.Pages;

public sealed class PostPreviewModel(
    int id,
    string title,
    string summary,
    string urlIdentifier,
    List<string> tags,
    DateTime createdAt)
{
    public int Id { get; } = id;

    public string Title { get; } = title;

    public string UrlIdentifier { get; } = urlIdentifier;

    public string Summary { get; } = summary;

    public List<string> Tags { get; } = tags;

    public DateTime CreatedAt { get; } = createdAt;
}

[AllowAnonymous]
public class PostsModel(ApplicationDbContext dbContext, VisitService visitService)
    : PageModel
{
    public List<PostPreviewModel> Items { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        await visitService.AddSiteAsync(this.HttpContext);
        
        Items = await dbContext.Posts
            .Where(x => x.IsPublished)
            .Select(
                x => new PostPreviewModel(
                    x.Id, 
                    x.Title,
                    x.Summary,
                    x.UrlIdentifier,
                    x.Tags.Select(t => t.Content).ToList(), x.CreatedAt))
            .ToListAsync();
        
        return Page();
    }
}