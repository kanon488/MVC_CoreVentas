using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventas.DAL.DBContext;
using Ventas.DAL.Interfaces;
using Ventas.Entity;

namespace Ventas.DAL.Implementacion
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        private readonly DBVENTAContext _dBContext;

        public VentaRepository(DBVENTAContext dBContext) : base(dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<Venta> Registrar(Venta entidad) 
        {
            Venta ventaGenerada = new Venta();

            using (var transaction = _dBContext.Database.BeginTransaction()) 
            {
                try
                {
                    foreach (DetalleVenta dv in entidad.DetalleVenta)
                    {
                        Producto producto_encontrado = _dBContext.Productos.
                                                        Where(p => p.IdProducto == dv.IdProducto).First();

                        producto_encontrado.Stock = producto_encontrado.Stock - dv.Cantidad;
                        _dBContext.Productos.Update(producto_encontrado);
                    }

                    await _dBContext.SaveChangesAsync();

                    NumeroCorrelativo correlativo = _dBContext.NumeroCorrelativos.
                                                                Where(n => n.Gestion == "venta").First();

                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                    correlativo.FechaActualizacion = DateTime.Now;

                    _dBContext.NumeroCorrelativos.Update(correlativo);
                    await _dBContext.SaveChangesAsync();

                    string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();

                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);

                    entidad.NumeroVenta = numeroVenta;

                    await _dBContext.AddAsync(entidad);
                    await _dBContext.SaveChangesAsync();

                    ventaGenerada = entidad;
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                return ventaGenerada;
            }
        }

        public async Task<List<DetalleVenta>> Reporte(DateTime FechaInicio, DateTime FechaFin)
        {
            List<DetalleVenta> listaResumen = await _dBContext.DetalleVenta.
                Include(v => v.IdVentaNavigation)
                .ThenInclude(u => u.IdUsuarioNavigation)
                .Include(v => v.IdVentaNavigation)
                .ThenInclude(tdv => tdv.IdTipoDocumentoVentaNavigation)
                .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value >= FechaInicio.Date && 
                    dv.IdVentaNavigation.FechaRegistro.Value <= FechaFin.Date
                ).ToListAsync();

            return listaResumen;
        }
    }
}
