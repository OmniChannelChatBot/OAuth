using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OAuth.Api.HealthChecks;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;

namespace OAuth.Api.Extensions
{
    public static class HealthCheckExtension
    {
        public static IHealthChecksBuilder AddCustomHealthChecks(this IServiceCollection services)
        {
            return services
                .AddHealthChecks()
                .AddCheck<MemoryHealthCheck>(
                    "memory-check",
                    HealthStatus.Degraded,
                    new[] { "memory" });
        }

        public static IApplicationBuilder UseCustomHealthChecks(this IApplicationBuilder app)
        {
            app
                .UseHealthChecks("/health")
                .UseHealthChecks(
                    "/health/detail",
                    new HealthCheckOptions
                    {
                        Predicate = _ => true,
                        AllowCachingResponses = false,
                        ResponseWriter = (httpContext, healthReport) =>
                        {
                            httpContext.Response.ContentType = MediaTypeNames.Application.Json;

                            var @object = new
                            {
                                Status = healthReport.Status.ToString(),
                                Entries = healthReport.Entries.ToDictionary(pair => pair.Key, r => new
                                {
                                    Status = r.Value.Status.ToString(),
                                    r.Value.Description,
                                    Data = r.Value.Data.ToDictionary(s => s.Key, e => e.Value)
                                })
                            };

                            var json = JsonSerializer.Serialize(@object, new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                WriteIndented = true
                            });

                            return httpContext.Response.WriteAsync(json);
                        }
                    })
                .UseHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("ready"),
                    AllowCachingResponses = false
                })
                .UseHealthChecks("/health/live", new HealthCheckOptions
                {
                    Predicate = _ => false,
                    AllowCachingResponses = false
                });

            return app;
        }
    }
}
