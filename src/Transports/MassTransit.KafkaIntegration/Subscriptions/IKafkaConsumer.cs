namespace MassTransit.KafkaIntegration.Subscriptions
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface IKafkaConsumer
    {
        Task Subscribe(CancellationToken cancellationToken);
        Task Unsubscribe(CancellationToken cancellationToken);
    }
}
