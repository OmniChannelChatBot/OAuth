using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuth.Core;
using OAuth.Core.Interfaces;
using OAuth.Core.Options;
using OAuth.Core.Services;
using OAuth.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace OAuth.Api.Extensions
{
    internal static class ServiceCollectionExtension
    {
        private static readonly string _namespaceApplication = $"{nameof(OAuth)}.{nameof(Api)}.{nameof(Application)}";

        public static void AddApplicationServices(
            this IServiceCollection services,
            Action<DBApiOptions> actionDBbApiOptions,
            Action<CookieOptions> actionCookieOptions) => services
                .Configure(actionDBbApiOptions)
                .Configure(actionCookieOptions)
                .AddMediatRHandlers()
                .AddFluentValidators()
                .AddIntergationServices()
                .AddScoped<ITokenService, TokenService>()
                .AddSingleton<IPasswordService, PasswordService>();

        public static IServiceCollection AddCorsClients(this IServiceCollection services, Action<ClientOriginPolicyOptions> action)
        {
            var clientOriginPolicyOptions = new ClientOriginPolicyOptions();
            action.Invoke(clientOriginPolicyOptions);

            services.AddCors(sa => sa
                .AddPolicy(Constants.ClientOriginPolicy, cpb =>
                {
                    cpb.WithOrigins(clientOriginPolicyOptions.Origins);
                    cpb.WithMethods(clientOriginPolicyOptions.Methods);
                    cpb.WithHeaders(clientOriginPolicyOptions.Headers);
                    if (clientOriginPolicyOptions.IsCredentials)
                    {
                        cpb.AllowCredentials();
                    }
                }));

            return services;
        }

        public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, Action<AccessTokenOptions> accessTokenOptions, Action<RefreshTokenOptions> refreshTokenOptions)
        {
            services.Configure(accessTokenOptions);
            services.Configure(refreshTokenOptions);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                var accessTokenOptions = services
                    .BuildServiceProvider()
                    .GetRequiredService<IOptions<AccessTokenOptions>>();

                x.ClaimsIssuer = accessTokenOptions.Value.Issuer;
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = accessTokenOptions.Value.GetTokenValidationParameters();
                x.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

        private static IServiceCollection AddIntergationServices(this IServiceCollection services)
        {
            services.AddHttpClient<IDbApiServiceClient, DbApiServiceClient>();
            return services;
        }

        private static IServiceCollection AddFluentValidators(this IServiceCollection services) => services
            .Scan(scan => scan
                .FromAssemblies(typeof(Startup).Assembly)
                .AddClasses(classes => classes
                    .InNamespaces(_namespaceApplication)
                    .AssignableTo(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

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
    }
}
