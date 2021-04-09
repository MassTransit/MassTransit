using System.Threading;
using EventStore.Client;
using GreenPipes;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class EventStoreDbConnectionContext :
        BasePipeContext,
        ConnectionContext
    {
        public EventStoreDbConnectionContext(IHostSettings hostSettings, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            HostSettings = hostSettings;
        }

        public IHostSettings HostSettings { get; }

        public EventStoreClient CreateEventStoreDbClient()
        {
            var settings = EventStoreClientSettings.Create(HostSettings.ConnectionString);
            settings.ConnectionName = $"{HostSettings.ConnectionName}";

            if (HostSettings.DefaultCredentials != null)
                settings.DefaultCredentials = HostSettings.DefaultCredentials;

            return new EventStoreClient(settings);
        }
    }
}
