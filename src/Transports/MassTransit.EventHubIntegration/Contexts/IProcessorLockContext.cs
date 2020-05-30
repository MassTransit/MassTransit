namespace MassTransit.EventHubIntegration.Contexts
{
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;


    public interface IProcessorLockContext
    {
        Task Complete(ProcessEventArgs eventArgs, CancellationToken cancellationToken);
    }


    public class ProcessorLockContext :
        IProcessorLockContext
    {
        public Task Complete(ProcessEventArgs eventArgs, CancellationToken cancellationToken)
        {
            //TODO: use batching here, this operation is slow as hell
            return eventArgs.UpdateCheckpointAsync(cancellationToken);
        }
    }
}
