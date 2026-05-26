using Microsoft.EntityFrameworkCore;
using DAL.Entities;
using DAL.Enums;
using DAL.Interfaces;
using DAL.Persistence;

namespace DAL.Repositories;

public class AdvertisementRepository : BaseRepository<Advertisement>, IAdvertisementRepository
{
    public AdvertisementRepository(AppDbContext context) : base(context) { }

    public override async Task<Advertisement?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(a => a.Category)
            .Include(a => a.User)
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Advertisement>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(a => a.Category)
            .Include(a => a.User)
            .Include(a => a.Tags)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Advertisement>> GetAdvertisementsByCategoryAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(a => a.Category)
            .Include(a => a.User)
            .Include(a => a.Tags)
            .Where(a => a.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Advertisement>> GetAdvertisementsByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(a => a.Category)
            .Include(a => a.User)
            .Include(a => a.Tags)
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Advertisement>> GetActiveAdvertisementsAsync(CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(a => a.Category)
            .Include(a => a.User)
            .Include(a => a.Tags)
            .Where(a => a.Status == AdStatus.Active)
            .ToListAsync(cancellationToken);
    }
}