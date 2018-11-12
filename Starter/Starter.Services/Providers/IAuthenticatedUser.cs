using System;
using Starter.Services.Enums;

namespace Starter.Services.Providers
{
    public interface IAuthenticatedUser
    {
        bool IsAuthenticated { get; }
        string Fullname { get; }
        Guid Id { get; }
        string ServerHash { get; }
        string Email { get; }
        UserRoles? Role { get; }
    }
}