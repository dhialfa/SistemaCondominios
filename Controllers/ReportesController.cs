using Microsoft.AspNetCore.Mvc;

namespace SistemaCondominios.Controllers
{
    public class ReportesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
