using System.Threading;
using EventStore.Client;
using GreenPipes;
using MassTransit.Registration;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class EventStoreDbConnectionContext :
        BasePipeContext,
        ConnectionContext
    {
        readonly IConfigurationServiceProvider _provider;

        public EventStoreDbConnectionContext(IConfigurationServiceProvider provider, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _provider = provider;
        }

        public EventStoreClient CreateEventStoreDbClient()
        {
            return _provider.GetRequiredService<EventStoreClient>();
        }
    }
}
