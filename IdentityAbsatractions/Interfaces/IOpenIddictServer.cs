using IdentityAbstractions.FormsModels;
using Microsoft.AspNetCore.Http;

namespace IdentityAbstractions.Interfaces
{
    public interface IOpenIddictServer
    {
        public Task<(AuthorizeViewModel? Model, bool? IgnoreAuthenticationChallenge)> AuthorizeAsync(HttpContext HttpContext, bool? IgnoreAuthenticationChallenge = null);
        public Task AcceptAsync(HttpContext HttpContext);
        public Task DenyAsync(HttpContext HttpContext);
        public Task LogoutAsync(HttpContext HttpContext);
        public Task ExchangeAsync(HttpContext HttpContext);
    }
}
