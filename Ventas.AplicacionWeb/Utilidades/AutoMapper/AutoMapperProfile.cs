using AutoMapper;
using System.Globalization;
using Ventas.AplicacionWeb.Models.ViewModels;
using Ventas.Entity;

namespace Ventas.AplicacionWeb.Utilidades.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region Rol
            CreateMap<Rol, VMRol>().ReverseMap();
            #endregion Rol

            #region Usuario
            CreateMap<Usuario, VMUsuario>()
                .ForMember(destino => destino.EsActivo,
               opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)
              )
               .ForMember(destino => destino.NombreRol,
               opt => opt.MapFrom(origen => origen.IdRolNavigation.Descripcion));

            CreateMap<VMUsuario, Usuario>()
                .ForMember(
                   destino =>
                   destino.EsActivo,
                   opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)
                )
                .ForMember(
                    destino =>
                    destino.IdRolNavigation,
                    opt => opt.Ignore()
                );
            #endregion Usuario

            #region Negocio
            CreateMap<Negocio, VMNegocio>()
                .ForMember(
                destino =>
                destino.PorcentajeImpuesto,
                opt => opt.MapFrom(origen => Convert.ToString(origen.PorcentajeImpuesto.Value,
                new CultureInfo("es-MX")))
                );

            CreateMap<VMNegocio,Negocio>()
                .ForMember(
                destino =>
                destino.PorcentajeImpuesto,
                opt => opt.MapFrom(origen => Convert.ToDecimal(origen.PorcentajeImpuesto,
                new CultureInfo("es-MX")))
                );

            #endregion Negocio

            #region Categoría
            CreateMap<Categoria, VMCategoria>()
                .ForMember(
                destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)
                );
            CreateMap<VMCategoria,Categoria>()
                .ForMember(
                destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)
                );

            #endregion Categoría

            #region Producto

            CreateMap<Producto, VMProducto>()
                .ForMember(
                 destino =>
                destino.EsActivo,
                   opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)
                )
                .ForMember(destino =>
                 destino.NombreCategoria,
                 opt => opt.MapFrom(origen => origen.IdCategoriaNavigation.Descripcion)
                ).
                ForMember(destino =>
                  destino.Precio,
                 opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-MX")))
                );

            CreateMap<VMProducto,Producto>()
                .ForMember(
                 destino =>
                destino.EsActivo,
                   opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)
                )
                .ForMember(destino =>
                 destino.IdCategoriaNavigation,
                 opt => opt.Ignore()
                ).
                ForMember(destino =>
                  destino.Precio,
                 opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-MX")))
                );



            #endregion  Producto

            #region TipoDocumentoVenta
            CreateMap<TipoDocumentoVenta, VMTipoDocumentoVenta>().ReverseMap();

            #endregion TipoDocumentoVenta

            #region  Venta
                //CreateMap<Venta,VMVenta>()
            #endregion Venta

        }
    }
}
