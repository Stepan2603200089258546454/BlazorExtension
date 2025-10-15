using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIddictAbstraction
{
    public static class Constants
    {
        public static class Endpoints
        {
            public const string Authorization = "connect/authorize";
            public const string EndSession = "connect/logout";
            public const string Introspection = "connect/introspect";
            public const string Token = "connect/token";
            public const string UserInfo = "connect/userinfo";
            public const string EndUserVerification = "connect/verify";
        }
    }
}
