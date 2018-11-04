using System;
using Microsoft.Extensions.DependencyInjection;
using Starter.Common.TimeAPI;
using Starter.DAL.Infrastructure;
using Starter.Services.QRCodes;
using Starter.Services.Registration;
using Starter.Services.Token;
using Starter.Services.TwoFactorAuth.TOTP;

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
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddScoped<ITotpProvider, TotpProvider>();
            services.AddScoped<IQRCodeService, QRCodeService>();
            services.AddScoped(typeof(TimeAPI));
        }
    }
}