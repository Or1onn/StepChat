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
        public IActionResult MainView(int userId)
        {
            return View(new UsersModel() { Id = userId });
        }

        public void UploadImage()
        {
            var webRootPath = WebHostEnvironment.WebRootPath + "/images/stepwpicon.jpg";

            ImagesModel images = new() { Image = System.IO.File.ReadAllBytes(webRootPath), ImageId = 1 };
            _context.Images.Add(images);
            _context.SaveChanges();
        }

        [HttpPost("/getPrivateKey")]
        public async Task<IResult> GetKey(string? email, int chatId)
        {
            var user2 = await _context!.Users
                       .Where(e => e.Email == email)
                       .Select(e => e.Id)
                       .FirstOrDefaultAsync();

            //var _chatId = await _context!.ChatUsers
            //           .Where(e => e.User1 == userId && e.User2 == user2)
            //           .Select(e => e.Id)
            //           .FirstOrDefaultAsync();


            var key = await _context!.Keys
                       .Where(x => x.ChatId == _chatId)
                       .Select(x => x.Key)
                       .FirstOrDefaultAsync();

            var response = new
            {
                chatId = _chatId,
                privateKey = key,
            };

            return Results.Json(response);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}