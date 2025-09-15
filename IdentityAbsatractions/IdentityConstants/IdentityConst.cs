using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityAbstractions.IdentityConstants
{
    public static partial class IdentityConst
    {
        public const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        public const string StatusCookieName = "Identity.StatusMessage";
        public const string LoginCallbackAction = "LoginCallback";
        public const string LinkLoginCallbackAction = "LinkLoginCallback";
    }
}
