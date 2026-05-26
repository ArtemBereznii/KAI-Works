namespace BLL.DTOs.Advertisement;

public record DeleteAdvertisementRequest(
    Guid AdvertisementId,
    Guid RequestingUserId
);