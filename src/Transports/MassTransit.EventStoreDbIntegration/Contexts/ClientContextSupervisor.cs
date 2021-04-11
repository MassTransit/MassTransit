using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class ClientContextSupervisor :
        TransportPipeContextSupervisor<ClientContext>,
        IClientContextSupervisor
    {
        public ClientContextSupervisor(IHostSettings hostSettings)
            : base(new ClientContextFactory(hostSettings))
        {
        }
    }
}
