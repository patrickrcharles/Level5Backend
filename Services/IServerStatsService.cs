namespace Level5Backend.Services
{
    public interface IServerStatsService
    {
        Task RecomputeAsync(CancellationToken cancellationToken = default);
    }
}
