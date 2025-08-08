using Microsoft.AspNetCore.Mvc;

namespace SistemaCondominios.Controllers
{
    public class MainMenuController : Controller
    {
        public IActionResult Index()
        {
            var user = HttpContext.User;

            var roles = new
            {
                IsAdmin = user.IsInRole("SuperAdministrador"),
                IsAdminCondominal = user.IsInRole("Administrador"),
                IsGuarda = user.IsInRole("Guarda"),
                IsCondominal = user.IsInRole("Propietario")
            };

            ViewBag.Roles = roles;

            return View();
        }
    }

}
