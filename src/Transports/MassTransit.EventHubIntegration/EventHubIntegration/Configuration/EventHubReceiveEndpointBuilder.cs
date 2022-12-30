namespace MassTransit.EventHubIntegration.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using MassTransit.Configuration;
    using Transports;


    public class EventHubReceiveEndpointBuilder :
        ReceiveEndpointBuilder
    {
        readonly IBusInstance _busInstance;
        readonly Func<EventProcessorClient> _clientFactory;
        readonly IReceiveEndpointConfiguration _configuration;
        readonly IEventHubHostConfiguration _hostConfiguration;
        readonly Func<PartitionClosingEventArgs, Task> _partitionClosingHandler;
        readonly Func<PartitionInitializingEventArgs, Task> _partitionInitializingHandler;
        readonly ReceiveSettings _receiveSettings;

        public EventHubReceiveEndpointBuilder(IEventHubHostConfiguration hostConfiguration, IBusInstance busInstance,
            IReceiveEndpointConfiguration configuration, ReceiveSettings receiveSettings,
            Func<EventProcessorClient> clientFactory,
            Func<PartitionClosingEventArgs, Task> partitionClosingHandler,
            Func<PartitionInitializingEventArgs, Task> partitionInitializingHandler)
            : base(configuration)
        {
            _hostConfiguration = hostConfiguration;
            _busInstance = busInstance;
            _configuration = configuration;
            _receiveSettings = receiveSettings;
            _clientFactory = clientFactory;
            _partitionClosingHandler = partitionClosingHandler;
            _partitionInitializingHandler = partitionInitializingHandler;
        }

        public IEventHubReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var context = new EventHubReceiveEndpointContext(_hostConfiguration, _busInstance, _configuration,
                _clientFactory, _partitionClosingHandler, _partitionInitializingHandler);

            context.GetOrAddPayload(() => _busInstance.HostConfiguration.Topology);
            context.AddOrUpdatePayload(() => _receiveSettings, _ => _receiveSettings);

            return context;
        }
    }
}
