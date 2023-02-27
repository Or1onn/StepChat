using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StepChat.Classes.Configuration;
using StepChat.Contexts;
using StepChat.Models;
using StepChat.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StepChat.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly IConfigService _configService;
        private readonly MessengerDataDbContext _context;
        private readonly EmailSender _emailSender;

        public AuthorizationController(IConfigService configService, MessengerDataDbContext context, EmailSender sender)
        {
            _configService = configService;
            _context = context;
            _emailSender = sender;
        }

        public SymmetricSecurityKey GetSymmetricSecurityKey(string? key)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key!));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogIn(UsersModel loginUser)
        {
            PasswordHasher hasher = new PasswordHasher();
            UsersModel? user = await _context.Users.FirstOrDefaultAsync(x => x.Email == loginUser.Email);

            if (user != null && hasher.VerifyHashedPassword(user.Password, loginUser.Password))
            {
                var issuer = _configService.GetValue("Jwt:Issuer");
                var audience = _configService.GetValue("Jwt:Audience");
                var key = _configService.GetValue("Jwt:Key");

                var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Email) };
                var jwt = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(60)),
                    signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256));
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                TempData["Token"] = encodedJwt;

                return RedirectToAction("MainView", "Home", new { userId = user.Id });
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
        public async Task<ActionResult> Create(UsersModel? user, string? name, string? surname, string? patronymic)
        {
            try
            {
                if (user != null)
                {
                    user.FullName = name + ' ' + surname + ' ' + patronymic;
                    PasswordHasher hasher = new PasswordHasher();
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
                            role = user.Role
                        },
                        protocol: HttpContext.Request.Scheme);

                    await _emailSender.SendEmailAsync(user.Email + "@itstep.edu.az",
                        $"<a href='{callbackUrl}'>Verify<a>");

                    return RedirectToAction("EmailConfirmationPage", "Authorization");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return View("LoginPage");
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<List<UsersModel>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
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
        public async Task<ActionResult> ConfirmEmail(string email, string password, string fullname, string phoneNumber,
            int imageId, string role)
        {
            UsersModel user = new()
            {
                Email = email, Password = password, FullName = fullname, PhoneNumber = phoneNumber, ImageId = imageId,
                Role = role,
                PrivateKeysStorageId = !_context.Users.Any() ? 0 : _context.Users.Max(x => x.PrivateKeysStorageId + 1)
            };

            PrivateKeysStorageModel privateKeysStorageModel = new PrivateKeysStorageModel()
                { KeysId = user.PrivateKeysStorageId };

            await _context.Users.AddAsync(user);
            await _context.PrivateKeysStorages.AddAsync(privateKeysStorageModel);

            await _context.SaveChangesAsync();

            return RedirectToAction("LoginPage", "Authorization");
        }

        public ActionResult EmailConfirmationPage()
        {
            return View();
        }
    }
}