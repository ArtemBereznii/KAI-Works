namespace BLL.DTOs.Advertisement;

public record UpdateAdvertisementRequest(
    Guid Id,
    string? Title,
    string? Content
);