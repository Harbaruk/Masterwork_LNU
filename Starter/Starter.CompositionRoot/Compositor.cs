using System;
using Microsoft.Extensions.DependencyInjection;
using Starter.DAL.Infrastructure;
using Starter.Services.Token;

namespace Starter.CompositionRoot
{
    /// <summary>
    /// Register all services from here
    /// </summary>
    public static class Compositor
    {
        public static void Compose(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, EFUnitOfWork>();
            services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));

            services.AddScoped<ITokenService, TokenService>();
        }
    }
}