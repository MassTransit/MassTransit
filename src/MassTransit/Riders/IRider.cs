namespace MassTransit.Riders
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface IRider
    {
        Task Start(CancellationToken cancellationToken = default);
        Task Stop(CancellationToken cancellationToken = default);
    }
}
