using Microsoft.AspNetCore.Mvc;

namespace SistemaCondominios.Controllers
{
    public class MainMenuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
