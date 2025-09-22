using CommonComponents.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorExtension.Client.Pages
{
    [Route(_url)]
    public partial class Counter : ComponentBase
    {
        private const string _url = "/counter";
        private static IComponentRenderMode _renderAssigned = RenderMode.InteractiveAuto;

        private int currentCount = 0;
        private InteractiveInfo InteractiveInfo { get; set; }

        protected override void OnInitialized()
        {
            InteractiveInfo ??= new InteractiveInfo(RendererInfo, AssignedRenderMode);
            base.OnInitialized();
        }

        private void IncrementCount()
        {
            currentCount++;
        }
    }
}
