using System;
using EventStore.Client;
using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class ConnectionContextSupervisor :
        TransportPipeContextSupervisor<ConnectionContext>,
        IConnectionContextSupervisor
    {
        public ConnectionContextSupervisor(Func<EventStoreClient> esdbClientFactory)
            : base(new ConnectionContextFactory(esdbClientFactory))
        {
        }
    }
}
