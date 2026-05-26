using DAL.Entities;

namespace DAL.Interfaces;

public interface IAdvertisementRepository : IBaseRepository<Advertisement>
{
    Task<IEnumerable<Advertisement>> GetAdvertisementsByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Advertisement>> GetAdvertisementsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Advertisement>> GetActiveAdvertisementsAsync(CancellationToken cancellationToken = default);
}