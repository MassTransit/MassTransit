namespace MassTransit.KafkaIntegration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Registration;
    using Registration.Attachments;


    public class KafkaBusAttachment :
        IBusAttachment
    {
        readonly IEnumerable<IKafkaSubscription> _subscriptions;

        public KafkaBusAttachment(IEnumerable<IKafkaSubscription> subscriptions)
        {
            Name = "confluent.kafka";
            _subscriptions = subscriptions;
        }

        public string Name { get; }

        public async ValueTask Connect(CancellationToken cancellationToken)
        {
            if (_subscriptions == null || _subscriptions.Any())
                return;
            await Task.WhenAll(_subscriptions.Select(subscription => subscription.Subscribe(cancellationToken)));
        }

        public async ValueTask Disconnect(CancellationToken cancellationToken)
        {
            if (_subscriptions == null || _subscriptions.Any())
                return;
            await Task.WhenAll(_subscriptions.Select(subscription => subscription.Unsubscribe(cancellationToken)));
        }
    }


    public static class KafkaBusAttachmentExtensions
    {
        public static void ConnectKafka(this IBusInstance busInstance, params IKafkaSubscription[] subscriptions)
        {
            busInstance.Connect(new KafkaBusAttachment(subscriptions));
        }
    }
}
