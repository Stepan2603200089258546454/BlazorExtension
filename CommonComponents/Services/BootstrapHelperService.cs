using CommonComponents.Models;
using CommonComponents.Services.Base;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Services
{
    public class BootstrapHelperService : BaseJSService
    {
        public BootstrapHelperService(IJSRuntime jsRuntime): base(jsRuntime, "bootstrapFunctions")
        {

        }

        public async ValueTask<bool> OpenModal(string id)
        {
            IJSObjectReference module = await moduleTask.Value;
            return await module.InvokeAsync<bool>("openModal", id);
        }
        public async ValueTask<bool> CloseModal(string id)
        {
            IJSObjectReference module = await moduleTask.Value;
            return await module.InvokeAsync<bool>("closeModal", id);
        }
    }
}
