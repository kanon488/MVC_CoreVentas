using Microsoft.AspNetCore.Mvc;

namespace Ventas.AplicacionWeb.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
