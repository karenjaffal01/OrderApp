using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class DownStreamHealthChecks : IHealthCheck
{
    private readonly string[] _serviceUrls = new[]
    {
        "http://localhost:7001/health/ready",
        "http://localhost:7002/health/ready",
        "http://localhost:7003/health/ready",
        "http://localhost:7004/health/ready"
    };

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        using var client = new HttpClient();
        foreach (var url in _serviceUrls)
        {
            try
            {
                var response = await client.GetAsync(url, cancellationToken);
                if (!response.IsSuccessStatusCode)
                    return HealthCheckResult.Unhealthy($"Downstream service failed: {url}");
            }
            catch
            {
                return HealthCheckResult.Unhealthy($"Downstream service unreachable: {url}");
            }
        }

        return HealthCheckResult.Healthy("All downstream services are healthy");
    }
}

