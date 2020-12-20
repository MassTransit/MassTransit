namespace MassTransit.EventHubIntegration
{
    using Context;
    using Contexts;


    public interface IEventHubReceiveEndpointContext :
        ReceiveEndpointContext
    {
        IProcessorContextSupervisor ContextSupervisor { get; }
    }
}
