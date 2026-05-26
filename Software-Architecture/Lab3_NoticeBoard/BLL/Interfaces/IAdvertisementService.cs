using BLL.DTOs.Advertisement;

namespace BLL.Interfaces;

public interface IAdvertisementService
{
    Task<Guid> CreateAsync(CreateAdvertisementRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateAdvertisementRequest request, CancellationToken cancellationToken = default);
    Task DeactivateAsync(DeactivateAdvertisementRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(DeleteAdvertisementRequest request, CancellationToken cancellationToken = default);
    Task<AdvertisementResponse?> GetByIdAsync(Guid advertisementId, CancellationToken cancellationToken = default);
    Task<List<AdvertisementResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<AdvertisementResponse>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<List<AdvertisementResponse>> SearchAsync(SearchAdvertisementsRequest request, CancellationToken cancellationToken = default);
}