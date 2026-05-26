namespace BLL.DTOs.Advertisement;

public record AdvertisementResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string Content { get; init; } = null!;
    public string Status { get; init; } = null!;
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = null!;
    public Guid UserId { get; init; }
    public string Username { get; init; } = null!;
    public List<string> Tags { get; init; } = new();
}