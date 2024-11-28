using System;
using Domain.Entities;
using FluentValidation;

namespace Application.Validation
{
	public class PermissionValidation : AbstractValidator<Permission>
    {
		public PermissionValidation()
		{
            RuleFor(prod => prod.PermissionName).NotEmpty()
                .MaximumLength(50)
                .MinimumLength(2)
                .NotNull()
                .Must(name => char.IsUpper(name[0]))
                .WithMessage("Permission name not be null or empty!!!");
        }
	}
}

