namespace Forum.Models;

public sealed record Topic
{
    public required string Creator { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Title { get; init; }
}