using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using AutoMapper;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using Starter.API.Attributes;
using Starter.API.Crypto;
using Starter.API.Extensions;
using Starter.API.Policies;
using Starter.API.Providers;
using Starter.Common.DomainTaskStatus;
using Starter.CompositionRoot;
using Starter.DAL;
using Starter.Services.Blocks;
using Starter.Services.CacheManager;
using Starter.Services.Crypto;
using Starter.Services.Providers;
using Starter.Services.Token;
using Starter.Services.TwoFactorAuth.TOTP;
using Swashbuckle.AspNetCore.Swagger;

namespace Starter
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            Compositor.Compose(services);

            services.ConfigureFromSection<CryptoOptions>(Configuration);
            services.ConfigureFromSection<JwtOptions>(Configuration);
            services.ConfigureFromSection<RedisOptions>(Configuration);
            services.ConfigureFromSection<TotpOptions>(Configuration);
            services.ConfigureFromSection<BlockSettingsOptions>(Configuration);
            services.ConfigureFromSection<TrustfullServersOptions>(Configuration);

            services.AddSingleton<ICryptoContext, AspNetCryptoContext>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthenticatedUser, AuthenticatedUserProvider>();
            services.AddAutoMapper();
            services.AddSingleton<ICacheManager, CacheManager>();
            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = Configuration.GetSection("Redis")["ConnectionString"];
                option.InstanceName = "master";
            });

            services.AddScoped(typeof(DomainTaskStatus));
            services.AddScoped(typeof(ValidateModelAttribute));
            services.AddScoped(typeof(ErrorHandleAttribute));

            services.AddAuthorization(options => options.AddPolicy(AuthPolicies.AuthenticatedUser, AuthenticatedUserPolicy.Policy));
            services.AddAuthorization(options => options.AddPolicy(AuthPolicies.TwoFactorAuth, TwoFactorPolicy.Policy));
            services.AddAuthorization(options => options.AddPolicy(AuthPolicies.TrustfullServer, TrustfullServerPolicy.Policy));
            services.AddDbContext<ProjectDbContext>(o =>
            {
                string connStr = Configuration.GetConnectionString(_hostingEnvironment.EnvironmentName);
                if (String.IsNullOrWhiteSpace(connStr))
                {
                    throw new Exception($"No connection string defined for {_hostingEnvironment.EnvironmentName}");
                }
                o.UseSqlServer(connStr);
            }, ServiceLifetime.Scoped);

            services.AddCors(o => o.AddPolicy("CORS", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter
                {
                    AllowIntegerValues = false,
                    CamelCaseText = false
                });
                options.SerializerSettings.DateParseHandling = Newtonsoft.Json.DateParseHandling.DateTimeOffset;
                options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;
            })
           .AddFluentValidation(o =>
           {
               o.RegisterValidatorsFromAssemblyContaining<Startup>();
           });

            services.AddLogging();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration.GetSection("Jwt")["ValidIssuer"],
                    ValidAudience = Configuration.GetSection("Jwt")["ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Jwt")["Key"]))
                };
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Starter", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);
            });

            services.AddHangfire(x =>
            {
                x.UseDefaultActivator();
                x.UseSqlServerStorage(Configuration.GetConnectionString(_hostingEnvironment.EnvironmentName));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CORS");

            app.UseAuthentication();

            app.UseSwagger();

            app.UseStaticFiles();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Starter V1");
                c.EnableFilter();
            });

            app.UseMvc();

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HandfireAuthorizationAttribute() }
            });

            //todo: remove test
            RecurringJob.AddOrUpdate(() => Console.WriteLine("hello"), Cron.Minutely);
        }
    }
}