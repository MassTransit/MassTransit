namespace MassTransit.EventHubIntegration.Transport
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Riders;
    using Util;


    public class EventHubRider :
        BaseRider
    {
        readonly IEnumerable<IEventHubReceiveEndpoint> _endpoints;

        public EventHubRider(IEnumerable<IEventHubReceiveEndpoint> endpoints, RiderObservable observers)
            : base("azure.event-hub", observers)
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
