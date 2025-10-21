using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIddictAbstractions.Constants
{
    public static partial class OpenIddictConst
    {
        public static class Route
        {
            public static class OppenIddictServer
            {
                public const string AuthorizationEndpoint = "/connect/authorize";
                public const string EndSessionEndpoint = "/connect/logout";
                public const string IntrospectionEndpoint = "/connect/introspect";
                public const string TokenEndpoint = "/connect/token";
                public const string UserInfoEndpoint = "/connect/userinfo";
                public const string EndUserVerificationEndpoint = "/connect/verify";
            }
        }
    }
}
