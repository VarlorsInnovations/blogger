using System.ComponentModel.DataAnnotations;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly ILogger<PostsController> _logger;
    private readonly ApplicationDbContext _dbContext;
    
    public PostsController(ILogger<PostsController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePostAsync([FromBody] PostTransferObject data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Something is wrong with the provided data!");
        }
        
        var duplicate = await _dbContext.Posts.FirstOrDefaultAsync(
            p => p.Title.ToLower() == data.Title.ToLower() ||
                 p.UrlIdentifier.ToLower() == data.UrlIdentifier.ToLower());

        if (duplicate is not null)
        {
            return BadRequest("Post with same title or url identifier already exists!");
        }
        
        // bug: user put in tags that do not exist
        // bug: user puts in related posts that do not exist
        
        List<Tag> tags = await _dbContext.Tags.Where(x => data.Tags.Contains(x.Content)).ToListAsync();
        List<Post> related = await _dbContext.Posts.Where(x => data.RelatedPosts.Contains(x.Id)).ToListAsync();
        
        if (tags.Count != data.Tags.Count)
        {
            return BadRequest("One of the tags doesn't exist!");
        }

        if (related.Count != data.RelatedPosts.Count)
        {
            return BadRequest("One of the related posts doesn't exist!");
        }

        Post post = new Post()
        {
            Title = data.Title,
            UrlIdentifier = Uri.EscapeDataString(data.UrlIdentifier),
            Summary = data.Summary,
            CreatedAt = DateTime.UtcNow,
            IsPublished = data.IsPublished,
            Tags = tags,
            Parts = data.Content.Select(x => new ContentPart { Content = x.Content, Type = x.Type, Link = x.Link })
                .ToList(),
            Relations = related
        };

        await _dbContext.Posts.AddAsync(post);
        await _dbContext.SaveChangesAsync();
        
        return Ok("Post creation was successful!");
    }

    [HttpPost("{postId}")]
    public async Task<IActionResult> UpdatePostAsync([FromRoute]int id, [FromBody] PostTransferObject data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Something is wrong with the provided data!");
        }

        Post? post = await _dbContext.Posts.Include(post => post.Parts).FirstOrDefaultAsync(x => x.Id == id);
        if (post is null)
        {
            return BadRequest($"Post with id {id} does not exist!");
        }

        if (_dbContext.Posts
            .Where(x => x.Id == post.Id)
            .Any(x => x.UrlIdentifier.ToLower() == data.UrlIdentifier.ToLower() ||
                      x.Title.ToLower() == data.UrlIdentifier.ToLower()))
        {
            return BadRequest("Post with same title or url identifier already exists!");
        }
        
        List<Tag> tags = await _dbContext.Tags.Where(x => data.Tags.Contains(x.Content)).ToListAsync();
        List<Post> related = await _dbContext.Posts.Where(x => data.RelatedPosts.Contains(x.Id)).ToListAsync();
        
        if (tags.Count != data.Tags.Count)
        {
            return BadRequest("One of the tags doesn't exist!");
        }

        if (related.Count != data.RelatedPosts.Count)
        {
            return BadRequest("One of the related posts doesn't exist!");
        }
        
        // todo: how to make uploads for images, etc 
        
        foreach (PostContentParts part in data.Content)
        {
            ContentPart? p = post.Parts.FirstOrDefault(x => x.Id == part.Id);

            if (p is null)
            {   
                ContentPart created = new ContentPart { Content = part.Content, Type = part.Type, Link = part.Link };
                post.Parts.Add(created);
            }
            else
            {
                p.Content = part.Content;
                p.Type = part.Type;
                p.Link = part.Link;
            }
        }
        
        post.Title = data.Title;
        post.UrlIdentifier = data.UrlIdentifier;
        post.Summary = data.Summary;
        post.IsPublished = data.IsPublished;
        post.Tags = tags;
        post.Relations = related;

        _dbContext.Posts.Update(post);
        await _dbContext.SaveChangesAsync();
        
        return Ok("Post update was successful!");
    }

    [HttpDelete("{postId}")]
    public async Task<IActionResult> DeletePostAsync([FromRoute] int id)
    {
        Post? post = await _dbContext.Posts.FirstOrDefaultAsync(x => x.Id == id);
        
        if (post is null)
        {
            return BadRequest($"Post with id {id} does not exist!");
        }

        _dbContext.Posts.Remove(post);
        await _dbContext.SaveChangesAsync();
        
        return Ok();
    }
}

public sealed class PostTransferObject
{
    [Required]
    [MinLength(10)]
    public string Title { get; set; }

    [Required]
    [MinLength(5)]
    public string UrlIdentifier { get; set; }

    [Required]
    [MinLength(20)]
    public string Summary { get; set; }

    [Required]
    public List<string> Tags { get; set; }

    [Required]
    [MinLength(1)]
    public List<PostContentParts> Content { get; set; }
    
    public List<int> RelatedPosts { get; set; }

    public bool IsPublished { get; set; } = false;
}

public sealed class PostContentParts
{
    public int Id { get; set; }
    
    public ContentPartType Type { get; set; }

    public string? Link { get; set; }
    
    public string Content { get; set; }
}