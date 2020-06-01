namespace MassTransit.KafkaIntegration.Transport
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface IKafkaReceiveEndpoint :
        IReceiveEndpoint
    {
        Task Connect(CancellationToken cancellationToken);
        Task Disconnect(CancellationToken cancellationToken);
    }
}
