namespace MassTransit.EventHubIntegration.Contexts
{
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using GreenPipes;


    public interface IEventHubProcessorContext :
        PipeContext
    {
        IHostSettings HostSettings { get; }
        IStorageSettings StorageSettings { get; }
        ReceiveSettings ReceiveSettings { get; }

        Task InitializePartition(PartitionInitializingEventArgs eventArgs);
        Task ClosePartition(PartitionClosingEventArgs eventArgs);

        Task<bool> TryCreateContainerIfNotExists(CancellationToken cancellationToken);

        EventProcessorClient CreateClient();
    }
}
