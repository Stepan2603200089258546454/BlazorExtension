using IdentityAbstractions;
using IdentityAbstractions.IdentityConstants;
using IdentityAbstractions.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace DataContext.IdentityExtensions
{
    public sealed class IdentityRedirectManager(NavigationManager navigationManager) : IIdentityRedirectManager
    {
        private string CurrentPath => navigationManager.ToAbsoluteUri(navigationManager.Uri).GetLeftPart(UriPartial.Path);

        private static readonly CookieBuilder StatusCookieBuilder = new()
        {
            SameSite = SameSiteMode.Strict,
            HttpOnly = true,
            IsEssential = true,
            MaxAge = TimeSpan.FromSeconds(5),
        };

        [DoesNotReturn]
        public void RedirectTo(string? uri)
        {
            uri ??= string.Empty;

            // Запретить открытые перенаправления.
            if (Uri.IsWellFormedUriString(uri, UriKind.Relative) == false)
            {
                uri = navigationManager.ToBaseRelativePath(uri);
            }

            navigationManager.NavigateTo(uri);
            // Во время статического рендеринга NavigateTo генерирует исключение NavigationException,
            // которое обрабатывается фреймворком как перенаправление.
            // Поэтому, пока этот метод вызывается из статически отрисованного компонента Identity,
            // исключение InvalidOperationException никогда не генерируется.
            throw new InvalidOperationException($"{nameof(IdentityRedirectManager)} можно использовать только во время статического рендеринга.");
        }

        [DoesNotReturn]
        public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
        {
            string uriWithoutQuery = navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
            string newUri = navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
            RedirectTo(newUri);
        }

        [DoesNotReturn]
        public void RedirectToWithStatus(string uri, string message, HttpContext context)
        {
            context.Response.Cookies.Append(IdentityConst.StatusCookieName, message, StatusCookieBuilder.Build(context));
            RedirectTo(uri);
        }

        [DoesNotReturn]
        public void RedirectToCurrentPage() => RedirectTo(CurrentPath);

        [DoesNotReturn]
        public void RedirectToCurrentPageWithStatus(string message, HttpContext context)
            => RedirectToWithStatus(CurrentPath, message, context);
    }
}
