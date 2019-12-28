using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuth.Api.Controllers.Filters;
using OAuth.Core.Interfaces;
using OAuth.Core.Options;
using OAuth.Core.Services;
using OAuth.Infrastructure.Services;
using System;
using System.Text;

namespace OAuth.Api.Extensions
{
    internal static class ServiceCollectionExtension
    {
        private static readonly string _namespaceApplication = $"{nameof(OAuth)}.{nameof(Api)}.{nameof(Application)}";

        public static void AddApplicationServices(this IServiceCollection services, Action<DBApiOptions> options) => services
            .Configure(options)
            .AddMediatR()
            .AddMediatRHandlers()
            .AddIntergationServices()
            .AddScoped<ISecurityTokenService, SecurityTokenService>();

        public static IServiceCollection AddHealthCheckServices(this IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddCustomHealthChecks();
            return services;
        }

        public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, Action<SecurityTokenOptions> options)
        {
            services.Configure(options);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                var securityTokenOptions = services
                    .BuildServiceProvider()
                    .GetRequiredService<IOptions<SecurityTokenOptions>>();
                var key = Encoding.UTF8.GetBytes(securityTokenOptions.Value.Secret);

                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateIssuer = true,
                    ValidIssuer = securityTokenOptions.Value.Issuer,
                    ValidateAudience = true,
                    ValidAudience = securityTokenOptions.Value.Audience,
                    ValidateLifetime = true,

                    ClockSkew = TimeSpan.FromSeconds(5)
                };
            });

            return services;
        }
        public static IMvcBuilder AddApiServices(this IServiceCollection services) => services
            .AddFluentValidators()
            .AddMvcActionFilters()
            .AddControllers(o =>
            {
                o.Filters.AddService<ApiActionFilter>();
                o.Filters.AddService<ApiResultFilter>();
            })
                .AddJsonOptions(o => o.JsonSerializerOptions.IgnoreNullValues = true)
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                    options.SuppressMapClientErrors = true;
                })
                .AddFluentValidation();

        private static IServiceCollection AddIntergationServices(this IServiceCollection services)
        {
            services.AddHttpClient<IDbApiServiceClient, DbApiServiceClient>();

            return services;
        }

        private static IServiceCollection AddMediatR(this IServiceCollection services) => services
            .AddScoped<ServiceFactory>(p => t => p.GetService(t))
            .AddScoped<IMediator, Mediator>();

        private static IServiceCollection AddMediatRHandlers(this IServiceCollection services) => services
            .Scan(scan =>
            {
                scan
                    .FromAssemblies(typeof(Startup).Assembly)
                    .AddClasses(classes => classes
                        .InNamespaces(_namespaceApplication)
                        .AssignableTo(typeof(IRequestHandler<>)))
                    .AsImplementedInterfaces()
                    .AddClasses(classes => classes
                        .InNamespaces(_namespaceApplication)
                        .AssignableTo(typeof(IRequestHandler<,>)))
                    .AsImplementedInterfaces()
                    .AddClasses(classes => classes
                        .InNamespaces(_namespaceApplication)
                        .AssignableTo(typeof(IPipelineBehavior<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime();
            });

        private static IServiceCollection AddFluentValidators(this IServiceCollection services) => services
            .Scan(scan =>
            {
                scan
                    .FromAssemblies(typeof(Startup).Assembly)
                    .AddClasses(classes => classes
                        .InNamespaces(_namespaceApplication)
                        .AssignableTo(typeof(IValidator<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime();
            });

        private static IServiceCollection AddMvcActionFilters(this IServiceCollection services) => services
            .AddScoped<ApiActionFilter>()
            .AddScoped<ApiResultFilter>();
    }
}
