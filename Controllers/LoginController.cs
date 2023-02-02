using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StepChat.Classes.Auth;
using StepChat.Classes.Configuration;
using StepChat.Contexts;
using StepChat.Models;

namespace StepChat.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfigService? _configService;
        private readonly ITokenService? _tokenService;
        MessengerDataDbContext _context = new();

        public LoginController(ITokenService tokenService, IConfigService configService)
        {
            _tokenService = tokenService;
            _configService = configService;
        }

        // GET: LoginController
        public ActionResult Index()
        {
            return View();
        }

        // GET: LoginController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: LoginController/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogIn(UsersModel users)
        {
            UsersModel? user = await _context!.Users!.FirstOrDefaultAsync(x => x.Email == users.Email && x.Password == users.Password);

            if (user != null)
            {
                var issuer = _configService?.GetValue("Jwt:Issuer");
                var audience = _configService?.GetValue("Jwt:Audience");
                var key = _configService?.GetValue("Jwt:Key");

                string generatedToken = _tokenService!.BuildToken(key, issuer, user);

                if (generatedToken != null)
                {
                    HttpContext.Session.SetString("Token", generatedToken);

                    return RedirectToAction("MainView", "Home");

                }
                else
                {
                    return NotFound();
                }
            }
            return NotFound();
        }

        public ActionResult LoginPage()
        {
            return View();
        }

        // POST: LoginController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LoginController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LoginController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LoginController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LoginController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
