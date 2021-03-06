using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OAuth.Api.Extensions;
using OAuth.Api.Middlewares;
using OAuth.Core.Options;
using OCCBPackage.Extensions;
using OCCBPackage.Options;
using OCCBPackage.Swagger.OperationFilters;

namespace OAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddMediatR();
            services.AddAutoMapper(typeof(Startup));
            services.AddCustomHealthChecks();
            services.AddJwtBearerAuthentication(
                options => Configuration.GetSection(nameof(AccessTokenOptions)).Bind(options),
                options => Configuration.GetSection(nameof(RefreshTokenOptions)).Bind(options));
            services.AddApplicationServices(options => Configuration.GetSection(nameof(DBApiOptions)).Bind(options));
            services.AddApiServices();
            services.AddCorsPolicy(options => Configuration.GetSection(nameof(CorsPolicyOptions)).Bind(options));
            services.AddCookiePolicy(options => Configuration.GetSection(nameof(CookiePolicyOptions)).Bind(options));
            services.AddCustomSwagger(o =>
            {
                o.AddBearerSecurityDefinition();
                o.OperationFilter<OperationApiProblemDetailsFilter>(
                    new int[] { 504, 503, 502, 501, 500, 415, 413, 412, 405, 400 });
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<ApiExceptionMiddleware>();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseCors(CorsPolicyOptions.CorsPolicy);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCustomHealthChecks();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.UseCustomSwagger();
        }
    }
}
