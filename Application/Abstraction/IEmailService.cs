using System;
namespace Application.Abstraction
{
	public interface IEmailService
	{
       public Task SendEmailAsync(string email, string subject, string message);

    }
}

