using MassTransit.Context;
using MassTransit.EventStoreDbIntegration.Contexts;

namespace MassTransit.EventStoreDbIntegration
{
    public interface IEventStoreDbReceiveEndpointContext :
        ReceiveEndpointContext
    {
        IProcessorContextSupervisor ContextSupervisor { get; }
    }
}
