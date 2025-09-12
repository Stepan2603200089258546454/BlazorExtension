using DataContext.DataContext;
using DataContext.IdentityExtensions;
using DataContext.IdentityServices;
using IdentityAbstractions.Interfaces;
using IdentityAbstractions.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
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
                IDbContextFactory<ApplicationDbContext> dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
                using (ApplicationDbContext dbContext = await dbContextFactory.CreateDbContextAsync())
                {
                    if ((await dbContext.Database.GetPendingMigrationsAsync())?.Any() == true) //проверяем нужны ли миграции
                        dbContext.Database.Migrate(); //Пытаемся актуализировать и принять миграции при старте приложения
                }
            }
        }

        public static void AddIdentityEndpoints(this IEndpointRouteBuilder host)
        {
            host.MapAdditionalIdentityEndpoints();
        }

        public static void AddDataBase(this IHostApplicationBuilder builder)
        {
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IIdentityRedirectManager, IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
            
            builder.Services.AddScoped<IUserManager, IdentityUserManager>();
            builder.Services.AddScoped<IAccountManager, IdentityAccountManager>();

            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

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
    }
}
