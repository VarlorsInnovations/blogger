using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PostPartController : ControllerBase
{
    private readonly ILogger<PostPartController> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;
    
    private static IEnumerable<string> _allowedFileExtensions = new[] { ".jpg", ".png", ".gif" };

    public PostPartController(
        IWebHostEnvironment environment,
        ILogger<PostPartController> logger, 
        ApplicationDbContext context)
    {
        _logger = logger;
        _dbContext = context;
        _environment = environment;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] PostPartTransferObject data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Something is wrong with provided data!");
        }

        IEnumerable<ContentPart> parts = data.Parts.Select(x => new ContentPart()
        {
            Content = x.Content,
            Type = x.Type,
            Link = x.Link
        });

        List<int> ids = [];
        foreach (ContentPart part in parts)
        {
            var result = await _dbContext.ContentParts.AddAsync(part);
            ids.Add(result.Entity.Id);
        }
        
        await _dbContext.SaveChangesAsync();

        return Ok(ids);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadAsync([FromForm]IFormFile file)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Something is wrong with provided data!");
        }

        if (file.FileName.Count(x => x == '.') != 1)
        {
            return BadRequest("File is not a valid file name!");
        }

        string ext = Path.GetExtension(file.FileName);


        if (!_allowedFileExtensions.Contains(ext))
        {
            return BadRequest("File type is not allowed!");
        }

        string contentRoot = _environment.WebRootPath;
        var path = Path.Combine(contentRoot, "uploads");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        
        string fileName = $"{Guid.NewGuid().ToString()}{ext}";
        string filePath = Path.Combine(path, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        
        return Ok(
            $$"""
            {
                "fileName": "/uploads/{{fileName}}"
            }
            """);
    }
    
    [HttpPost("{id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] PostContentPart data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Something is wrong with the provided data!");
        }

        var part = await _dbContext.ContentParts.FirstOrDefaultAsync(x => x.Id == id);

        if (part is null)
        {
            return NotFound("This content part doesn't exist!");
        }

        part.Content = data.Content.Trim();
        part.Link = data.Link;
        part.Type = data.Type;
        
        _dbContext.ContentParts.Update(part);
        await _dbContext.SaveChangesAsync();

        return Ok("Update content part was successful!");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Something is wrong with the provided data!");
        }
        
        ContentPart? part = await _dbContext.ContentParts.FirstOrDefaultAsync(x => x.Id == id);

        if (part is null)
        {
            return NotFound("This content part doesn't exist!");
        }
        
        _dbContext.ContentParts.Remove(part);
        await _dbContext.SaveChangesAsync();
        
        return Ok("Part deletion was successful!");
    }
}

public sealed class PostPartTransferObject
{
    public List<PostContentPart> Parts { get; set; } = new();
}
