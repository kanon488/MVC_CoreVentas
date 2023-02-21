using Microsoft.AspNetCore.Mvc;

namespace Ventas.AplicacionWeb.Controllers
{
    public class ProductoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
