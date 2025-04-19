using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

[Table("posts")]
public sealed class Post
{
    [Key]
    public int Id { get; set; }

    public string UrlIdentifier { get; set; }
    
    public string Title { get; set; }
    
    public string Summary { get; set; }

    public List<Tag> Tags { get; set; }

    public List<ContentPart> Parts { get; set; }
    
    public List<Post> Relations { get; set; }

    public bool IsPublished { get; set; } = false;

    public DateTime CreatedAt { get; set; }
}

[Table("post_tags")]
public sealed class PostTag
{
    public int Id { get; set; }
    
    public int PostId { get; set; }
    public int TagId { get; set; }

    public Post Post { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}