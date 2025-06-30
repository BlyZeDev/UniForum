namespace Forum.Models;

public sealed record User
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string Username { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Biography { get; init; }
}