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


    public class ServiceBusAttributePublishTransportProvider :
        IPublishTransportProvider
    {
        readonly IBinder _binder;
        readonly CancellationToken _cancellationToken;

        public ServiceBusAttributePublishTransportProvider(IBinder binder, CancellationToken cancellationToken)
        {
            _binder = binder;
            _cancellationToken = cancellationToken;
        }

        Task<ISendTransport> IPublishTransportProvider.GetPublishTransport<T>(Uri publishAddress)
        {
            return GetSendTransport(publishAddress);
        }

        async Task<ISendTransport> GetSendTransport(Uri address)
        {
            var queueOrTopicName = address.AbsolutePath.Trim('/');

            var serviceBusTopic = new ServiceBusAttribute(queueOrTopicName, EntityType.Topic);

            IAsyncCollector<Message> collector = await _binder.BindAsync<IAsyncCollector<Message>>(serviceBusTopic, _cancellationToken).ConfigureAwait(false);

            LogContext.Debug?.Log("Creating Publish Transport: {Topic}", queueOrTopicName);

            var client = new CollectorMessageSendEndpointContext(queueOrTopicName, collector, _cancellationToken);

            var source = new CollectorSendEndpointContextSupervisor(client);

            var transportContext = new HostServiceBusSendTransportContext(address, source, LogContext.Current.CreateLogContext(LogCategoryName.Transport.Send));

            var transport = new ServiceBusSendTransport(transportContext);

            return transport;
        }
    }
}
