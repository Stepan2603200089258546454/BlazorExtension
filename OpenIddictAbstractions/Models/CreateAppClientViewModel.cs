using System.ComponentModel.DataAnnotations;

namespace OpenIddictAbstractions.Models
{
    public class CreateAppClientViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string RedirectUris { get; set; } = string.Empty;
        public string PostLogoutRedirectUris { get; set; } = string.Empty;
    }
}
