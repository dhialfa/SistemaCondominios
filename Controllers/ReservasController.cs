using Microsoft.AspNetCore.Mvc;

namespace SistemaCondominios.Controllers
{
    public class ReservasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
