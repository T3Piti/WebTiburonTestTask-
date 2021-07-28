using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTiburonTestTask.Context;
using WebTiburonTestTask.Interfaces;
using WebTiburonTestTask.Services;

namespace WebTiburonTestTask
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuretion)
        {
            services.AddControllers();
            services.AddHttpClient();
            services.AddDistributedMemoryCache();
            services.AddSession();

            var connectionString = configuretion["DbConnection"];
            services.AddDbContext<SurveyDBContext>(option => option.UseNpgsql(configuretion.GetConnectionString("DbConnection")));
            services.AddScoped<ISurveyService, SurveyService>();
            services.AddResponseCompression(options => options.EnableForHttps = true);
            return services;
        }
    }
}
