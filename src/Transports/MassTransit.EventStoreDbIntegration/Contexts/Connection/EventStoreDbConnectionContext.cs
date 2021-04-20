using System;
using System.Threading;
using EventStore.Client;
using GreenPipes;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class EventStoreDbConnectionContext :
        BasePipeContext,
        ConnectionContext
    {
        readonly Func<EventStoreClient> _esdbClientFactory;

        public EventStoreDbConnectionContext(Func<EventStoreClient> esdbClientFactory, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _esdbClientFactory = esdbClientFactory;
        }

        public EventStoreClient CreateEventStoreDbClient()
        {
            return _esdbClientFactory();
        }
    }
}
