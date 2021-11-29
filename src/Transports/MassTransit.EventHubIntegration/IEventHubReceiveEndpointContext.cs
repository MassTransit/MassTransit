namespace MassTransit
{
    using EventHubIntegration;
    using Transports;


    public interface IEventHubReceiveEndpointContext :
        ReceiveEndpointContext
    {
        IProcessorContextSupervisor ContextSupervisor { get; }
    }
}
