namespace MassTransit.KafkaIntegration.Subscriptions
{
    using System.Threading;
    using System.Threading.Tasks;


    public interface IKafkaSubscription
    {
        Task Subscribe(CancellationToken cancellationToken);
        Task Unsubscribe(CancellationToken cancellationToken);
    }
}
