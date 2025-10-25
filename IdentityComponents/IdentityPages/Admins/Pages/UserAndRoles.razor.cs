using CommonComponents.Enums.Bootstraps;
using CommonComponents.Models;
using CommonComponents.Models.Bootstraps;
using IdentityAbstractions.IdentityConstants;
using IdentityAbstractions.Interfaces;
using IdentityAbstractions.Models;
using IdentityComponents.IdentityPages.Admins.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityComponents.IdentityPages.Admins.Pages
{
    [Authorize]
    [Layout(typeof(AdminLayout))]
    [Route(IdentityConst.IdentityRoute.Admins.UserAndRoleEditor)]
    public partial class UserAndRoles : ComponentBase, IDisposable
    {
        private static IComponentRenderMode _renderAssigned = RenderMode.InteractiveServer;

        [Inject]
        public IIdentityManager Manager { get; set; }

        private ModalDialogSettings ModalDialogUser = new ModalDialogSettings()
        {
            Id = "modalUser",
            Backdrop = ModalDialogBackdrop.Static,
            Position = ModalDialogPosition.Center,
            Size = ModalDialogSize.XL,
        };
        private ModalDialogSettings ModalDialogRole = new ModalDialogSettings()
        {
            Id = "modalRole",
            Backdrop = ModalDialogBackdrop.Static,
            Position = ModalDialogPosition.Center,
            Size = ModalDialogSize.XL,
        };
        private AccordionSettings accordionSettings = new AccordionSettings()
        {
            Id = "accordion-user-roles",
            AddClass = "mb-0",
            StyleType = AccordionStyleType.Default,
            CollapsedType = AccordionCollapsedType.NoCollapsed,
        };


        private IList<ApplicationUser>? Users { get; set; }
        private ApplicationUser? SelectedUser { get; set; }
        private IList<SelectedModel<ApplicationRole>>? UserRoles { get; set; }
        private IList<SelectedModel<ApplicationRole>>? AvailableUserRoles { get; set; }

        private IList<ApplicationRole>? Roles { get; set; }
        private ApplicationRole? SelectedRole { get; set; }

        private bool IsLoadingUsers { get; set; } = false;
        private bool IsSelectedUser { get; set; } = false;
        private bool IsLoadingRoles { get; set; } = false;
        private bool IsSelectedRole { get; set; } = false;

        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        private CancellationToken cancelToken => cancelTokenSource.Token;

        protected override async Task OnInitializedAsync()
        {
            if (RendererInfo.IsInteractive)
            {
                await LoadUsers();
                await LoadRoles();
            }
            await base.OnInitializedAsync();
        }
        private async Task LoadUsers()
        {
            IsLoadingUsers = true;
            Users = await Manager.GetAllUsers(cancelToken);
            IsLoadingUsers = false;
        }
        private async Task LoadRoles()
        {
            IsLoadingRoles = true;
            Roles = await Manager.GetAllRoles(cancelToken);
            IsLoadingRoles = false;
        }
        private async Task Select(ApplicationUser user)
        {
            IsSelectedUser = true;
            SelectedUser = user;
            await UploadUserRoles();
            IsSelectedUser = false;
        }
        private void Select(ApplicationRole role)
        {
            IsSelectedRole = true;
            SelectedRole = role;
            IsSelectedRole = false;
        }
        private async Task Delete(ApplicationRole role)
        {
            await Manager.DeleteRoleAsync(role);
            await LoadRoles();
        }
        private async Task UploadUserRoles()
        {
            if (SelectedUser is not null)
            {
                UserRoles = (await Manager.GetUserRolesAsync(SelectedUser, cancelToken))
                    .Select(x => new SelectedModel<ApplicationRole>(x))
                    .ToList();
                AvailableUserRoles = (await Manager.GetAvailableRolesForUserAsync(SelectedUser, cancelToken))
                    .Select(x => new SelectedModel<ApplicationRole>(x))
                    .ToList();
            }
            else
            {
                UserRoles = null;
                AvailableUserRoles = null;
            }
        }

        private ButtonSettings GetPresetDefault(ButtonStyleType styleType) => ButtonSettings.GetPresetDefault(string.Empty, styleType);
        private ButtonSettings GetPresetOpenModal(string idModal) => ButtonSettings.GetPresetOpenModal(string.Empty, idModal);

        private async Task RoleOk(bool? args)
        {
            if (SelectedRole == null) return;

            if (args == true)
            {
                if (string.IsNullOrEmpty(SelectedRole.Name) == true) return;

                if (string.IsNullOrEmpty(SelectedRole.Id) == true)
                {
                    _ = await Manager.CreateRoleAsync(SelectedRole.Name);
                }
                else
                {
                    _ = await Manager.UpdateRoleAsync(SelectedRole, SelectedRole.Name);
                }
                await LoadRoles();
            }
            else
            {

            }
            SelectedRole = null;
        }
        private async Task UserOk(bool? args)
        {
            if (args == true)
            {
                if (SelectedUser is null) return;

                IEnumerable<string>? deleteUserRoles = UserRoles?.Where(x => x.IsSelected).Select(x => x.Model.Name);
                if (deleteUserRoles?.Count() > 0)
                {
                    await Manager.RemoveRolesFromUserAsync(SelectedUser, deleteUserRoles);
                }
                IEnumerable<string>? availableUserRoles = AvailableUserRoles?.Where(x => x.IsSelected).Select(x => x.Model.Name);
                if (availableUserRoles?.Count() > 0)
                {
                    await Manager.AssignRolesToUserAsync(SelectedUser, availableUserRoles);
                }
            }
            else
            {

            }
            SelectedUser = null;
            UserRoles = null;
            AvailableUserRoles = null;
        }
        private void AddNewRole(MouseEventArgs args)
        {
            SelectedRole = new ApplicationRole()
            {
                Id = string.Empty,
            };
        }

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

                Users = null;
                SelectedUser = null;
                UserRoles = null;
                AvailableUserRoles = null;
                Roles = null;
                SelectedRole = null;
            }

            _disposed = true;
        }
    }
}
