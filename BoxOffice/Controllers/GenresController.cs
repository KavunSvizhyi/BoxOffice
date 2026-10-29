using Microsoft.AspNetCore.Mvc;

namespace BoxOffice.Controllers
{
    public class GenresController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
