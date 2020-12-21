namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Azure.Storage.Blobs;
    using Configuration;
    using Context;
    using Contexts;
    using GreenPipes.Agents;
    using MassTransit.Registration;
    using Util;


    public class EventHubReceiveEndpointContext :
        BaseReceiveEndpointContext,
        IEventHubReceiveEndpointContext
    {
        readonly IBusInstance _busInstance;
        readonly Recycle<IEventHubProcessorContextSupervisor> _contextSupervisor;

        public EventHubReceiveEndpointContext(IBusInstance busInstance, IReceiveEndpointConfiguration endpointConfiguration,
            ReceiveSettings receiveSettings, Func<BlobContainerClient> blobContainerClientFactory,
            Func<BlobContainerClient, EventProcessorClient> clientFactory,
            Func<PartitionClosingEventArgs, Task> partitionClosingHandler,
            Func<PartitionInitializingEventArgs, Task> partitionInitializingHandler)
            : base(busInstance.HostConfiguration, endpointConfiguration)
        {
            _busInstance = busInstance;
            _contextSupervisor = new Recycle<IEventHubProcessorContextSupervisor>(() =>
                new EventHubProcessorContextSupervisor(busInstance.HostConfiguration.Agent, LogContext, receiveSettings, blobContainerClientFactory,
                    clientFactory,
                    partitionClosingHandler, partitionInitializingHandler));
        }

        public override void AddAgent(IAgent agent)
        {
            _contextSupervisor.Supervisor.AddAgent(agent);
        }

        public override Exception ConvertException(Exception exception, string message)
        {
            return exception;
        }

        public IEventHubProcessorContextSupervisor ContextSupervisor => _contextSupervisor.Supervisor;

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
