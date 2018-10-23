using Microsoft.AspNetCore.Mvc;

namespace DCOClearinghouse.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        public IActionResult LogIn([Bind("Username", "Password")])
        {
            if (ModelState.IsValid)
            {
                var isValid = ()
            }
        }
    }
}