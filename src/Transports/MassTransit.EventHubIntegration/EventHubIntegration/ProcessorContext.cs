namespace MassTransit.EventHubIntegration
{
    using Azure.Messaging.EventHubs;
    using Logging;


    public interface ProcessorContext :
        PipeContext
    {
        ILogContext LogContext { get; }
        EventProcessorClient GetClient(ProcessorClientBuilderContext context);
    }
}
