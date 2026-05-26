using DAL.Entities;

namespace DAL.Interfaces;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<IEnumerable<Category>> GetSubcategoriesAsync(Guid parentCategoryId, CancellationToken cancellationToken = default);
    Task<bool> HasAdvertisementsAsync(Guid categoryId, CancellationToken cancellationToken = default);
}