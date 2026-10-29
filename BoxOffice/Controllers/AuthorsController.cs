using Microsoft.AspNetCore.Mvc;

namespace BoxOffice.Controllers
{
    public class AuthorsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
