using MassTransit.Registration;
using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class ConnectionContextSupervisor :
        TransportPipeContextSupervisor<ConnectionContext>,
        IConnectionContextSupervisor
    {
        public ConnectionContextSupervisor(IConfigurationServiceProvider provider)
            : base(new ConnectionContextFactory(provider))
        {
        }
    }
}
