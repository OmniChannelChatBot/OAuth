using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OAuth.Api.Extensions;
using OAuth.Api.Middlewares;
using OAuth.Core.Options;

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
            services.AddHealthCheckServices();
            services.AddJwtBearerAuthentication(
                options => Configuration.GetSection(nameof(AccessTokenOptions)).Bind(options),
                options => Configuration.GetSection(nameof(RefreshTokenOptions)).Bind(options));
            services.AddAutoMapper(typeof(Startup));
            services.AddApplicationServices(options => Configuration.GetSection(nameof(DBApiOptions)).Bind(options));
            services.AddApiServices();
            services.AddCustomSwagger();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<ApiExceptionMiddleware>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCustomHealthChecks();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.UseCustomSwagger();
        }
    }
}
