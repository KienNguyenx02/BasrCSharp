using Microsoft.AspNetCore.Mvc;
using WebApplication1.Application.Interfaces;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IEmailService _emailService;

        public FilesController(IWebHostEnvironment hostingEnvironment, IEmailService emailService)
        {
            _hostingEnvironment = hostingEnvironment;
            _emailService = emailService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsFolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { Message = "File uploaded successfully.", FileName = file.FileName });
        }

        [HttpGet("download/{fileName}")]
        public IActionResult Download(string fileName)
        {
            var uploadsFolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads");
            var filePath = Path.Combine(uploadsFolder, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", fileName);
        }

        [HttpPost("send-email-with-attachment")]
        public async Task<IActionResult> SendEmailWithAttachment([FromForm] string toEmail, [FromForm] string subject, [FromForm] string message, [FromForm] string fileName)
        {
            var uploadsFolder = Path.Combine(_hostingEnvironment.ContentRootPath, "Uploads");
            var filePath = Path.Combine(uploadsFolder, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("Attachment file not found.");

            try
            {
                await _emailService.SendEmailAsync(toEmail, subject, message, new List<string> { filePath });
                return Ok("Email sent successfully with attachment.");
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Error sending email: {ex.Message}");
            }
        }
    }
}
