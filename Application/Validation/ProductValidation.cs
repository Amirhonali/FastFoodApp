using System;
using Domain.Entities;
using FluentValidation;

namespace Application.Validation
{
	public class ProductValidation : AbstractValidator<Product>
	{
		public ProductValidation()
		{
            RuleFor(prod => prod.Name).NotEmpty()
                .MaximumLength(50)
                .MinimumLength(2)
                .NotNull()
                .Must(name => char.IsUpper(name[0]))
                .WithMessage("Product name not be null or empty!!!");

            RuleFor(prod => prod.Description).NotEmpty()
                .MaximumLength(70)
                .MinimumLength(5)
                .NotNull()
                .WithMessage("Product description not be null or empty!!!");
        }
	}
}

