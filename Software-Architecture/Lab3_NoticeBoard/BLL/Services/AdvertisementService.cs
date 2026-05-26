using AutoMapper;
using BLL.DTOs.Advertisement;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services;

public class AdvertisementService : BaseService, IAdvertisementService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AdvertisementService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Guid> CreateAsync(CreateAdvertisementRequest request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request);

        var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new KeyNotFoundException($"Category with ID {request.CategoryId} not found.");

        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"User with ID {request.UserId} not found.");

        var advertisement = Advertisement.Create(request.Title, request.Content, category, user);

        if (request.TagIds != null && request.TagIds.Any())
        {
            foreach (var tagId in request.TagIds)
            {
                var tag = await _unitOfWork.Tags.GetByIdAsync(tagId, cancellationToken);
                if (tag != null) advertisement.AddTag(tag);
            }
        }

        await _unitOfWork.Advertisements.AddAsync(advertisement, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return advertisement.Id;
    }

    public async Task UpdateAsync(UpdateAdvertisementRequest request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request);

        var advertisement = await _unitOfWork.Advertisements.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Advertisement with ID {request.Id} not found.");

        bool hasChanges = false;

        if (request.Title != null && request.Title != advertisement.Title)
        {
            advertisement.CorrectTitle(request.Title);
            hasChanges = true;
        }

        if (request.Content != null && request.Content != advertisement.Content)
        {
            advertisement.CorrectContent(request.Content);
            hasChanges = true;
        }

        if (!hasChanges) return;

        _unitOfWork.Advertisements.Update(advertisement);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(DeactivateAdvertisementRequest request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request);

        var advertisement = await _unitOfWork.Advertisements.GetByIdAsync(request.AdvertisementId, cancellationToken)
            ?? throw new KeyNotFoundException($"Advertisement with ID {request.AdvertisementId} not found.");

        advertisement.Deactivate(request.RequestingUserId);

        _unitOfWork.Advertisements.Update(advertisement);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DeleteAdvertisementRequest request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request);

        var advertisement = await _unitOfWork.Advertisements.GetByIdAsync(request.AdvertisementId, cancellationToken)
            ?? throw new KeyNotFoundException($"Advertisement with ID {request.AdvertisementId} not found.");

        if (advertisement.UserId != request.RequestingUserId)
            throw new InvalidOperationException("Only the user who added this advertisement can delete it.");

        _unitOfWork.Advertisements.Delete(advertisement);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<AdvertisementResponse?> GetByIdAsync(Guid advertisementId, CancellationToken cancellationToken)
    {
        var advertisement = await _unitOfWork.Advertisements.GetByIdAsync(advertisementId, cancellationToken)
            ?? throw new KeyNotFoundException($"Advertisement with ID {advertisementId} not found.");

        return _mapper.Map<AdvertisementResponse>(advertisement);
    }

    public async Task<List<AdvertisementResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var advertisements = await _unitOfWork.Advertisements.GetAllAsync(cancellationToken);
        return _mapper.Map<List<AdvertisementResponse>>(advertisements.ToList());
    }

    public async Task<List<AdvertisementResponse>> GetActiveAsync(CancellationToken cancellationToken)
    {
        var advertisements = await _unitOfWork.Advertisements.GetActiveAdvertisementsAsync(cancellationToken);
        return _mapper.Map<List<AdvertisementResponse>>(advertisements.ToList());
    }

    public async Task<List<AdvertisementResponse>> SearchAsync(SearchAdvertisementsRequest request, CancellationToken cancellationToken)
    {
        await ValidateAsync(request);

        var activeAds = await _unitOfWork.Advertisements.GetActiveAdvertisementsAsync(cancellationToken);

        if (request.CategoryId.HasValue)
            activeAds = activeAds.Where(a => a.CategoryId == request.CategoryId.Value);

        if (request.UserId.HasValue)
            activeAds = activeAds.Where(a => a.UserId == request.UserId.Value);

        if (!string.IsNullOrWhiteSpace(request.TagName))
            activeAds = activeAds.Where(a => a.Tags.Any(t => t.Name.Equals(request.TagName, StringComparison.OrdinalIgnoreCase)));

        return _mapper.Map<List<AdvertisementResponse>>(activeAds.ToList());
    }
}