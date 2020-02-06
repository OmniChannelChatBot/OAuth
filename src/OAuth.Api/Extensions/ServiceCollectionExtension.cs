using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuth.Core.Interfaces;
using OAuth.Core.Options;
using OAuth.Core.Services;
using OAuth.Infrastructure.Services;
using OCCBPackage.Extensions;
using System;
using System.Threading.Tasks;

namespace OAuth.Api.Extensions
{
    internal static class ServiceCollectionExtension
    {
        private static readonly string _namespaceApplication = $"{nameof(OAuth)}.{nameof(Api)}.{nameof(Application)}";

        public static void AddApplicationServices(this IServiceCollection services, Action<DBApiOptions> options) => services
            .Configure(options)
            .AddMediatRHandlers()
            .AddFluentValidators()
            .AddIntergationServices()
            .AddScoped<ITokenService, TokenService>()
            .AddSingleton<IPasswordService, PasswordService>();

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
