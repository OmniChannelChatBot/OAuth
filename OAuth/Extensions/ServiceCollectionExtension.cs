﻿using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuth.Controllers.Filters;
using OAuth.Options;
using OAuth.Services;
using System;
using System.Text;

namespace OAuth.Extensions
{
    internal static class ServiceCollectionExtension
    {
        public static void AddApplication(this IServiceCollection services) => services
            .AddScoped<IUserService, UserService>();

        public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services, Action<AppOptions> options)
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
                var appOptions = services
                    .BuildServiceProvider()
                    .GetRequiredService<IOptions<AppOptions>>();
                var key = Encoding.UTF8.GetBytes(appOptions.Value.Secret);

                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            return services;
        }
        public static IMvcBuilder AddApi(this IServiceCollection services) => services
            .AddMvcActionFilters()
            .AddControllers(o => o.Filters.AddService<ApiActionFilter>())
                .AddJsonOptions(o => o.JsonSerializerOptions.IgnoreNullValues = true)
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                    options.SuppressMapClientErrors = true;
                })
                .AddFluentValidation();

        private static IServiceCollection AddMvcActionFilters(this IServiceCollection services) => services
            .AddScoped<ApiActionFilter>();
    }
}