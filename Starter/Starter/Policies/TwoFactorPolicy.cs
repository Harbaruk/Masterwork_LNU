using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Starter.API.Policies
{
    public class TwoFactorPolicy
    {
        public static AuthorizationPolicy Policy
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes("Bearer")
                    .RequireAuthenticatedUser()
                    .RequireClaim(ClaimTypes.AuthenticationMethod, "TOTP")
                    .Build();
            }
        }
    }
}