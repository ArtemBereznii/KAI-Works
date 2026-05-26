using BLL.DTOs.Tag;

namespace BLL.Interfaces;

public interface ITagService
{
    Task<Guid> CreateAsync(CreateTagRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateTagRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid tagId, CancellationToken cancellationToken = default);
    Task<TagResponse?> GetByIdAsync(Guid tagId, CancellationToken cancellationToken = default);
    Task<List<TagResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}