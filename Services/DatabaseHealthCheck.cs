using Level5Backend.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Level5Backend.Services
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly Level5Context _context;

        public DatabaseHealthCheck(Level5Context context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return await _context.Database.CanConnectAsync(cancellationToken)
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("Cannot connect to the database.");
        }
    }
}
