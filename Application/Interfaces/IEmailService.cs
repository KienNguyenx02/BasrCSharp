using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebApplication1.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message, List<string>? attachmentFilePaths = null);
    }
}