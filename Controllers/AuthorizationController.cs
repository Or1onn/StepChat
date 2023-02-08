using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Plugins;
using StepChat.Classes.Auth;
using StepChat.Classes.Configuration;
using StepChat.Contexts;
using StepChat.Models;
using StepChat.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace StepChat.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly IConfigService _configService;
        private readonly ITokenService _tokenService;
        MessengerDataDbContext _context;
        EmailSender _sender;

        public AuthorizationController(ITokenService tokenService, IConfigService configService, MessengerDataDbContext context, EmailSender sender)
        {
            _tokenService = tokenService;
            _configService = configService;
            _context = context;
            _sender = sender;
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

        public SymmetricSecurityKey GetSymmetricSecurityKey(string? key)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key!));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogIn(UsersModel loginUser)
        {
            PasswordHasher? hasher = new PasswordHasher();
            UsersModel? user = await _context!.Users!.FirstOrDefaultAsync(x => x.Email == loginUser.Email);

            if (user != null && hasher.VerifyHashedPassword(user.Password, loginUser.Password))
            {
                var issuer = _configService?.GetValue("Jwt:Issuer");
                var audience = _configService?.GetValue("Jwt:Audience");
                var key = _configService?.GetValue("Jwt:Key");

                var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Email!) };
                var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(60)),
                   signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                TempData["Token"] = encodedJwt;
                
                return RedirectToAction("MainView", "Home");
            }
            return NotFound();
        }

        public ActionResult LoginPage()
        {
            return View();
        }
        [HttpGet("/getToken")]
        public string? GetToken()
        {
            return TempData["Token"]?.ToString();
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
                    PasswordHasher? hasher = new PasswordHasher();
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Authorization",
                        values: new
                        {
                            email = user.Email,
                            password = hasher.HashPassword(user.Password),
                            fullname = user.FullName,
                            phoneNumber = user.PhoneNumber,
                            imageId = user.ImageId,
                            role = user.Role,
                            privateKeysStorageId = _context.Users.Count() == 0 ? 0 : _context.Users.Max(x => x.PrivateKeysStorageId + 1)
                        },
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(int id)
        {
            return View(await _context.Users.FindAsync(id));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            UsersModel? user = await _context.Users.FindAsync(id);

            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("MainView", "Home");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string? email, string? password, string fullname, DateTime birthDate, string phoneNumber, int imageId, string role, int privateKeysStorageId)
        {
            UsersModel user = new() { Email = email!, Password = password!, FullName = fullname, PhoneNumber = phoneNumber, ImageId = imageId, Role = role, PrivateKeysStorageId = privateKeysStorageId };
            PrivateKeysStorageModel privateKeysStorageModel = new PrivateKeysStorageModel() { KeysId = user!.PrivateKeysStorageId };

            if (user != null)
            {
                await _context.Users.AddAsync(user);
                await _context.PrivateKeysStorages.AddAsync(privateKeysStorageModel);

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