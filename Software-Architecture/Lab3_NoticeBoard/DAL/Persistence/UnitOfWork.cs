using Microsoft.EntityFrameworkCore.Storage;
using DAL.Interfaces;

namespace DAL.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public ICategoryRepository Categories { get; }
    public IAdvertisementRepository Advertisements { get; }
    public ITagRepository Tags { get; }
    public IUserRepository Users { get; }

    public UnitOfWork(
        AppDbContext context,
        ICategoryRepository categoryRepository,
        IAdvertisementRepository advertisementRepository,
        ITagRepository tagRepository,
        IUserRepository userRepository)
    {
        _context = context;
        Categories = categoryRepository;
        Advertisements = advertisementRepository;
        Tags = tagRepository;
        Users = userRepository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var result = await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}