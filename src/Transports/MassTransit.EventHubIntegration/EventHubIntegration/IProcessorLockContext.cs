namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;


    public interface IProcessorLockContext
    {
        Task Pending(ProcessEventArgs eventArgs);
        Task Faulted(ProcessEventArgs eventArgs, Exception exception);
        Task Complete(ProcessEventArgs eventArgs);
    }
}
