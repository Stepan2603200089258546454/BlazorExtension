using IdentityAbstractions.IdentityConstants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityComponents.IdentityPages.Account.Shared
{
    public partial class StatusMessage : ComponentBase
    {
        private string? messageFromCookie;

        [Parameter]
        public string? Message { get; set; }

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        private string? DisplayMessage => Message ?? messageFromCookie;

        protected override void OnInitialized()
        {
            messageFromCookie = HttpContext.Request.Cookies[IdentityConst.StatusCookieName];

            if (messageFromCookie is not null)
            {
                HttpContext.Response.Cookies.Delete(IdentityConst.StatusCookieName);
            }
        }
    }
}
