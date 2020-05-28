namespace MassTransit.KafkaIntegration.Subscriptions
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
