namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;
    using Util;


    public interface IProcessorLockContext :
        IChannelExecutorPool<ProcessEventArgs>
    {
        Task Pending(ProcessEventArgs eventArgs);
        Task Faulted(ProcessEventArgs eventArgs, Exception exception);
        Task Complete(ProcessEventArgs eventArgs);
    }
}
