namespace MassTransit.Riders
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface IRider
    {
        string Name { get; }
        Task Start(CancellationToken cancellationToken);
        Task Stop(CancellationToken cancellationToken);
    }
}
