using CommonComponents.Enums;
using CommonComponents.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Services
{
    public class DeviceInfoService : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public DeviceInfoService(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/CommonComponents/deviceDetector.js").AsTask());
        }
        /// <summary>
        /// Получить размеры экрана (только интерактив)
        /// </summary>
        public async ValueTask<ViewportSize> GetViewportSize()
        {
            IJSObjectReference module = await moduleTask.Value;
            var result = await module.InvokeAsync<ViewportSize>("getViewportSize");
            return result;
        }
        /// <summary>
        /// Получить информацию о устройстве (только интерактив)
        /// </summary>
        public async ValueTask<DeviceInfo> GetDeviceInfo()
        {
            return new DeviceInfo(await GetUserAgent());
        }
        /// <summary>
        /// Получить информацию о устройстве (только статичный)
        /// </summary>
        public DeviceInfo GetDeviceInfo(HttpContext? context)
        {
            return new DeviceInfo(GetUserAgent(context));
        }

        private string GetUserAgent(HttpContext? context)
        {
            if (context == null) return string.Empty;
            return context.Request.Headers["User-Agent"].ToString();
        }
        private async ValueTask<string> GetUserAgent()
        {
            IJSObjectReference module = await moduleTask.Value;
            return await module.InvokeAsync<string>("getUserAgent");
        }
        
        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                IJSObjectReference module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}
