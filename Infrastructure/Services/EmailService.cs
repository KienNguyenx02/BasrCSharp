using System.Net.Mail;
using System.Net;
using WebApplication1.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebApplication1.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message, List<string>? attachmentFilePaths = null)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var host = smtpSettings["Host"];
            var port = int.Parse(smtpSettings["Port"]);
            var userName = smtpSettings["UserName"];
            var password = smtpSettings["Password"];
            var enableSsl = bool.Parse(smtpSettings["EnableSsl"]);

            var fromAddress = new MailAddress(userName, "Your App Name");
            var toAddress = new MailAddress(toEmail);

            var smtp = new SmtpClient
            {
                Host = host,
                Port = port,
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(userName, password)
            };

            using (var mailMessage = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            })
            {
                if (attachmentFilePaths != null && attachmentFilePaths.Any())
                {
                    foreach (var filePath in attachmentFilePaths)
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            mailMessage.Attachments.Add(new Attachment(filePath));
                        }
                    }
                }
                await smtp.SendMailAsync(mailMessage);
            }
        }
    }
}
