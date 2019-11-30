using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OAuth.Extensions;
using OAuth.Options;

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
                options.Url
                    = Configuration.GetSection(nameof(AppOptions)).Value;
            });

            services.AddHealthChecks();
            services.AddApi();
            services.AddApplication();
            services.AddJwtBearerAuthentication(options => Configuration.GetSection(nameof(AppOptions)).Bind(options));
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
