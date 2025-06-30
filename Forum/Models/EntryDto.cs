namespace Forum.Models;

public sealed record EntryDto
{
    public required Entry Entry { get; init; }
    public required string Username { get; init; }
}