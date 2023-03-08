using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using Ventas.AplicacionWeb.Models.ViewModels;
using Ventas.AplicacionWeb.Utilidades.Response;
using Ventas.BLL.Interfaces;
using Ventas.Entity;

namespace Ventas.AplicacionWeb.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioService;
        private readonly IRolService _rolService;

        public UsuarioController(IMapper mapper, IUsuarioService usuarioService, IRolService rolService)
        {   
            _mapper = mapper;
            _usuarioService = usuarioService;
            _rolService = rolService;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaRoles() 
        {
            var lista = await _rolService.Lista();
            List<VMRol> vmListaRoles = _mapper.Map<List<VMRol>>(lista);
            return StatusCode(StatusCodes.Status200OK,vmListaRoles);
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var lista = await _usuarioService.Lista();
            List<VMUsuario> vmUsuarioLista = _mapper.Map<List<VMUsuario>>(lista);
            return StatusCode(StatusCodes.Status200OK, new { data = vmUsuarioLista });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo) 
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();
            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_en_codigo);
                    fotoStream = foto.OpenReadStream();
                }

                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";

                Usuario usuario_creado = await _usuarioService.Crear(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto, urlPlantillaCorreo);


                vmUsuario = _mapper.Map<VMUsuario>(usuario_creado);
                gResponse.estado = true;
                gResponse.objeto = vmUsuario;
            }
            catch (Exception ex)
            {
                gResponse.estado = false;
                gResponse.mensaje = ex.Message; 
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();
            try
            {
                VMUsuario vmUsuario = JsonConvert.DeserializeObject<VMUsuario>(modelo);

                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_en_codigo);
                    fotoStream = foto.OpenReadStream();
                }


                Usuario usuario_editado = await _usuarioService.Editar(_mapper.Map<Usuario>(vmUsuario), fotoStream, nombreFoto);


                vmUsuario = _mapper.Map<VMUsuario>(usuario_editado);
                gResponse.estado = true;
                gResponse.objeto = vmUsuario;
            }
            catch (Exception ex)
            {
                gResponse.estado = false;
                gResponse.mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idUsuario) 
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.estado = await _usuarioService.Eliminar(idUsuario);
            }
            catch (Exception ex)
            {

                gResponse.estado = false;
                gResponse.mensaje = ex.Message; 
            }
            return StatusCode(StatusCodes.Status200OK,gResponse);
        }
    }
}
