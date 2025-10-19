using OpenIddict.Abstractions;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace OpenIddictServer.Helpers
{
    public static class DestinationsHelpers
    {
        public static IEnumerable<string> GetDestinations(Claim claim)
        {
            // Примечание: по умолчанию утверждения НЕ включаются автоматически в токены доступа и идентификации.
            // Чтобы разрешить OpenIddict сериализовать их, необходимо прикрепить к ним назначение, которое указывает
            // следует ли их включать в токены доступа, в токены идентификации или в оба.

            switch (claim.Type)
            {
                case Claims.Name or Claims.PreferredUsername:
                    yield return Destinations.AccessToken;

                    if (claim.Subject!.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Name or Claims.Subject:
                    yield return Destinations.AccessToken;
                    yield return Destinations.IdentityToken;
                    yield break;

                case Claims.Email:
                    yield return Destinations.AccessToken;

                    if (claim.Subject!.HasScope(Scopes.Email))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Role:
                    yield return Destinations.AccessToken;

                    if (claim.Subject!.HasScope(Scopes.Roles))
                        yield return Destinations.IdentityToken;

                    yield break;

                // Никогда не включайте штамп безопасности в токены доступа и идентификации, так как это секретное значение.
                case "AspNet.Identity.SecurityStamp": yield break;

                default:
                    yield return Destinations.AccessToken;
                    yield break;
            }
        }
    }
}
