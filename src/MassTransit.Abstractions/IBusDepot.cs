namespace MassTransit
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface IBusDepot
    {
        Task Start(CancellationToken cancellationToken);

        Task Stop(CancellationToken cancellationToken);
    }
}
