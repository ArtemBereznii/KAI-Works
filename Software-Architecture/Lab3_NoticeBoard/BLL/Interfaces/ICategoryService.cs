using BLL.DTOs.Category;

namespace BLL.Interfaces;

public interface ICategoryService
{
    Task<Guid> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<CategoryResponse?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<List<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<CategoryResponse>> GetSubcategoriesAsync(Guid parentCategoryId, CancellationToken cancellationToken = default);
}