namespace Forum.Models;

public sealed record Entry
{
    public required string Author { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Title { get; init; }
    public required string Text { get; init; }
    public required string? TopicCreator { get; init; }
    public required string? TopicTitle { get; init; }
}