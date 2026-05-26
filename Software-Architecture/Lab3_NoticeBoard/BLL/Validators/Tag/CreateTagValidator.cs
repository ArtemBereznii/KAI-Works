using FluentValidation;
using BLL.DTOs.Tag;

namespace BLL.Validators.Tag;

public class CreateTagValidator : AbstractValidator<CreateTagRequest>
{
    public CreateTagValidator()
    {
        RuleFor(t => t.Name)
            .NotEmpty().WithMessage("Tag name cannot be empty.")
            .MaximumLength(50).WithMessage("Tag name must not exceed 50 characters.");
    }
}