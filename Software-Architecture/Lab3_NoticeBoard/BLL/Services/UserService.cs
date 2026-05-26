using AutoMapper;
using BLL.DTOs.User;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services;

public class UserService : BaseService, IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request);

        var existingUser = await _unitOfWork.Users.GetByUsernameAsync(request.Username, cancellationToken);
        if (existingUser != null)
            throw new InvalidOperationException($"Username '{request.Username}' is already taken.");

        var user = User.Create(request.Username);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

    public async Task UpdateAsync(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request);

        var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"User with ID {request.Id} not found.");

        bool hasChanges = false;

        if (request.Username != null && request.Username != user.Username)
        {
            var existingUser = await _unitOfWork.Users.GetByUsernameAsync(request.Username, cancellationToken);
            if (existingUser != null && existingUser.Id != user.Id)
                throw new InvalidOperationException($"Username '{request.Username}' is already taken.");

            user.CorrectUsername(request.Username);
            hasChanges = true;
        }

        if (!hasChanges) return;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

        _unitOfWork.Users.Delete(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserResponse?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException($"User with ID {userId} not found.");

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<List<UserResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        return _mapper.Map<List<UserResponse>>(users.ToList());
    }
}