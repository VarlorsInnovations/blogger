using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Backend.Pages;

[AllowAnonymous]
public class PostModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;
    
    public PostModel(ApplicationDbContext dbContext) => _dbContext = dbContext;
    
    [BindProperty]
    public Post? Post { get; private set; }
    
    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }
        
        // Post = await _dbContext.Posts.FirstOrDefaultAsync(x => x.Id == id);
        Post = new Post();
        Post.Title = "Talking about depressions";
        Post.Tags = new List<Tag>() { new Tag() { Id = 1, Content = "Intro" } };
        Post.Content = "In this post i take you on a journey and share my experience with depression.";
        Post.CreatedAt = DateTime.UtcNow;
        
        if (Post is null) 
        {
            return NotFound();
        }

        return Page();
    }
}