using CommonComponents.Models;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Services
{
    public class DeviceInfo : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public DeviceInfo(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/CommonComponents/deviceDetector.js").AsTask());
        }

        public async ValueTask<ViewportSize> GetViewportSize()
        {
            IJSObjectReference module = await moduleTask.Value;
            var result = await module.InvokeAsync<ViewportSize>("getViewportSize");
            return result;
        }
        public async ValueTask<DeviceInfo> GetDeviceInfo()
        {
            IJSObjectReference module = await moduleTask.Value;
            var result = await module.InvokeAsync<DeviceInfo>("getDeviceInfo");
            return result;
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}
