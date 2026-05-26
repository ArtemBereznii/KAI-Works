using FluentValidation;
using BLL.DTOs.Tag;

namespace BLL.Validators.Tag;

public class UpdateTagValidator : AbstractValidator<UpdateTagRequest>
{
    public UpdateTagValidator()
    {
        RuleFor(t => t.Id)
            .NotEmpty().WithMessage("Tag ID cannot be empty.");

        RuleFor(t => t.Name)
            .MaximumLength(50).WithMessage("Tag name must not exceed 50 characters.")
            .When(t => !string.IsNullOrWhiteSpace(t.Name));
    }
}