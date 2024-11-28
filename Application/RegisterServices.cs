using System;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using Application.Abstraction;
using Application.Services;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
	public static class RegisterServices
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			services.AddFluentValidation(opt => opt.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped<IEmailService, EmailService>();

            return services;	
		}
	}
}

