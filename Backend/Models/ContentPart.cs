using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public enum ContentPartType
{
    Paragraph,
    Heading1,
    Heading2,
    Heading3,
    Heading4,
    Image,
    Video,
    Link,
    Code
}

[Table("content_parts")]
public sealed class ContentPart
{
    [Key]
    public int Id { get; set; }

    public ContentPartType Type { get; set; }

    public string? Link { get; set; }

    public string Content { get; set; }
    
}