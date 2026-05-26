using FluentValidation;
using BLL.DTOs.User;

namespace BLL.Validators.User;

public class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(u => u.Username)
            .NotEmpty().WithMessage("Username cannot be empty.")
            .MaximumLength(100).WithMessage("Username must not exceed 100 characters.");
    }
}