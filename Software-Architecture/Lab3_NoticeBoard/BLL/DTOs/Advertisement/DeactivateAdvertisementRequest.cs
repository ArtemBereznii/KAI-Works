namespace BLL.DTOs.Advertisement;

public record DeactivateAdvertisementRequest(
    Guid AdvertisementId,
    Guid RequestingUserId
);