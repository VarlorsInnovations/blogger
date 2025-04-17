using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

[Table("posts")]
public sealed class Post
{
    [Key]
    public int Id { get; set; }

    public string Title { get; set; }

    public List<Tag> Tags { get; set; }

    public string Content { get; set; }

    public DateTime CreatedAt { get; set; }
}
