using IdentityAbstractions.IdentityConstants;
using IdentityAbstractions.Interfaces;
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
    public partial class UserAndRoles : ComponentBase
    {
        private static IComponentRenderMode _renderAssigned = RenderMode.InteractiveServer;

        [Inject]
        public IIdentityManager Manager { get; set; }
    }
}
