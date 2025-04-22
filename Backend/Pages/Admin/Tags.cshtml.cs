using Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Backend.Pages.Admin;

public class TagsModel(ILogger<TagsModel> logger, ApplicationDbContext dbContext) : PageModel
{
    public List<TagsTransferObject> Tags { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        Tags = await dbContext.Tags.Select(x => new TagsTransferObject(x.Id, x.Content)).ToListAsync();
        return Page();
    }
}

public sealed class TagsTransferObject
{
    public int Id { get; }
    
    public string Content { get; }

    public TagsTransferObject(int id, string content)
    {
        Id = id;
        Content = content;
    }
}