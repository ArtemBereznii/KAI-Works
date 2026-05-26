namespace BLL.DTOs.Tag;

public record TagResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
}