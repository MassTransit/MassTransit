namespace MassTransit.EventHubIntegration
{
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;


    public interface ProcessorClientBuilderContext
    {
        Task OnPartitionInitializing(PartitionInitializingEventArgs eventArgs);
        Task OnPartitionClosing(PartitionClosingEventArgs eventArgs);
    }
}
