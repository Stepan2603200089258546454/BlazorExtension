using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace IdentityAbstractions.Interfaces
{
    public interface IIdentityRedirectManager
    {
        [DoesNotReturn]
        public void RedirectTo(string? uri);
        [DoesNotReturn]
        public void RedirectTo(string uri, Dictionary<string, object?> queryParameters);
        [DoesNotReturn]
        public void RedirectToWithStatus(string uri, string message, HttpContext context);
        [DoesNotReturn]
        public void RedirectToCurrentPage();
        [DoesNotReturn]
        public void RedirectToCurrentPageWithStatus(string message, HttpContext context);
    }
}
