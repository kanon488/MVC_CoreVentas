using Microsoft.AspNetCore.Mvc;

namespace Ventas.AplicacionWeb.Controllers
{
    public class ReporteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
