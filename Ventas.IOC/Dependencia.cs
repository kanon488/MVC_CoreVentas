using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ventas.DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using Ventas.DAL.Interfaces;
using Ventas.DAL.Implementacion;
using Ventas.BLL.Interfaces;
using Ventas.BLL.Implementacion;

namespace Ventas.IOC
{
    public static class Dependencia
    {
        public static void InyectarDependencia(this IServiceCollection services, IConfiguration Configuration) 
        {
            services.AddDbContext<DBVENTAContext>(options => 
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultDB"));
            });
        }
    }
}
