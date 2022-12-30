namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Logging;


    public interface ProcessorContext :
        PipeContext
    {
        ILogContext? LogContext { get; }
        event Func<ProcessErrorEventArgs, Task>? ProcessError;

        EventProcessorClient GetClient(ProcessorClientBuilderContext context, Func<ProcessErrorEventArgs, Task> errorHandler);
    }
}
