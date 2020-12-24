namespace MassTransit.Riders
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface RiderHandle
    {
        Task Ready { get; }
        Task StopAsync(CancellationToken cancellationToken);
    }
}
