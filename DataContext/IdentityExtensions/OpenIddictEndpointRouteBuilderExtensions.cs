using IdentityAbstractions.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DataContext.IdentityExtensions
{
    //internal static class OpenIddictEndpointRouteBuilderExtensions
    //{
    //    public static void MapOpenIddictEndpoints(this IEndpointRouteBuilder endpoints)
    //    {
    //        endpoints.MapPost("/connect/logout", Logout)
    //            .ExcludeFromDescription();
    //        endpoints.MapPost("/connect/token", ExchangeAsync)
    //            .DisableAntiforgery()
    //            .ExcludeFromDescription();
    //    }

    //    private static async Task Logout(HttpContext HttpContext, [FromServices] IOpenIddictServer server)
    //    {
    //        await server.LogoutAsync(HttpContext);
    //    }
    //    private static async Task ExchangeAsync(HttpContext HttpContext, [FromServices] IOpenIddictServer server)
    //    {
    //        await server.ExchangeAsync(HttpContext);
    //    }
    //}
}
