using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using StepChat.Classes.Auth;
using StepChat.Classes.Configuration;
using StepChat.Contexts;
using StepChat.Models;
using StepChat.Services;
using System;
using System.Linq;
using System.Text;

namespace StepChat.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly IConfigService? _configService;
        private readonly ITokenService? _tokenService;
        MessengerDataDbContext _context = new();
        EmailSender _sender = new();
        UserManager<UsersModel> _userManager;

        public AuthorizationController(ITokenService tokenService, IConfigService configService)
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

        public ActionResult RegistrationPage()
        {
            return View();
        }

        // POST: LoginController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UsersModel user)
        {
            try
            {
                if (user != null)
                {
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Authorization",
                        values: new { email = user.Email, password = user.Password, fullname = user.FullName, birthDate = user.BirthDate, phoneNumber = user.PhoneNumber, imageId = user.ImageId, role = user.Role },
                        protocol: HttpContext.Request.Scheme);

                    EmailSender _emailSender = new();
                    await _emailSender.SendEmailAsync(user.Email, $"<a href='{callbackUrl}'>Verify<a>");

                    return RedirectToAction("EmailConfirmationPage", "Authorization");
                }
            }
            catch
            {
                return View();
            }

            return View("LoginPage");
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public List<UsersModel> GetAll()
        {
            return _context.Users.ToList<UsersModel>();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string? email, string? password, string fullname, DateTime birthDate, string phoneNumber, int imageId, string role)
        {
            UsersModel user = new() { Email = email, Password = password, FullName = fullname, BirthDate = birthDate, PhoneNumber = phoneNumber, ImageId = imageId, Role = role };
            
            if (user != null)
            {
                await _context.AddAsync(user);
                await _context.SaveChangesAsync();
                return View("LoginPage");
            }
            else
            {
                return View(null);
            }
        }


        public ActionResult EmailConfirmationPage(int id)
        {
            return View();
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
