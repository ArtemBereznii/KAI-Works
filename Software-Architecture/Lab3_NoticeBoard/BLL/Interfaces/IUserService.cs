using BLL.DTOs.User;

namespace BLL.Interfaces;

public interface IUserService
{
    Task<Guid> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserResponse?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}