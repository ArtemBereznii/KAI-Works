namespace BLL.DTOs.Tag;

public record UpdateTagRequest(
    Guid Id,
    string? Name
);