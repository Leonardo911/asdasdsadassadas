using Microsoft.AspNetCore.Mvc;

namespace NavojoaDigitalFrontEnd.Controllers
{
    public class EgresosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Partidas()
        {
            return View("Partidas");
        }
    }
}
