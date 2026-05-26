using Microsoft.EntityFrameworkCore;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Persistence;

namespace DAL.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    public override async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(c => c.ParentCategory)
            .Include(c => c.Subcategories)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetSubcategoriesAsync(Guid parentCategoryId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Where(c => c.ParentCategoryId == parentCategoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasAdvertisementsAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        return await _context.Advertisements.AnyAsync(a => a.CategoryId == categoryId, cancellationToken);
    }
}