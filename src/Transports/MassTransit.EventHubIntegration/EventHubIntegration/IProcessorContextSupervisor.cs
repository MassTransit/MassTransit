namespace MassTransit.EventHubIntegration
{
    using Transports;


    public interface IProcessorContextSupervisor :
        ITransportSupervisor<ProcessorContext>
    {
    }
}
