using FluentValidation;
using BLL.DTOs.Category;

namespace BLL.Validators.Category;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name cannot be empty.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
    }
}