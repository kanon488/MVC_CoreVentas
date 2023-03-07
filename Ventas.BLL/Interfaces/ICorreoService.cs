using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ventas.BLL.Interfaces
{
    public interface ICorreoService
    {

        Task<bool> EnviarCorreo(string corroDestino, string asunto, string mensaje);
    }

}
