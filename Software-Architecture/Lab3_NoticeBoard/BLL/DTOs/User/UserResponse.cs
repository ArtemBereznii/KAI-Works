namespace BLL.DTOs.User;

public record UserResponse
{
    public Guid Id { get; init; }
    public string Username { get; init; } = null!;
}