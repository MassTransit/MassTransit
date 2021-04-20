using MassTransit.Context;
using MassTransit.EventStoreDbIntegration.Contexts;

namespace MassTransit.EventStoreDbIntegration
{
    public interface IEventStoreDbSubscriptionContext :
        ReceiveEndpointContext
    {
        ISubscriptionContextSupervisor ContextSupervisor { get; }
    }
}
