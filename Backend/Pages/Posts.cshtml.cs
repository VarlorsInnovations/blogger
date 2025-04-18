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

    public string Summary { get; }

    public List<string> Tags { get; }
    
    public DateTime CreatedAt { get; }

    public PostViewModel(
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

    // public List<PostViewModel> Items => _dbContext.Posts.Select(
    //     x => new PostViewModel(x.Id, x.Title, x.Content, x.Tags, x.CreatedAt)).ToList(); 
    
    public List<PostViewModel> Items => [
        new PostViewModel(1, "Title 1", "Summary 1", ["Tools", "Healing"], DateTime.UtcNow),
        new PostViewModel(2, "Title 2", "Summary 2", ["Tipps", "Healing"], DateTime.UtcNow)
    ]; 

    public PostsModel(ApplicationDbContext dbContext)
        => _dbContext = dbContext;
    
    public IActionResult OnGet() => Page();
}