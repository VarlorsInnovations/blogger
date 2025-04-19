using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Authorize]
[Route("/api/[controller]")]
public class TagController : ControllerBase
{
    private readonly ILogger<TagController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public TagController(ILogger<TagController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTagsAsync([FromBody] TagTransferObject data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Something is wrong with the request data!");
        }
        
        List<string> tagContent = data.Tags.Select(x => x.ToLower()).ToList();
        List<Tag> tags = await _dbContext.Tags.Where(x => tagContent.Contains(x.Content)).ToListAsync();

        if (tags.Count != 0)
        {
            return BadRequest("There are some tags with the same content.");
        }
        
        List<Tag> created = tagContent.Select(x => new Tag() { Content = x }).ToList();
        await _dbContext.Tags.AddRangeAsync(created);
        await _dbContext.SaveChangesAsync();
        
        return Ok("Tag creation was successful!");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTagsAsync([FromRoute] int id)
    {
        var tag = await _dbContext.Tags.FirstOrDefaultAsync(x => x.Id == id);

        if (tag is null)
        {
            return NotFound();
        }
        
        _dbContext.Tags.Remove(tag);
        await _dbContext.SaveChangesAsync();
        
        return Ok("Tag deletion was successful!");
    }
}

public sealed class TagTransferObject
{
    public List<string> Tags { get; set; }
}