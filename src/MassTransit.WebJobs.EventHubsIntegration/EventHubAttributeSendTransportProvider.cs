namespace MassTransit.WebJobs.EventHubsIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Microsoft.Azure.EventHubs;
    using Microsoft.Azure.WebJobs;
    using Transports;


    public class EventHubAttributeSendTransportProvider :
        ISendTransportProvider
    {
        readonly IBinder _binder;
        readonly CancellationToken _cancellationToken;

        public EventHubAttributeSendTransportProvider(IBinder binder, CancellationToken cancellationToken)
        {
            _binder = binder;
            _cancellationToken = cancellationToken;
        }

        async Task<ISendTransport> ISendTransportProvider.GetSendTransport(Uri address)
        {
            var eventHubName = address.AbsolutePath.Trim('/');

            var attribute = new EventHubAttribute(eventHubName);

            IAsyncCollector<EventData> collector = await _binder.BindAsync<IAsyncCollector<EventData>>(attribute, _cancellationToken).ConfigureAwait(false);

            var client = new CollectorEventDataSendEndpointContext(eventHubName, collector, _cancellationToken);

            var source = new CollectorEventDataSendEndpointContextSource(client);

            var transport = new EventHubSendTransport(source, address);

            return transport;
        }

        public Uri NormalizeAddress(Uri address)
        {
            return address;
        }
    }
}
