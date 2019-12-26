using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OAuth.Api.HealthChecks
{
    public class MemoryHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            const long threshold = 1 * 1024 * 1024 * 1024;

            var allocated = GC.GetTotalMemory(false);
            var data = new Dictionary<string, object>
            {
                { "AllocatedBytes", allocated },
                { "AllocatedKBytes", allocated / 1024 },
                { "AllocatedMBytes", allocated / 1024 / 1024 },
                { "AllocatedGBytes", allocated / 1024 / 1024 / 1024 },
                { "Gen0Collections", GC.CollectionCount(0) },
                { "Gen1Collections", GC.CollectionCount(1) },
                { "Gen2Collections", GC.CollectionCount(2) }
            };

            var status = allocated < threshold ? HealthStatus.Healthy : HealthStatus.Degraded;

            return Task.FromResult(new HealthCheckResult(
                status,
                $"Reports degraded status if allocated bytes >= {threshold} bytes.",
                null,
                data));
        }
    }
}
