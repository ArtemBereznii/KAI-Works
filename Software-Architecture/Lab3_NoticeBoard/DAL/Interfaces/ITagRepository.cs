using DAL.Entities;

namespace DAL.Interfaces;

public interface ITagRepository : IBaseRepository<Tag>
{
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}