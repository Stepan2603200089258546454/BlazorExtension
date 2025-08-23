using DataContext.DataContext;
using DataContext.IdentityModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContext
{
    public static class RegistrationDataBase
    {
        public static async ValueTask AutoMigrate(this IHost host)
        {
            // Миграции EF Core
            using (var scope = host.Services.CreateScope())
            {
                var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
                using (var dbContext = await dbContextFactory.CreateDbContextAsync())
                {
                    if ((await dbContext.Database.GetPendingMigrationsAsync())?.Any() == true) //проверяем нужны ли миграции
                        dbContext.Database.Migrate(); //Пытаемся актуализировать и принять миграции при старте приложения
                }
            }
        }

        public static void AddDataBase(this IHostApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            }).AddIdentityCookies();

            string connectionString = builder.Configuration.GetSection("PostgreSettings")?["DefaultConnection"] ?? throw new InvalidOperationException("Строка подключения «DefaultConnection» не найдена.");
            string version = builder.Configuration.GetSection("PostgreSettings")?["Version"] ?? throw new InvalidOperationException("Не указана версия PostgreSQL");

            builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
                    options.UseNpgsql(connectionString, npgsqlOptions =>
                    {
                        npgsqlOptions.SetPostgresVersion(Version.Parse(version));
                    }));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();
        }
        //public static void AddDataBase(this IServiceCollection services)
        //{

        //}
    }
}
