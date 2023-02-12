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

namespace StepChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public IWebHostEnvironment WebHostEnvironment { get; }

        MessengerDataDbContext? _context;
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
        public IActionResult MainView()
        {
                return View();
        }

        public void UploadImage()
        {
            var webRootPath = WebHostEnvironment.WebRootPath + "/images/stepwpicon.jpg";

            ImagesModel images = new() { Image = System.IO.File.ReadAllBytes(webRootPath), ImageId = 1 };
            _context.Images.Add(images);
            _context.SaveChanges();
        }

        [HttpPost("/getPrivateKey")]
        public async Task<string?> GetKey(string? email)
        {
            var user = await _context!.Users.FirstOrDefaultAsync(x => x.Email == email);
            var chat = await _context!.Chats.FirstOrDefaultAsync(x => x.CreateChatUserId == user!.Id);
            var messages = await _context!.Messages.Where(x => x.ChatId == chat.Id).ToListAsync();

            var keys = await _context!.Keys.FirstOrDefaultAsync(x => x.KeyOwnerId == user!.PrivateKeysStorageId);
            
            return keys.Key;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}