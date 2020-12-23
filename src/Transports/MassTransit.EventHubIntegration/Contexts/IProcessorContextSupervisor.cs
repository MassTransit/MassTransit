namespace MassTransit.EventHubIntegration.Contexts
{
    using Transports;


    public interface IProcessorContextSupervisor :
        ITransportSupervisor<ProcessorContext>
    {
    }
}
