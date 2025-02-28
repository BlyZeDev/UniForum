namespace Forum.Models;

public sealed record Like
{
    public required string LikeUser { get; init; }
    public required string EntryAuthor { get; init; }
    public required DateTime EntryCreatedAt { get; init; }
}