namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Util;


    public interface ProcessorContext :
        PipeContext,
        IChannelExecutorPool<ProcessEventArgs>,
        IProcessorLockContext
    {
        event Func<ProcessErrorEventArgs, Task> ProcessError;
        EventProcessorClient CreateClient(Func<ProcessErrorEventArgs, Task> onError);
    }
}
