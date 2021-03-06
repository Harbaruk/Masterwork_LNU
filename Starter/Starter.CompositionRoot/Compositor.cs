﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Starter.Common.TimeAPI;
using Starter.DAL.Infrastructure;
using Starter.Services.BankAccount;
using Starter.Services.Blocks;
using Starter.Services.Mining;
using Starter.Services.Mining.ServersClient;
using Starter.Services.QRCodes;
using Starter.Services.Registration;
using Starter.Services.TestDataGenerationService;
using Starter.Services.Token;
using Starter.Services.Transactions;
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
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<IServersClient, ServersClient>();
            services.AddScoped<IBlockService, BlockService>();
            services.AddScoped<IMiningService, MiningService>();

            services.AddScoped<ITestDataGenerationService, TestDataGenerationService>();

            services.AddScoped(typeof(TimeAPI));
        }
    }
}