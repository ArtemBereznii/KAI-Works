using AutoMapper;
using BLL.DTOs.Tag;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services;

public class TagService : BaseService, ITagService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TagService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> CreateAsync(CreateTagRequest request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request);

        var existingTag = await _unitOfWork.Tags.GetByNameAsync(request.Name, cancellationToken);
        if (existingTag != null)
            throw new InvalidOperationException($"Tag '{request.Name}' already exists.");

        var tag = Tag.Create(request.Name);

        await _unitOfWork.Tags.AddAsync(tag, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return tag.Id;
    }

    public async Task UpdateAsync(UpdateTagRequest request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request);

        var tag = await _unitOfWork.Tags.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Tag with ID {request.Id} not found.");

        bool hasChanges = false;

        if (request.Name != null && request.Name != tag.Name)
        {
            var existingTag = await _unitOfWork.Tags.GetByNameAsync(request.Name, cancellationToken);
            if (existingTag != null && existingTag.Id != tag.Id)
                throw new InvalidOperationException($"Tag '{request.Name}' already exists.");

            tag.CorrectName(request.Name);
            hasChanges = true;
        }

        if (!hasChanges) return;

        _unitOfWork.Tags.Update(tag);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid tagId, CancellationToken cancellationToken)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(tagId, cancellationToken)
            ?? throw new KeyNotFoundException($"Tag with ID {tagId} not found.");

        _unitOfWork.Tags.Delete(tag);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<TagResponse?> GetByIdAsync(Guid tagId, CancellationToken cancellationToken)
    {
        var tag = await _unitOfWork.Tags.GetByIdAsync(tagId, cancellationToken)
            ?? throw new KeyNotFoundException($"Tag with ID {tagId} not found.");

        return _mapper.Map<TagResponse>(tag);
    }

    public async Task<List<TagResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var tags = await _unitOfWork.Tags.GetAllAsync(cancellationToken);
        return _mapper.Map<List<TagResponse>>(tags.ToList());
    }
}