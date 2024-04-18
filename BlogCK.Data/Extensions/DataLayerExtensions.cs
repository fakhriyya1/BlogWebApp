using BlogCK.Data.Context;
using BlogCK.Data.Repositories.Abstractions;
using BlogCK.Data.Repositories.Concretes;
using BlogCK.Data.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogCK.Data.Extensions
{
    public static class DataLayerExtensions
    {
        public static IServiceCollection LoadDatalayerExtension(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddDbContext<AppDbContext>(option => option.UseSqlServer(config.GetConnectionString("Default")));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
