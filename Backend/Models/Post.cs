using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

[Table("posts")]
public sealed class Post
{
    [Key]
    public int Id { get; set; }
    
    public string Title { get; set; }

    public List<PostTag> Tags { get; set; }

    public List<ContentPart> Parts { get; set; }
    
    public List<PostRelation> Relations { get; set; }

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