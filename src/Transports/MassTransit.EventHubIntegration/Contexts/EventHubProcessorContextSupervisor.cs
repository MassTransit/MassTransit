namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using GreenPipes.Agents;
    using Transports;


    public class EventHubProcessorContextSupervisor :
        TransportPipeContextSupervisor<IEventHubProcessorContext>,
        IEventHubProcessorContextSupervisor
    {
        public EventHubProcessorContextSupervisor(IAgent supervisor, IHostSettings hostSettings, IStorageSettings storageSettings,
            ReceiveSettings receiveSettings,
            Action<EventProcessorClientOptions> configureOptions, Func<PartitionClosingEventArgs, Task> partitionClosingHandler,
            Func<PartitionInitializingEventArgs, Task> partitionInitializingHandler)
            : base(supervisor,
                new EventHubProcessorContextFactory(hostSettings, storageSettings, receiveSettings, configureOptions, partitionClosingHandler,
                    partitionInitializingHandler))
        {
        }
    }
}
