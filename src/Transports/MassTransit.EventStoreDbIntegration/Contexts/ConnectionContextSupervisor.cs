using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class ConnectionContextSupervisor :
        TransportPipeContextSupervisor<ConnectionContext>,
        IConnectionContextSupervisor
    {
        public ConnectionContextSupervisor(IHostSettings hostSettings)
            : base(new ConnectionContextFactory(hostSettings))
        {
        }
    }
}
