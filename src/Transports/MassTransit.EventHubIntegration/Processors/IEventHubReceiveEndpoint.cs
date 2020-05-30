namespace MassTransit.EventHubIntegration.Processors
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface IEventHubReceiveEndpoint :
        IReceiveEndpoint
    {
        Task Connect(CancellationToken cancellationToken);
        Task Disconnect(CancellationToken cancellationToken);
    }
}
