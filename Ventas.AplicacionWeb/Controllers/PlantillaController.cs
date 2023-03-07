using Microsoft.AspNetCore.Mvc;

namespace Ventas.AplicacionWeb.Controllers
{
    public class PlantillaController : Controller
    {
        public IActionResult EnviarClave(string correo, string clave)
        {
            ViewData["correo"] = correo;
            ViewData["clave"] = clave;
            ViewData["url"] = $"{this.Request.Scheme}://{this.Request.Host}";
            return View();
        }

        public IActionResult RestablecerClave(string clave)
        {
            ViewData["clave"] = clave;
            return View();
        }
    }
}
