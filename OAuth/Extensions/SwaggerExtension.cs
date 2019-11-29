using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OAuth.Swagger.OperationFilters;

namespace OAuth.Extensions
{
    public static class SwaggerExtension
    {
        private const string RoutePrefix = "api-doc";
        private const string DefaultVersion = "v1";

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services) => services
            .AddSwaggerGen(c =>
             {
                 const string Scheme = "Bearer";

                 c.AddSecurityDefinition(Scheme, new OpenApiSecurityScheme
                 {
                     Description = "Bearer authentication",
                     Type = SecuritySchemeType.Http,
                     Scheme = Scheme.ToLower(),
                     BearerFormat = "JWT"
                 });

                 c.SwaggerDoc(nameof(OAuth), new OpenApiInfo
                 {
                     Version = DefaultVersion,
                     Title = nameof(OAuth),
                     Description = $"`{nameof(OAuth)}`",
                 });

                 c.OperationFilter<OperationSecurityDefinitionFilter>(Scheme);

                 c.EnableAnnotations();
             });

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app) => app
            .UseSwagger(options =>
            {
                options.RouteTemplate = $"{RoutePrefix}/{{documentName}}/swagger.json";
            })
            .UseSwaggerUI(c =>
            {
                c.DocumentTitle = $"{nameof(OAuth)} api docs";

                var endpointName = $"{nameof(OAuth)} {DefaultVersion}";

                c.SwaggerEndpoint($"/{RoutePrefix}/{nameof(OAuth)}/swagger.json", endpointName);

                c.RoutePrefix = RoutePrefix;
                c.DisplayRequestDuration();
                c.DisplayOperationId();
            });
    }
}
