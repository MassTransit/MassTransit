namespace MassTransit.KafkaIntegration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Attachments;
    using Subscriptions;
    using Util;


    public class KafkaBaseBusAttachment :
        BaseBusAttachment
    {
        public const string InstanceName = "confluent.kafka";
        readonly IEnumerable<IKafkaReceiveEndpoint> _endpoints;

        public KafkaBaseBusAttachment(IEnumerable<IKafkaReceiveEndpoint> endpoints, BusAttachmentObservable observers)
            : base(InstanceName, observers)
        {
            _endpoints = endpoints;
        }

        protected override Task BaseConnect(CancellationToken cancellationToken)
        {
            if (_endpoints == null || !_endpoints.Any())
                return TaskUtil.Completed;

            return Task.WhenAll(_endpoints.Select(consumer => consumer.Connect(cancellationToken)));
        }

        protected override Task BaseDisconnect(CancellationToken cancellationToken)
        {
            if (_endpoints == null || !_endpoints.Any())
                return TaskUtil.Completed;

            return Task.WhenAll(_endpoints.Select(consumer => consumer.Disconnect(cancellationToken)));
        }
    }
}
