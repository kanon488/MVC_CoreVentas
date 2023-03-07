using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventas.BLL.Interfaces;
using Ventas.DAL.Interfaces;
using Ventas.Entity;

namespace Ventas.BLL.Implementacion
{
    public class RolService : IRolService
    {
        private readonly IGenericRepository<Rol> _repositorio;


        public RolService(IGenericRepository<Rol> repositorio)
        {
            _repositorio = _repositorio;
        }
        public async Task<List<Rol>> Lista()
        {
            IQueryable<Rol> query = await _repositorio.Consultar();
            return query.ToList();

        }
    }
}
