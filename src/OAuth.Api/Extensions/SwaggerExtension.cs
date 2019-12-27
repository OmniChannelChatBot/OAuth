using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OAuth.Api.Models;
using OAuth.Api.Swagger.OperationFilters;

namespace OAuth.Api.Extensions
{
    internal static class SwaggerExtension
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
                 c.OperationFilter<OperationApiProblemDetailsFilter>(
                        new int[] { 504, 503, 502, 501, 500, 415, 413, 412, 405, 400 },
                        typeof(ApiProblemDetails));

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
