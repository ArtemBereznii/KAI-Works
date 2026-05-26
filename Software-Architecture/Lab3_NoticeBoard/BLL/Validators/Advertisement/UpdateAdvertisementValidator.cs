using FluentValidation;
using BLL.DTOs.Advertisement;

namespace BLL.Validators.Advertisement;

public class UpdateAdvertisementValidator : AbstractValidator<UpdateAdvertisementRequest>
{
    public UpdateAdvertisementValidator()
    {
        RuleFor(a => a.Id)
            .NotEmpty().WithMessage("Advertisement ID cannot be empty.");

        RuleFor(a => a.Title)
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
            .When(a => !string.IsNullOrWhiteSpace(a.Title));

        RuleFor(a => a.Content)
            .MaximumLength(2000).WithMessage("Content must not exceed 2000 characters.")
            .When(a => !string.IsNullOrWhiteSpace(a.Content));
    }
}