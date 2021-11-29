namespace MassTransit.Transports
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface HostRiderHandle
    {
        IRider Rider { get; }

        Task<RiderReady> Ready { get; }

        Task StopAsync(CancellationToken cancellationToken = default);

        Task StopAsync(bool remove, CancellationToken cancellationToken = default);
    }
}
