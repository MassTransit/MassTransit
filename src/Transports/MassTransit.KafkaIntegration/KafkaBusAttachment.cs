namespace MassTransit.KafkaIntegration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Registration.Attachments;
    using Subscriptions;


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

        public async Task Connect(CancellationToken cancellationToken)
        {
            if (_subscriptions == null || !_subscriptions.Any())
                return;

            await Task.WhenAll(_subscriptions.Select(subscription => subscription.Subscribe(cancellationToken))).ConfigureAwait(false);
        }

        public async Task Disconnect(CancellationToken cancellationToken)
        {
            if (_subscriptions == null || !_subscriptions.Any())
                return;

            await Task.WhenAll(_subscriptions.Select(subscription => subscription.Unsubscribe(cancellationToken))).ConfigureAwait(false);
        }
    }
}
