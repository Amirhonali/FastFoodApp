
using FluentValidation;
using Domain.Entities;

namespace Application.Validation
{
	public class RoleValidation : AbstractValidator<Role>
	{
        public RoleValidation()
        {
            RuleFor(prod => prod.Name).NotEmpty()
                    .MaximumLength(50)
                    .MinimumLength(2)
                    .NotNull()
                    .Must(name => char.IsUpper(name[0]))
                    .WithMessage("Permission name not be null or empty!!!");
        }
    }
}

