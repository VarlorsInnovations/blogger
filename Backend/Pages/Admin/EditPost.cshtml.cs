using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Backend.Pages.Admin;

public class EditPostModel : PageModel
{
    private readonly ILogger<EditPostModel> _logger;
    private readonly ApplicationDbContext _dbContext;

    public EditPostObject? Post { get; private set; }
    
    public List<PostPreviewModel> RecentPosts { get; private set; }

    public EditPostModel(ILogger<EditPostModel> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        Post? post = await _dbContext.Posts.Include(post => post.Tags).Include(post => post.Parts).FirstOrDefaultAsync(x => x.Id == id);
        RecentPosts = await _dbContext.Posts
            .Where(x => x.IsPublished)
            .Select(x => new PostPreviewModel(
            x.Id, 
            x.Title, 
            x.Summary, 
            x.UrlIdentifier, 
            x.Tags.Select(t => t.Content).ToList(),
            x.CreatedAt)).ToListAsync();
        
        if (post is null)
        {
            return Page();
        }

        Post = new EditPostObject(
            post.Id,
            post.UrlIdentifier,
            post.Title,
            post.Summary,
            post.IsPublished,
            post.Tags,
            post.Parts,
            post.Relations);
        
        return Page();
    }
}

public sealed class EditPostObject
{
    public int Id { get; }
    
    public string UrlIdentifier { get; set; }
    
    public string Title { get; set; }
    
    public string Summary { get; set; }

    public bool IsPublished { get; set; } = false;
    
    public List<Tag> Tags { get; set; }

    public List<ContentPart> Parts { get; set; }
    
    public List<Post> Relations { get; set; }

    public EditPostObject(
        int id,
        string urlIdentifier,
        string title,
        string summary,
        bool isPublished,
        List<Tag> tags,
        List<ContentPart> parts,
        List<Post> relations)
    {
        Id = id;
        UrlIdentifier = urlIdentifier;
        Title = title;
        Summary = summary;
        IsPublished = isPublished;
        Tags = tags;
        Parts = parts;
        Relations = relations;
    }
}