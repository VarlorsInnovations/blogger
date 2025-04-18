using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public sealed class Tag
{
    [Key]
    public int Id { get; set; }

    public string Content { get; set; }
    
}
