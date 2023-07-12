namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;


    public interface IProcessorLockContext :
        IAsyncDisposable
    {
        Task Pending(ProcessEventArgs eventArgs);
        Task Complete(ProcessEventArgs eventArgs);
        Task Faulted(ProcessEventArgs eventArgs, Exception exception);
        void Canceled(ProcessEventArgs eventArgs, CancellationToken cancellationToken);
    }
}
