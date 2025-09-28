using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Services.Base
{
    public abstract class BaseJSService : IAsyncDisposable
    {
        protected readonly Lazy<Task<IJSObjectReference>> moduleTask;

        protected BaseJSService(IJSRuntime jsRuntime, string fileName)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", $"./_content/CommonComponents/{fileName}.js").AsTask());
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
