using Microsoft.Extensions.Hosting;

namespace Level5Backend.Services
{
    // Runs stats recomputation off the request path on a fixed interval instead of inline on every
    // highscore POST. Hosted services are singletons, so a scope is created per tick to obtain a
    // request-scoped Level5Context.
    public class ServerStatsBackgroundService : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(10);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ServerStatsBackgroundService> _logger;

        public ServerStatsBackgroundService(IServiceScopeFactory scopeFactory, ILogger<ServerStatsBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);
            do
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var statsService = scope.ServiceProvider.GetRequiredService<IServerStatsService>();
                    await statsService.RecomputeAsync(stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Failed to recompute server stats");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
    }
}
