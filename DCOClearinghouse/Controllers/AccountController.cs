using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DCOClearinghouse.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace DCOClearinghouse.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login(string ReturnUrl)
        {
            ViewData["returnUrl"] = ReturnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn([FromForm]LogInViewModel loginVM, [FromQuery] string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (LogInUser(loginVM.Username, loginVM.Password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, loginVM.Username),
                        new Claim(ClaimTypes.Role, "Administrator")
                    };


                    // TODO: get roles from the database record

                    var identity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    var props = new AuthenticationProperties {IsPersistent = loginVM.RememberMe};

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Index", "ResourceAdmin");
                    }

                    if (Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewData["message"] = "Invalid Username or password.";
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Resources");
        }

        private bool LogInUser(string username, string password)
        {
            if (username == "admin" && password == "admin")
            {
                return true;
            }

            return false;
        }
    }
}