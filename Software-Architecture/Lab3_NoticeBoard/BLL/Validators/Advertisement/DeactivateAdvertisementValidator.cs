using FluentValidation;
using BLL.DTOs.Advertisement;

namespace BLL.Validators.Advertisement;

public class DeactivateAdvertisementValidator : AbstractValidator<DeactivateAdvertisementRequest>
{
    public DeactivateAdvertisementValidator()
    {
        RuleFor(x => x.AdvertisementId)
            .NotEmpty().WithMessage("Advertisement ID cannot be empty.");

        RuleFor(x => x.RequestingUserId)
            .NotEmpty().WithMessage("Requesting User ID cannot be empty.");
    }
}