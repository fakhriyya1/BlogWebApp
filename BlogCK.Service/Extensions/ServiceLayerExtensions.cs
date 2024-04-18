using BlogCK.Service.FluentValidations;
using BlogCK.Service.Helpers.Images;
using BlogCK.Service.Services.Abstractions;
using BlogCK.Service.Services.Concrete;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Reflection;

namespace BlogCK.Service.Extensions
{
    public static class ServiceLayerExtensions
    {
        public static IServiceCollection LoadServicelayerExtension(this IServiceCollection services)
        {
            var assembly=Assembly.GetExecutingAssembly();

            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IImageHelper, ImageHelper>();
            services.AddScoped<IDashboardService, DashboardService>();

            services.AddAutoMapper(assembly);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddControllersWithViews().AddFluentValidation(opt =>
            {
                opt.RegisterValidatorsFromAssemblyContaining<ArticleValidator>();
                opt.DisableDataAnnotationsValidation = true;

                //opt.ValidatorOptions.LanguageManager.Culture = new CultureInfo("az");  - errrorlarin dilini deyisdirme. WithName()-e de bax!!
            });

            return services;
        }
    }
}
