using Microsoft.AspNetCore.Mvc;

namespace UserService.API.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
