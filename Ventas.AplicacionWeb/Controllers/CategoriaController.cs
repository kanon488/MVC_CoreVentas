using Microsoft.AspNetCore.Mvc;

namespace Ventas.AplicacionWeb.Controllers
{
    public class CategoriaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
