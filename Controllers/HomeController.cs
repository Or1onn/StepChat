using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StepChat.Contexts;
using StepChat.Models;
using System.Diagnostics;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace StepChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly MessengerDataDbContext _context;
        private IWebHostEnvironment WebHostEnvironment { get; }

        public HomeController(MessengerDataDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            WebHostEnvironment = webHostEnvironment;
        }
        
        
        // public void UploadImage()
        // {
        //     var webRootPath = WebHostEnvironment.WebRootPath + "/images/stepwpicon.jpg";
        //
        //     ImagesModel images = new() { Image = System.IO.File.ReadAllBytes(webRootPath), ImageId = 1 };
        //     _context!.Images.Add(images);
        //     _context!.SaveChanges();
        // }
        
        public IActionResult MainView(int userId)
        {
            return View(new UsersModel() { Id = userId });
        }

        [HttpPost("/uploadFile")]
        public async Task<int> UploadFile(IFormFile? fileUpload, int chatId, int id)
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
                
                var message = new MessagesModel() { Id = file.Id, ChatId = chatId, Text = "type=file", CreateTime = DateTime.Now, UserId = id};
                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();
                
                return file.Id;
            }

            return -1;
        }

        [HttpPost("/donloadFile")]
        public async Task<IActionResult> DownloadFile(int fileId)
        {
            var file = await _context.Files.FindAsync(fileId);

            if (file == null)
            {
                return NotFound();
            }

            var stream = new MemoryStream(file.File);

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
                       .AsNoTracking()
                       .Select(x => x.Key )
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