using CatalogOnline.ContextModels;
using CatalogOnline.Logic;
using CatalogOnline.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CatalogOnline.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly CatalogOnlineContext _context;

        public AuthenticationController(CatalogOnlineContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {

            return View("Login");
        }


        [HttpPost]
        public async Task<IActionResult> Login(UserLogin model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(model.Email))
                    {
                        ModelState.AddModelError(string.Empty, "Câmpul 'Email' este gol");
                    }
                    else if (string.IsNullOrEmpty(model.Password))
                    {
                        ModelState.AddModelError(string.Empty, "Câmpul 'Parola' este gol");
                    }

                    if (_context.Student.Where(user => user.Email.ToLower() == model.Email.ToLower()
                                                && user.Password == model.Password).Any())
                    {
                        List<Claim> claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, model.Email),
                            new Claim("Rol", "Student")
                        };
                        var claimIdentity = new ClaimsIdentity(claims, "AuthenticationCookie");
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));
                        return RedirectToAction("Index", "Student");
                    }
                    else if (_context.Profesor.Where(user => user.Email.ToLower() == model.Email.ToLower()
                                                        && user.Password == model.Password).Any())
                    {
                        List<Claim> claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, model.Email),
                            new Claim("Rol", "Profesor")
                        };
                        var claimIdentity = new ClaimsIdentity(claims, "AuthenticationCookie");
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));
                        return RedirectToAction("Index", "Profesor");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Email sau parolă greșite");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Eroare la login: " + ex.Message);
                }
            }
            return View("Login", model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("Index", "Home");
            }
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
