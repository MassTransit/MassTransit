namespace MassTransit.EventHubIntegration.Contexts
{
    using Transports;


    public interface IEventHubProcessorContextSupervisor :
        ITransportSupervisor<IEventHubProcessorContext>
    {
    }
}
