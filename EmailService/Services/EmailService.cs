using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using EmailService.Models;
using Microsoft.Extensions.Options;

namespace EmailService.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _settings;

        public EmailService(IOptions<SmtpSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.To))
            {
                Console.WriteLine("[EmailService] ERROR: Mottagaradress saknas.");
                throw new ArgumentException("Mottagaradress krävs.");
            }

            try
            {
                Console.WriteLine($"[EmailService] Skickar e-post till: {message.To}");
                Console.WriteLine($"[EmailService] Ämne: {message.Subject}");
                Console.WriteLine($"[EmailService] Body: {message.Body}");

                var mail = new MailMessage
                {
                    From = new MailAddress(_settings.Username),
                    Subject = message.Subject,
                    Body = message.Body,
                    IsBodyHtml = false
                };

                mail.To.Add(new MailAddress(message.To));

                using var smtp = new SmtpClient(_settings.Host, _settings.Port)
                {
                    Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                    EnableSsl = _settings.EnableSsl
                };

                Console.WriteLine($"[SMTP] Ansluter till {_settings.Host}:{_settings.Port} med SSL: {_settings.EnableSsl}");
                await smtp.SendMailAsync(mail);
                Console.WriteLine("[SMTP] E-post skickad ✅");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[SMTP ERROR] Något gick fel:");
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
