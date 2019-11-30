using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using OAuth.Controllers.Filters;

namespace OAuth.Extensions
{
    internal static class ServiceCollectionExtension
    {
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
