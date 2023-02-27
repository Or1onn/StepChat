using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StepChat.Contexts;
using StepChat.Models;
using System.Diagnostics;
// ReSharper disable All

namespace StepChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly MessengerDataDbContext _context;

        public int MyProperty { get; set; }
        public HomeController(MessengerDataDbContext context)
        {
            _context = context;
        }
        
        public IActionResult MainView(int userId)
        {
            return View(new UsersModel() { Id = userId });
        }

        [HttpPost("/uploadFile")]
        public async Task<int> UploadFile(IFormFile? fileUpload)
        {
            if (fileUpload is { Length: > 0 })
            {
                var fileContent = new byte[fileUpload.Length];
                await using (var stream = fileUpload.OpenReadStream())
                {
                    await stream.ReadAsync(fileContent.AsMemory(0, (int)fileUpload.Length));
                }

                var file = new FilesModel() { File = fileContent, Name = fileUpload.FileName, ContentType = fileUpload.ContentType };
                await _context.Files.AddAsync(file);
                await _context.SaveChangesAsync();

                return file.Id;
            }

            return -1;
        }

        [HttpPost("/donloadFile")]
        public async Task<IActionResult> DownloadFile(int fileId)
        {
            // Получите файл из базы данных по его идентификатору (fileId)
            var file = await _context.Files.FindAsync(fileId);

            if (file == null)
            {
                return NotFound();
            }

            // Создайте поток для чтения данных файла
            var stream = new MemoryStream(file.File);

            // Верните файл клиенту в виде FileStreamResult
            return new FileStreamResult(stream, file.ContentType)
            {
                FileDownloadName = file.Name
            };
        }

        [HttpPost("/getPrivateKey")]
        public async Task<string?> GetKey(int chatId)
        {
            var key = await _context.Keys
                       .Where(x => x.ChatId == chatId)
                       .Select(x => x.Key)
                       .FirstOrDefaultAsync();

            return key;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}