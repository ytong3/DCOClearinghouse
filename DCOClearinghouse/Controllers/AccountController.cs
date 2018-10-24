using System.Threading.Tasks;
using DCOClearinghouse.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DCOClearinghouse.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogIn([Bind("Username", "Password")] LogInViewModel loginVM )
        {
            if (ModelState.IsValid)
            {
            }
            return View();
        }
    }
}