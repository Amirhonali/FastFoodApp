using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Application.Abstraction;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress(
            _configuration["Smtp:SenderName"],
            _configuration["Smtp:SenderEmail"]
        ));
        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;

        emailMessage.Body = new TextPart("plain")
        {
            Text = message
        };

        using (var client = new SmtpClient())
        {
            try
            {
                await client.ConnectAsync(_configuration["Smtp:Server"], int.Parse(_configuration["Smtp:Port"]), SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(_configuration["Smtp:SenderEmail"], _configuration["Smtp:SenderPassword"]);

                await client.SendAsync(emailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw; 
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}