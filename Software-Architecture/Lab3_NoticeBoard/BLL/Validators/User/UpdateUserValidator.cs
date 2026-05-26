using FluentValidation;
using BLL.DTOs.User;

namespace BLL.Validators.User;

public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserValidator()
    {
        RuleFor(u => u.Id)
            .NotEmpty().WithMessage("User ID cannot be empty.");

        RuleFor(u => u.Username)
            .MaximumLength(100).WithMessage("Username must not exceed 100 characters.")
            .When(u => !string.IsNullOrWhiteSpace(u.Username));
    }
}