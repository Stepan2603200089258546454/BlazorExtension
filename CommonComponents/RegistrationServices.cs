using CommonComponents.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents
{
    public static class RegistrationServices
    {
        /// <summary>
        /// Регистрируем общие сервисы из библиотеки общих сомпонентов
        /// </summary>
        public static void AddCommonServices(this IServiceCollection services)
        {
            services.AddScoped<BootstrapHelperService>();
            services.AddScoped<DeviceInfoService>();
        }
    }
}
