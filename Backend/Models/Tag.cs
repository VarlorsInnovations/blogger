using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

[Table("tags")]
public sealed class Tag
{
    [Key]
    public int Id { get; set; }

    public string Content { get; set; }

    public List<Post> Posts { get; set; }
}
