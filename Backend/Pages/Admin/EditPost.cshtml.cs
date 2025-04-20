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

    public EditPostModel(ILogger<EditPostModel> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        Post? post = await _dbContext.Posts.FirstOrDefaultAsync(x => x.Id == id);

        if (post is null)
        {
            return Page();
        }

        Post = new EditPostObject();
        return Page();
    }
}

public sealed class EditPostObject
{
    public int Id { get; set; }
    
    
}