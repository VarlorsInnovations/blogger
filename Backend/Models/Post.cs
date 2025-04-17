namespace Backend.Models;

public sealed class Post
{
    public int Id { get; }

    public string Title { get; }

    public List<Tag> Tags { get; }

    public string Content { get; }

    public DateTime CreatedAt { get; }
}
