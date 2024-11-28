using System;
using Domain.Entities;
using FluentValidation;
namespace Application.Validation
{
	public class RestaurantValidation : AbstractValidator<Restaurant>
	{
		public RestaurantValidation()
		{
			RuleFor(rest => rest.Name).NotEmpty()
				.MaximumLength(50)
				.MinimumLength(2)
				.NotNull()
                .Must(name => char.IsUpper(name[0]))
                .WithMessage("Restaurant name not be null or empty!!!");

			RuleFor(rest => rest.Description).NotEmpty()
				.MaximumLength(500)
				.MinimumLength(10)
				.NotNull()
				.WithMessage("Restaurant description not be null or empty!!!");
		}
	}
}

