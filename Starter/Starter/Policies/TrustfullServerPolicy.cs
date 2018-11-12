using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Starter.API.Policies
{
    [Obsolete("Not for real usage, insecure workaround")]
    public class TrustfullServerPolicy
    {
        public static AuthorizationPolicy Policy
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes("Bearer")
                    .RequireAuthenticatedUser()
                    .RequireClaim(ClaimTypes.Role, "Server")
                    .Build();
            }
        }
    }
}