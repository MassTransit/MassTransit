namespace MassTransit.KafkaIntegration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Riders;
    using Subscriptions;
    using Util;


    public class KafkaRider :
        BaseRider
    {
        readonly IEnumerable<IKafkaReceiveEndpoint> _endpoints;

        public KafkaRider(IEnumerable<IKafkaReceiveEndpoint> endpoints, RiderObservable observers)
            : base("confluent.kafka", observers)
        {
            _endpoints = endpoints;
        }

        protected override Task BaseStart(CancellationToken cancellationToken)
        {
            if (_endpoints == null || !_endpoints.Any())
                return TaskUtil.Completed;

            return Task.WhenAll(_endpoints.Select(endpoint => endpoint.Connect(cancellationToken)));
        }

        protected override Task BaseStop(CancellationToken cancellationToken)
        {
            if (_endpoints == null || !_endpoints.Any())
                return TaskUtil.Completed;

            return Task.WhenAll(_endpoints.Select(endpoint => endpoint.Disconnect(cancellationToken)));
        }
    }
}
