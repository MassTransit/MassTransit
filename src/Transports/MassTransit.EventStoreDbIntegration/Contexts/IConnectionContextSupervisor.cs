using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public interface IConnectionContextSupervisor :
        ITransportSupervisor<ConnectionContext>
    {
    }
}
