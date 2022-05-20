namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Azure.Storage.Blobs;
    using Configuration;
    using MassTransit.Configuration;
    using Transports;
    using Util;


    public class EventHubReceiveEndpointContext :
        BaseReceiveEndpointContext,
        IEventHubReceiveEndpointContext
    {
        readonly IBusInstance _busInstance;
        readonly Recycle<IProcessorContextSupervisor> _contextSupervisor;

        public EventHubReceiveEndpointContext(IEventHubHostConfiguration hostConfiguration, IBusInstance busInstance,
            IReceiveEndpointConfiguration endpointConfiguration, ReceiveSettings receiveSettings,
            Func<IStorageSettings, BlobContainerClient> blobContainerClientFactory,
            Func<IHostSettings, BlobContainerClient, EventProcessorClient> clientFactory,
            Func<PartitionClosingEventArgs, Task> partitionClosingHandler,
            Func<PartitionInitializingEventArgs, Task> partitionInitializingHandler)
            : base(busInstance.HostConfiguration, endpointConfiguration)
        {
            _busInstance = busInstance;
            _contextSupervisor = new Recycle<IProcessorContextSupervisor>(() =>
                new ProcessorContextSupervisor(hostConfiguration.ConnectionContextSupervisor, busInstance.HostConfiguration, receiveSettings,
                    blobContainerClientFactory, clientFactory, partitionClosingHandler, partitionInitializingHandler));
        }

        public override void AddSendAgent(IAgent agent)
        {
            _contextSupervisor.Supervisor.AddSendAgent(agent);
        }

        public override void AddConsumeAgent(IAgent agent)
        {
            _contextSupervisor.Supervisor.AddConsumeAgent(agent);
        }

        public override Exception ConvertException(Exception exception, string message)
        {
            return new EventHubConnectionException(message, exception);
        }

        public IProcessorContextSupervisor ContextSupervisor => _contextSupervisor.Supervisor;

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            throw new NotSupportedException();
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            throw new NotSupportedException();
        }

        protected override IPublishEndpointProvider CreatePublishEndpointProvider()
        {
            return _busInstance.Bus;
        }

        protected override ISendEndpointProvider CreateSendEndpointProvider()
        {
            return _busInstance.Bus;
        }
    }
}
