using DesafioVeiculos.Domain.Interfaces;
using DesafioVeiculos.Domain.Services;
using DesafioVeiculos.Infra.Data;
using DesafioVeiculos.Infra.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace DesafioVeiculos.Infra.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void ResolveDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DesafioVeiculosContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DatabaseConnection"));
            }, ServiceLifetime.Scoped);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IVeiculoService, VeiculoService>();

            services.AddControllers();
        }

    }
}
