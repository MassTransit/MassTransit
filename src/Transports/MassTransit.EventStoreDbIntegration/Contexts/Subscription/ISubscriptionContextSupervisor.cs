using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public interface ISubscriptionContextSupervisor :
        ITransportSupervisor<SubscriptionContext>
    {
    }
}
