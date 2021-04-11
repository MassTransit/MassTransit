using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public interface IClientContextSupervisor :
        ITransportSupervisor<ClientContext>
    {
    }
}
