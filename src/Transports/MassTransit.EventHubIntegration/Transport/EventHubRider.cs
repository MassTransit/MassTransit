namespace MassTransit.EventHubIntegration
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Pipeline.Observables;
    using Riders;
    using Util;


    public class EventHubRider :
        BaseRider,
        IEventHubRider
    {
        readonly IEnumerable<IEventHubReceiveEndpoint> _endpoints;
        readonly IEventHubProducerSharedContext _producerSharedContext;

        public EventHubRider(IEnumerable<IEventHubReceiveEndpoint> endpoints, IEventHubProducerSharedContext producerSharedContext, RiderObservable observers)
            : base("azure.event-hub")
        {
            _endpoints = endpoints;
            _producerSharedContext = producerSharedContext;
        }

        public IEventHubProducerProvider GetProducerProvider(ConsumeContext consumeContext = default)
        {
            return new EventHubProducerProvider(_producerSharedContext, consumeContext);
        }
        //
        // protected override Task StartRider(CancellationToken cancellationToken)
        // {
        //     if (_endpoints == null || !_endpoints.Any())
        //         return TaskUtil.Completed;
        //
        //     return Task.WhenAll(_endpoints.Select(endpoint => endpoint.Connect(cancellationToken)));
        // }
        //
        // protected override async Task StopRider(CancellationToken cancellationToken)
        // {
        //     await _producerSharedContext.DisposeAsync().ConfigureAwait(false);
        //
        //     if (_endpoints == null || !_endpoints.Any())
        //         return;
        //
        //     await Task.WhenAll(_endpoints.Select(endpoint => endpoint.Disconnect(cancellationToken))).ConfigureAwait(false);
        // }

        protected override void AddReceiveEndpoint(IHost cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
