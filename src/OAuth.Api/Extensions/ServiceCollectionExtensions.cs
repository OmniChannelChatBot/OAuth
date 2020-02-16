using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OAuth.Core.Interfaces;
using OAuth.Core.Options;
using OAuth.Core.Services;
using OAuth.Infrastructure.Services;
using System;

namespace OAuth.Api.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        private static readonly string _namespaceApplication = $"{nameof(OAuth)}.{nameof(Api)}.{nameof(Application)}";

        public static void AddApplicationServices(this IServiceCollection services, Action<DBApiOptions> actionDBbApiOptions) => services
            .Configure(actionDBbApiOptions)
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
