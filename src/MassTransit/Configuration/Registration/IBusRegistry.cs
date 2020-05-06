namespace MassTransit.Registration
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface IBusRegistry
    {
        Task Start(CancellationToken cancellationToken);

        Task Stop(CancellationToken cancellationToken);
    }
}
