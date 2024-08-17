using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sertif.Data;
using sertif.Models;
using sertif.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace sertif.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // login
        [HttpGet]
        public IActionResult Login()
        {
            ViewData["Layout"] = "_AuthLayout";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username);

                if (user != null)
                {
                    bool isPasswordValid = VerifyPassword(model.Password, user.Password);

                    if (isPasswordValid)
                    {
                        // Jika password valid tapi belum di-hash, hash dan update
                        if (!user.Password.StartsWith("$"))
                        {
                            user.Password = _passwordHasher.HashPassword(user, model.Password);
                            await _context.SaveChangesAsync();
                        }

                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("UserId", user.UserId.ToString())
                };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Incorrect Username or Password.");
            }

            return View(model);
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(new User(), storedPassword, inputPassword);
            return result == PasswordVerificationResult.Success;
        }


        //logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Menghapus cookie autentikasi
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect ke halaman home atau halaman login
            return RedirectToAction("Index", "Home");
        }

        //register
        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Layout"] = "_AuthLayout";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Username = model.Username,
                    FullName = model.FullName,
                    Role = model.Role
                };

                // Hash password sebelum disimpan
                user.Password = _passwordHasher.HashPassword(user, model.Password);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Tambahkan kode ini
                if (user.Role == UserRole.Student)
                {
                    var student = new Student { UserId = user.UserId };
                    _context.Students.Add(student);
                }
                else if (user.Role == UserRole.Laboran)
                {
                    var laboran = new Laboran { UserId = user.UserId };
                    _context.Laborans.Add(laboran);
                }
                await _context.SaveChangesAsync();

                // Otomatis login setelah registrasi
                await LoginUser(user);

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        private async Task LoginUser(User user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role.ToString()),
        new Claim("UserId", user.UserId.ToString())
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}