namespace MassTransit.EventHubIntegration
{
    using Context;
    using Contexts;


    public interface IEventHubReceiveEndpointContext :
        ReceiveEndpointContext
    {
        IEventHubProcessorContextSupervisor ContextSupervisor { get; }
    }
}
