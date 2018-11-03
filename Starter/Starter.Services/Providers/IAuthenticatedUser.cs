using System;

namespace Starter.Services.Providers
{
    public interface IAuthenticatedUser
    {
        bool IsAuthenticated { get; }
        string Fullname { get; }
        Guid Id { get; }
        string Email { get; }
    }
}