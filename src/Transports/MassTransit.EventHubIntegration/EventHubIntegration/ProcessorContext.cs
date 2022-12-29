namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;


    public interface ProcessorContext :
        PipeContext,
        IProcessorLockContext
    {
        event Func<ProcessErrorEventArgs, Task> ProcessError;
        EventProcessorClient GetClient(Func<ProcessErrorEventArgs, Task> onError);
    }
}
