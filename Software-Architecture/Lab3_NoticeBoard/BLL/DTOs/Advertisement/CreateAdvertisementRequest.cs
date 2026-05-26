namespace BLL.DTOs.Advertisement;

public record CreateAdvertisementRequest(
    string Title,
    string Content,
    Guid CategoryId,
    Guid UserId,
    List<Guid> TagIds
);