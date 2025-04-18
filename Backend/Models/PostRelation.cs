namespace Backend.Models;

public class PostRelation
{
    public int Id { get; set; }

    public int OriginId { get; set; }
    public int RelatedId { get; set; }
    
    public Post OriginPost { get; set; } = null!;
    public Post RelatedPost { get; set; } = null!;
}