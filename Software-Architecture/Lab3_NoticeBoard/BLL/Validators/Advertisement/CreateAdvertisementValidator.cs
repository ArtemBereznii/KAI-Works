using FluentValidation;
using BLL.DTOs.Advertisement;

namespace BLL.Validators.Advertisement;

public class CreateAdvertisementValidator : AbstractValidator<CreateAdvertisementRequest>
{
    public CreateAdvertisementValidator()
    {
        RuleFor(a => a.Title)
            .NotEmpty().WithMessage("Title cannot be empty.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(a => a.Content)
            .NotEmpty().WithMessage("Content cannot be empty.")
            .MaximumLength(2000).WithMessage("Content must not exceed 2000 characters.");

        RuleFor(a => a.CategoryId)
            .NotEmpty().WithMessage("Category ID cannot be empty.");

        RuleFor(a => a.UserId)
            .NotEmpty().WithMessage("User ID cannot be empty.");
    }
}