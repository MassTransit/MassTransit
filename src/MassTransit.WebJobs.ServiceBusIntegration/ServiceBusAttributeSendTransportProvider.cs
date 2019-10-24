namespace MassTransit.WebJobs.ServiceBusIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.ServiceBus.Core.Contexts;
    using Azure.ServiceBus.Core.Transport;
    using Context;
    using Contexts;
    using Logging;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.ServiceBus;
    using Transports;


    public class ServiceBusAttributeSendTransportProvider :
        ISendTransportProvider
    {
        readonly IBinder _binder;
        readonly CancellationToken _cancellationToken;

        public ServiceBusAttributeSendTransportProvider(IBinder binder, CancellationToken cancellationToken)
        {
            _binder = binder;
            _cancellationToken = cancellationToken;
        }

        public async Task<ISendTransport> GetSendTransport(Uri address)
        {
            var queueOrTopicName = address.AbsolutePath.Trim('/');

            var serviceBusQueue = new ServiceBusAttribute(queueOrTopicName, EntityType.Queue);

            IAsyncCollector<Message> collector = await _binder.BindAsync<IAsyncCollector<Message>>(serviceBusQueue, _cancellationToken).ConfigureAwait(false);

            LogContext.Debug?.Log("Creating Send Transport: {Queue}", queueOrTopicName);

            var sendEndpointContext = new CollectorMessageSendEndpointContext(queueOrTopicName, collector, _cancellationToken);

            var source = new CollectorSendEndpointContextSupervisor(sendEndpointContext);

            var transportContext = new HostServiceBusSendTransportContext(address, source, LogContext.Current.CreateLogContext(LogCategoryName.Transport.Send));

            return new ServiceBusSendTransport(transportContext);
        }

        public Uri NormalizeAddress(Uri address)
        {
            return address;
        }
    }
}
