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
        public async Task<IActionResult> LogIn([FromForm]LogInViewModel loginVM, [FromQuery(Name="returnUrl")] string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (LogInUser(loginVM.Username, loginVM.Password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, loginVM.Username),
                        new Claim(ClaimTypes.Role, "site-administrator")
                    };


                    // TODO: get roles from the database record

                    var identity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    var props = new AuthenticationProperties();
                    props.IsPersistent = loginVM.RememberMe;

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
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