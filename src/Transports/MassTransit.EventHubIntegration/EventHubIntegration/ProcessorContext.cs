namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;


    public interface ProcessorContext :
        PipeContext,
        IProcessorLockContext
    {
        ReceiveSettings ReceiveSettings { get; }
        event Func<ProcessEventArgs, Task> ProcessEvent;
        event Func<ProcessErrorEventArgs, Task> ProcessError;

        Task<bool> CreateBlobIfNotExistsAsync(CancellationToken cancellationToken = default);

        Task StartProcessingAsync(CancellationToken cancellationToken = default);
        Task StopProcessingAsync(CancellationToken cancellationToken = default);
    }
}
