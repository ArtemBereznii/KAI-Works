using AutoMapper;
using BLL.DTOs.Category;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services;

public class CategoryService : BaseService, ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request);

        if (request.ParentCategoryId.HasValue)
        {
            var parent = await _unitOfWork.Categories.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken)
                ?? throw new KeyNotFoundException("Parent category not found.");
        }

        var category = Category.Create(request.Name, request.ParentCategoryId);

        await _unitOfWork.Categories.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Id;
    }

    public async Task UpdateAsync(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request);

        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Category with ID {request.Id} not found.");

        bool hasChanges = false;

        if (request.Name != null && request.Name != category.Name)
        {
            category.CorrectName(request.Name);
            hasChanges = true;
        }

        if (request.ParentCategoryId != category.ParentCategoryId)
        {
            if (request.ParentCategoryId.HasValue)
            {
                var parent = await _unitOfWork.Categories.GetByIdAsync(request.ParentCategoryId.Value, cancellationToken)
                    ?? throw new KeyNotFoundException("New parent category not found.");
            }
            category.ChangeParent(request.ParentCategoryId);
            hasChanges = true;
        }

        if (!hasChanges) return;

        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken)
            ?? throw new KeyNotFoundException($"Category with ID {categoryId} not found.");

        bool hasAds = await _unitOfWork.Categories.HasAdvertisementsAsync(categoryId, cancellationToken);
        if (hasAds)
            throw new InvalidOperationException("Cannot delete category with existing advertisements.");

        _unitOfWork.Categories.Delete(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<CategoryResponse?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(categoryId, cancellationToken)
            ?? throw new KeyNotFoundException($"Category with ID {categoryId} not found.");

        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task<List<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);
        return _mapper.Map<List<CategoryResponse>>(categories.ToList());
    }

    public async Task<List<CategoryResponse>> GetSubcategoriesAsync(Guid parentCategoryId, CancellationToken cancellationToken)
    {
        var subcategories = await _unitOfWork.Categories.GetSubcategoriesAsync(parentCategoryId, cancellationToken);
        return _mapper.Map<List<CategoryResponse>>(subcategories.ToList());
    }
}