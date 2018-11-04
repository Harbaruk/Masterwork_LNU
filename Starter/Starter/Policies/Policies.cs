using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Starter.API.Policies
{
    public static class AuthPolicies
    {
        public const string AuthenticatedUser = "AuthenticatedUser";

        public const string TwoFactorAuth = "TwoFactorAuth";
    }
}