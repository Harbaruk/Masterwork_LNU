using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Starter.API.Policies
{
    public class AuthenticatedUserPolicy
    {
        public static AuthorizationPolicy Policy
        {
            get
            {
                return new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes("Bearer")
                    .RequireAuthenticatedUser()
                    .Build();
            }
        }
    }
}