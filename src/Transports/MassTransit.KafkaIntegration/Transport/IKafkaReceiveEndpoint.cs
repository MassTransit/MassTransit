namespace MassTransit.KafkaIntegration.Transport
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface IKafkaReceiveEndpoint :
        IReceiveEndpoint
    {
        Task Start(CancellationToken cancellationToken);

        Task Stop();
    }
}
