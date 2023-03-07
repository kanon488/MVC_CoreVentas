using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ventas.BLL.Interfaces;
using Ventas.DAL.Interfaces;
using Ventas.Entity;

namespace Ventas.BLL.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _repositorio;
        private readonly IFirebaseService _firebaseService;
        private readonly IUtilidadesService _utilidadesService;
        private readonly ICorreoService _correoService;

        public UsuarioService(
            IGenericRepository<Usuario> repositorio,
            IFirebaseService firebaseService,
            IUtilidadesService utilidadesService,
            ICorreoService correoService)
        {
            _repositorio = repositorio;
            _firebaseService = firebaseService;
            _utilidadesService = utilidadesService;
            _correoService = correoService;
        }

        public async Task<List<Usuario>> Lista()
        {
            IQueryable<Usuario> query = await _repositorio.Consultar();
            return query.Include(r => r.IdRolNavigation).ToList();
        }

        public async Task<Usuario> Crear(Usuario entidad, Stream Foto = null, string nombreFoto = "", string urlPlantillaCorreo = "")
        {
            Usuario usuario_existe = await _repositorio.Obtener(u => u.Correo == entidad.Correo);

            if (usuario_existe != null)
                throw new TaskCanceledException("El correo ya existe.");

            try
            {
                string clave_generada = _utilidadesService.GenerarClave();
                entidad.Clave = _utilidadesService.ConvertirSha256(clave_generada);
                entidad.NombreFoto = nombreFoto;
                if (Foto != null)
                {
                    string urlFoto = await _firebaseService.SubirStorage(Foto, "carpeta_usuario", nombreFoto);
                    entidad.UrlFoto = urlFoto;
                }
                Usuario usuario_creado = await _repositorio.Crear(entidad);

                if (usuario_creado.IdUsuario == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el usuario");
                }
                if (urlPlantillaCorreo != "")
                {
                    urlPlantillaCorreo = urlPlantillaCorreo.Replace("[correo]", usuario_creado.Correo).Replace("[clave]", clave_generada);
                    string htmlCorreo = "";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader reader = null;
                            if (response.CharacterSet == null)
                            {
                                reader = new StreamReader(dataStream); ;
                            }
                            else
                                reader = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));

                            htmlCorreo = reader.ReadToEnd();
                            response.Close();
                            reader.Close();
                        };
                    }

                    if (htmlCorreo != "")
                        await _correoService.EnviarCorreo(usuario_creado.Correo, "Cuenta creada", htmlCorreo);

                }
                IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == usuario_creado.IdUsuario);
                usuario_creado = query.Include(r => r.IdRolNavigation).First();

                return usuario_creado;
            }
            catch (Exception ex)
            {
                throw;

            }
        }

        public async Task<Usuario> Editar(Usuario entidad, Stream Foto = null, string nombreFoto = "")
        {
            Usuario usuario_existe = await _repositorio.Obtener(u => u.Correo == entidad.Correo && u.IdUsuario != entidad.IdUsuario);

            if (usuario_existe != null)
            {
                throw new TaskCanceledException("El correo ya existe");
            }

            try
            {
                IQueryable<Usuario> queryUsuario = await _repositorio.Consultar(u => u.IdUsuario == entidad.IdUsuario);
                Usuario usuario_editar = queryUsuario.First();
                usuario_editar.Nombre = entidad.Nombre;
                usuario_editar.Correo = entidad.Correo;
                usuario_editar.Telefono = entidad.Telefono;
                usuario_editar.IdRol = entidad.IdRol;


                if (usuario_editar.NombreFoto == "")
                    usuario_editar.NombreFoto = nombreFoto;

                if (Foto != null)
                {
                    string urlFoto = await _firebaseService.SubirStorage(Foto, "carpeta_usuario", usuario_editar.NombreFoto);
                    usuario_editar.UrlFoto = urlFoto;

                }

                bool respuesta = await _repositorio.Editar(usuario_editar);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo modificar el usuario.");
                }
                Usuario usuario_editado = queryUsuario.Include(r => r.IdRolNavigation).First();

                return usuario_editado;
            }
            catch (Exception)
            {

                throw;
            }


        }

        public async Task<bool> Eliminar(int idUsuario)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(u => u.IdUsuario == idUsuario);

                if (usuario_encontrado == null)
                    throw new TaskCanceledException("El usuario no existe.");

                string nombreFoto = usuario_encontrado.NombreFoto;
                bool respuesta = await _repositorio.Eliminar(usuario_encontrado);

                if (respuesta)
                    await _firebaseService.EliminarStorage("carpeta_usuario", nombreFoto);

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            string clave_encriptada = _utilidadesService.ConvertirSha256(clave);
            Usuario usuario_encontrado = await _repositorio.Obtener(u => u.Correo.Equals(correo) && u.Clave.Equals(clave_encriptada));

            return usuario_encontrado;
        }

        public async Task<Usuario> ObtenerPorId(int idUsuario)
        {
            IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == idUsuario);
            Usuario resultado = query.Include(r => r.IdRolNavigation).FirstOrDefault();
            return resultado;
        }

        public async Task<bool> GuardarPerfil(Usuario entidad)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(u => u.IdUsuario == entidad.IdUsuario);

                if (usuario_encontrado == null)
                {
                    throw new TaskCanceledException("El usuario no existe");
                }

                usuario_encontrado.Correo = entidad.Correo;
                usuario_encontrado.Telefono = entidad.Telefono;

                bool respuesta = await _repositorio.Editar(usuario_encontrado);

                return respuesta;

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<bool> CambiarClave(int idUsuario, string claveActual, string claveNueva)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(u => u.IdUsuario == idUsuario);

                if (usuario_encontrado == null)
                {
                    throw new TaskCanceledException("El usuario no existe");
                }

                if (usuario_encontrado.Clave != _utilidadesService.ConvertirSha256(claveActual))
                    throw new TaskCanceledException("La contraseña ingresasa actual no es correcta.");

                usuario_encontrado.Clave = _utilidadesService.ConvertirSha256(claveNueva);
                bool respuesta = await _repositorio.Editar(usuario_encontrado);
                return respuesta;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<bool> RestablecerClave(string correo, string urlPlantillaCorreo)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(u => u.Correo == correo);
                if (usuario_encontrado == null)
                {
                    throw new TaskCanceledException("No se encontró usuario asociado a este correo.");
                }

                string clave_generada = _utilidadesService.GenerarClave();
                usuario_encontrado.Clave = _utilidadesService.ConvertirSha256(clave_generada);


                urlPlantillaCorreo = urlPlantillaCorreo.Replace("[clave]", clave_generada);
                string htmlCorreo = "";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlPlantillaCorreo);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = null;
                        if (response.CharacterSet == null)
                            reader = new StreamReader(dataStream);
                        else
                            reader = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));

                        htmlCorreo = reader.ReadToEnd();
                        response.Close();
                        reader.Close();
                    };
                }

                bool correo_enviado = false;

                if (htmlCorreo != "")
                    await _correoService.EnviarCorreo(correo, "Contraseña Reestablecida", htmlCorreo);

                if (!correo_enviado)
                    throw new TaskCanceledException("Tenemos problema. Por favor intentalo más tarde.");

                bool respuesta = await _repositorio.Editar(usuario_encontrado);
                return respuesta;

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
