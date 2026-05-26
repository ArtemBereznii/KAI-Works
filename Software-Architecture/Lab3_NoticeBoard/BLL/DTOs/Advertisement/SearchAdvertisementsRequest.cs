namespace BLL.DTOs.Advertisement;

public record SearchAdvertisementsRequest(
    Guid? CategoryId,
    Guid? UserId,
    string? TagName
);