using CommonComponents.Enums.Bootstraps;
using CommonComponents.Models.Bootstraps;
using IdentityAbstractions.IdentityConstants;
using IdentityAbstractions.Interfaces;
using IdentityComponents.IdentityPages.Admins.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddictAbstractions.Models;
using OpenIddictServer.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityComponents.IdentityPages.Admins.Pages
{
    [Authorize]
    [Layout(typeof(AdminLayout))]
    [Route(IdentityConst.IdentityRoute.Admins.OpenIddictEditor)]
    public partial class OpenIddict : ComponentBase, IDisposable
    {
        private static IComponentRenderMode _renderAssigned = RenderMode.InteractiveServer;

        [Inject]
        public IOpenIddictManager Manager { get; set; }

        private ModalDialogSettings ModalDialogApp = new ModalDialogSettings()
        {
            Id = "modalApp",
            Backdrop = ModalDialogBackdrop.Static,
            Position = ModalDialogPosition.Center,
            Size = ModalDialogSize.XL,
        };

        private CancellationTokenSource? cancelTokenSource = new CancellationTokenSource();
        private CancellationToken cancelToken => cancelTokenSource?.Token ?? default;

        private bool IsLoadingApplications { get; set; } = false;
        private IList<OpenIddictEntityFrameworkCoreApplication>? Applications { get; set; }

        private bool IsLoadingScopes { get; set; } = false;
        private IList<OpenIddictEntityFrameworkCoreScope>? Scopes { get; set; }

        private bool IsSelectedApplication { get; set; } = false;
        private CreateAppClientViewModel? SelectedApplication { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (RendererInfo.IsInteractive)
            {
                await LoadApplications();
                await LoadScopes();
            }
            await base.OnInitializedAsync();
        }

        private async Task LoadApplications()
        {
            IsLoadingApplications = true;
            Applications = await Manager.GetApplicationsAsync();
            IsLoadingApplications = false;
        }
        private async Task LoadScopes()
        {
            IsLoadingScopes = true;
            Scopes = await Manager.GetScopesAsync();
            IsLoadingScopes = false;
        }

        private void Select(OpenIddictEntityFrameworkCoreApplication app)
        {
            IsSelectedApplication = true;
            SelectedApplication = Manager.Edit(app);
            IsSelectedApplication = false;
        }
        private async Task Delete(OpenIddictEntityFrameworkCoreApplication app)
        {
            await Manager.DeleteAsync(app);
            await LoadApplications();
            await LoadScopes();
        }

        private void AddNewApp(MouseEventArgs args)
        {
            IsSelectedApplication = true;
            SelectedApplication = new CreateAppClientViewModel();
            IsSelectedApplication = false;
        }

        private async Task AppOk(bool? args)
        {
            if (args == true)
            {
                await Manager.AddOrUpdateAsync(SelectedApplication);
                await LoadApplications();
                await LoadScopes();
            }
            else
            {

            }
            SelectedApplication = null;
        }

        private ButtonSettings GetPresetOpenModal(string idModal) => ButtonSettings.GetPresetOpenModal(string.Empty, idModal);
        private ButtonSettings GetPresetDefault(ButtonStyleType styleType) => ButtonSettings.GetPresetDefault(string.Empty, styleType);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                cancelTokenSource?.Cancel();
                cancelTokenSource?.Dispose();
                cancelTokenSource = null;

        
                Applications = null;
            }

            _disposed = true;
        }
    }
}
