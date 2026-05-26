namespace DAL.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    ICategoryRepository Categories { get; }
    IAdvertisementRepository Advertisements { get; }
    ITagRepository Tags { get; }
    IUserRepository Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}