using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using StepChat.Contexts;
using StepChat.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Drawing;
using System.Text.Json;
using System.Text;
using StepChat.Hubs;

namespace StepChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MessengerDataDbContext _context;
        private IWebHostEnvironment WebHostEnvironment { get; }


        public int MyProperty { get; set; }
        public HomeController(ILogger<HomeController> logger, MessengerDataDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            WebHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult MainView(int userId)
        {
            return View(new UsersModel() { Id = userId });
        }

        [HttpPost("/uploadFile")]
        public async Task UploadImage(IFormFile fileUpload)
        {
            if (fileUpload != null && fileUpload.Length > 0)
            {
                var fileContent = new byte[fileUpload.Length];
                using (var stream = fileUpload.OpenReadStream())
                {
                    await stream.ReadAsync(fileContent, 0, (int)fileUpload.Length);
                }

                await _context.Files.AddAsync(new FilesModel() { File = fileContent, Name = fileUpload.FileName });
                await _context.SaveChangesAsync();
            }
        }

        public void UploadFile()
        {
            var webRootPath = WebHostEnvironment.WebRootPath + "/images/stepwpicon.jpg";

            ImagesModel images = new() { Image = System.IO.File.ReadAllBytes(webRootPath), ImageId = 1 };
            _context.Images.Add(images);
            _context.SaveChanges();
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