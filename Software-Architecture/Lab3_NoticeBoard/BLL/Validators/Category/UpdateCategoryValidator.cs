using FluentValidation;
using BLL.DTOs.Category;

namespace BLL.Validators.Category;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty().WithMessage("Category ID cannot be empty.");

        RuleFor(c => c.Name)
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
            .When(c => !string.IsNullOrWhiteSpace(c.Name));
    }
}