using FluentValidation;
using BLL.DTOs.Advertisement;

namespace BLL.Validators.Advertisement;

public class SearchAdvertisementsValidator : AbstractValidator<SearchAdvertisementsRequest>
{
    public SearchAdvertisementsValidator()
    {
        RuleFor(x => x)
            .Must(x => x.CategoryId.HasValue || x.UserId.HasValue || !string.IsNullOrWhiteSpace(x.TagName))
            .WithMessage("At least one search parameter (Category, User, or Tag) must be provided.");
    }
}