using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventas.BLL.Interfaces;
using Firebase.Storage;
using Ventas.Entity;
using Ventas.DAL.Interfaces;
using Firebase.Auth;

namespace Ventas.BLL.Implementacion
{
    public class FireBaseService : IFirebaseService
    {

        private readonly IGenericRepository<Configuracion> _repositorio;

        public FireBaseService(IGenericRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<string> SubirStorage(Stream streamArchivo, string carpetaDestino, string nombreArchivo)
        {
            string sUrlImagen = "";

            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);



                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                Config["ruta"],
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                })    
                .Child(Config[carpetaDestino])
                .Child(nombreArchivo)
                .PutAsync(streamArchivo,cancellation.Token);

                sUrlImagen = await task;
            }
            catch (Exception ex)
            {
                sUrlImagen = "";
            }
            return sUrlImagen;
        }


        public async Task<bool> EliminarStorage(string carpetaDestino, string nombreArchivo)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));

                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);



                var auth = new FirebaseAuthProvider(new FirebaseConfig(Config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(Config["email"], Config["clave"]);

                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                Config["ruta"],
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                })
                .Child(Config[carpetaDestino])
                .Child(nombreArchivo)
                .DeleteAsync();
                
                await task;

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
}
