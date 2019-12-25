using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OAuth.Api.Extensions;
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
            services.Configure<DBApiOptions>(options =>
            {
                options.Url = Configuration.GetSection(nameof(DBApiOptions)).Value;
            });

            services.AddHealthChecks();
            services.AddApiServices();
            services.AddApplicationServices();
            services.AddJwtBearerAuthentication(options => Configuration.GetSection(nameof(SecurityTokenOptions)).Bind(options));
            services.AddAutoMapper(typeof(Startup));
            services.AddHttpClient();
            services.AddCustomSwagger();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });

            app.UseCustomSwagger();
        }
    }
}
