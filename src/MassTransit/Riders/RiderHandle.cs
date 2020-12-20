namespace MassTransit.Riders
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface RiderHandle
    {
        Task<bool> Started { get; }
        Task StopAsync(CancellationToken cancellationToken);
    }
}
