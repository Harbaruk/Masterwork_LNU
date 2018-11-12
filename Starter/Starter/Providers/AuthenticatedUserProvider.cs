using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Starter.Services.Enums;
using Starter.Services.Providers;

namespace Starter.API.Providers
{
    public class AuthenticatedUserProvider : IAuthenticatedUser
    {
        private readonly IHttpContextAccessor _httpContext;

        public AuthenticatedUserProvider(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public bool IsAuthenticated
        {
            get
            {
                ClaimsPrincipal principal = _httpContext.HttpContext.User;
                return principal?.Identity?.IsAuthenticated ?? false;
            }
        }

        public Guid Id
        {
            get
            {
                var user = _httpContext.HttpContext.User;
                if (!(user?.Identity?.IsAuthenticated) ?? false)
                {
                    return default(Guid);
                }
                string nameId = user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid).Value;
                return Guid.Parse(nameId);
            }
        }

        public string Email
        {
            get
            {
                var user = _httpContext.HttpContext.User;
                return user?.Identity?.IsAuthenticated ?? false
                    ? user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value
                    : null;
            }
        }

        public string Fullname
        {
            get
            {
                var user = _httpContext.HttpContext.User;
                return user?.Identity?.IsAuthenticated ?? false
                    ? user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value
                    : null;
            }
        }

        public string ServerHash
        {
            get
            {
                var user = _httpContext.HttpContext.User;
                return user?.Identity?.IsAuthenticated ?? false
                    ? user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value
                    : null;
            }
        }

        public UserRoles? Role
        {
            get
            {
                var user = _httpContext.HttpContext.User;
                var result = user?.Identity?.IsAuthenticated ?? false ?
                   user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value : null;

                if (result != null)
                {
                    return Enum.Parse<UserRoles>(result);
                }
                return null;
            }
        }
    }
}