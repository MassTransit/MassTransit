namespace MassTransit.KafkaIntegration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Registration.Attachments;
    using Subscriptions;
    using Util;


    public class KafkaBusAttachment :
        IBusAttachment
    {
        readonly IEnumerable<IKafkaConsumer> _consumers;

        public KafkaBusAttachment(IEnumerable<IKafkaConsumer> consumers)
        {
            Name = "confluent.kafka";
            _consumers = consumers;
        }

        public string Name { get; }

        public Task Connect(CancellationToken cancellationToken)
        {
            if (_consumers == null || !_consumers.Any())
                return TaskUtil.Completed;

            return Task.WhenAll(_consumers.Select(consumer => consumer.Subscribe(cancellationToken)));
        }

        public Task Disconnect(CancellationToken cancellationToken)
        {
            if (_consumers == null || !_consumers.Any())
                return TaskUtil.Completed;

            return Task.WhenAll(_consumers.Select(consumer => consumer.Unsubscribe(cancellationToken)));
        }
    }
}
