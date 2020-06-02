namespace MassTransit.EventHubIntegration.Contexts
{
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;


    public interface IProcessorLockContext
    {
        Task Complete(ProcessEventArgs eventArgs, CancellationToken cancellationToken);
        Task OnPartitionInitializing(PartitionInitializingEventArgs eventArgs);
        Task OnPartitionClosing(PartitionClosingEventArgs eventArgs);
    }
}
