using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PostsController(ILogger<PostsController> logger, ApplicationDbContext dbContext)
    : ControllerBase
{
    private readonly ILogger<PostsController> _logger = logger;

    [HttpPost]
    public async Task<IActionResult> CreatePostAsync([FromBody] PostTransferObject data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Something is wrong with the provided data!");
        }
        
        Post? duplicate = await dbContext.Posts.FirstOrDefaultAsync(
            p => p.Title.ToLower() == data.Title.ToLower() ||
                 p.UrlIdentifier.ToLower() == data.UrlIdentifier.ToLower());

        if (duplicate is not null)
        {
            return BadRequest("Post with same title or url identifier already exists!");
        }
        
        // bug: user put in tags that do not exist
        // bug: user puts in related posts that do not exist
        
        List<Tag> tags = await dbContext.Tags.Where(x => data.Tags.Contains(x.Content)).ToListAsync();
        
        if (tags.Count != data.Tags.Count)
        {
            return BadRequest("One of the tags doesn't exist!");
        }

        List<Post> related = [];
        if (data.RelatedPosts is not null)
        {
            related = await dbContext.Posts.Where(x => data.RelatedPosts.Contains(x.Id)).ToListAsync();

            if (related.Count != data.RelatedPosts!.Count)
            {
                return BadRequest("One of the related posts doesn't exist!");
            }
        }
        
        Post post = new Post()
        {
            Title = data.Title,
            UrlIdentifier = Uri.EscapeDataString(data.UrlIdentifier),
            Summary = data.Summary,
            CreatedAt = DateTime.UtcNow,
            IsPublished = data.IsPublished,
            Tags = tags,
            Parts = await dbContext.ContentParts.Where(x => data.Content.Contains(x.Id)).ToListAsync(),
            Relations = related
        };

        await dbContext.Posts.AddAsync(post);
        await dbContext.SaveChangesAsync();
        
        return Ok("Post creation was successful!");
    }

    [HttpPost("{postId}")]
    public async Task<IActionResult> UpdateInfoAsync([FromRoute]int id, [FromBody]PostEditObject data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Something is wrong with the provided data!");
        }
        
        Post? post = await dbContext.Posts.Include(
            post => post.Parts)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (post is null)
        {
            return BadRequest($"Post with id {id} does not exist!");
        }

        if (dbContext.Posts
            .Where(x => x.Id != post.Id)
            .Any(x => x.UrlIdentifier.ToLower() == data.UrlIdentifier.ToLower() ||
                      x.Title.ToLower() == data.UrlIdentifier.ToLower()))
        {
            return BadRequest("Post with same title or url identifier already exists!");
        }
        
        post.Title = data.Title;
        post.UrlIdentifier = data.UrlIdentifier;
        post.Summary = data.Summary;
        post.IsPublished = data.IsPublished;
        
        dbContext.Posts.Update(post);
        await dbContext.SaveChangesAsync();
        
        return Ok("Post update was successful!");
    }

    [HttpPost("{postId}/tags")]
    public async Task<IActionResult> UpdateTagsAsync([FromRoute] int postId, [FromBody]PostRelationEdit<string> data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Something is wrong with the provided data!");
        }
        
        var post = await dbContext.Posts.Include(post => post.Tags).FirstOrDefaultAsync(x => x.Id == postId);

        if (post is null)
        {
            return BadRequest($"Post with id {postId} does not exist!");
        }

        List<Tag> added = await dbContext.Tags.Where(x => data.Add.Contains(x.Content)).ToListAsync();
        List<Tag> removed = await dbContext.Tags.Where(x => data.Remove.Contains(x.Content)).ToListAsync();
        
        foreach (var add in added)
        {
            if (!post.Tags.Contains(add))
            {
                post.Tags.Add(add);
            }
        }

        foreach (var remove in removed)
        {
            if (post.Tags.Contains(remove))
            {
                post.Tags.Remove(remove);
            }
        }
        
        dbContext.Posts.Update(post);
        await dbContext.SaveChangesAsync();
        
        return Ok("Update post tags was successful!");
    }

    [HttpPost("{postId}/related")]
    public async Task<IActionResult> UpdateRelatedPostsAsync([FromRoute] int postId,
        [FromBody] PostRelationEdit<int> data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Something is wrong with the provided data!");    
        }
        
        var post = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == postId);

        if (post is null)
        {
            return BadRequest($"Post with id {postId} does not exist!");
        }

        if (data.Add.Contains(post.Id) || data.Remove.Contains(post.Id))
        {
            return BadRequest("Post can't relate to itself!");
        }
        
        var added = await dbContext.Posts.Where(x => data.Add.Contains(x.Id)).ToListAsync();
        var removed = await dbContext.Posts.Where(x => data.Remove.Contains(x.Id)).ToListAsync();

        foreach (var add in added)
        {
            if (!post.Relations.Contains(add))
            {
                post.Relations.Add(add);
            }
        }

        foreach (var remove in removed)
        {
            if (post.Relations.Contains(remove))
            {
                post.Relations.Remove(remove);
            }
        }
        
        dbContext.Posts.Update(post);
        await dbContext.SaveChangesAsync();
        
        return Ok("Update related posts was successful!");
    }

    [HttpPost("{postId}/parts")]
    public async Task<IActionResult> UpdatePostPartsAsync([FromRoute] int postId,
        [FromBody] PostRelationEdit<int> data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Something is wrong with the provided data!");
        }
        
        var post = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == postId);

        if (post is null)
        {
            return BadRequest($"Post with id {postId} does not exist!");
        }
        
        List<ContentPart> removed = await dbContext.ContentParts.Where(x => data.Remove.Contains(x.Id)).ToListAsync();
        
        foreach (ContentPart remove in removed)
        {
            if (post.Parts.Contains(remove))
            {
                post.Parts.Remove(remove);
            }
        }
        
        // todo: linked list to preserve order of content parts 
        // todo: allow add and edit of content parts 
        
        dbContext.Posts.Update(post);
        await dbContext.SaveChangesAsync();
        
        return Ok("Update post parts was successful!");
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePostAsync([FromRoute] int id)
    {
        Post? post = await dbContext.Posts.FirstOrDefaultAsync(x => x.Id == id);
        
        if (post is null)
        {
            return BadRequest($"Post with id {id} does not exist!");
        }

        dbContext.Posts.Remove(post);
        await dbContext.SaveChangesAsync();
        
        return Ok();
    }
}

public sealed class PostRelationEdit<T>
{
    public List<T> Add { get; set; }

    public List<T> Remove { get; set; }
}

public sealed class PostEditObject
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
    public bool IsPublished { get; set; } = false;
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
    public List<int> Content { get; set; }
    
    public List<int>? RelatedPosts { get; set; }

    public bool IsPublished { get; set; } = false;
}

public sealed class PostContentPart
{
    [JsonConverter(typeof(JsonStringEnumConverter<ContentPartType>))]
    public ContentPartType Type { get; set; }

    public string? Link { get; set; }
    
    public string Content { get; set; }
}