namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Producer;
    using GreenPipes;


    public class EventHubConnectionContext :
        BasePipeContext,
        ConnectionContext
    {
        readonly ConcurrentDictionary<string, EventHubProducerClient> _clients;
        readonly Action<EventHubProducerClientOptions> _configureOptions;

        public EventHubConnectionContext(IHostSettings hostSettings, IStorageSettings storageSettings, Action<EventHubProducerClientOptions> configureOptions,
            CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _configureOptions = configureOptions;
            HostSettings = hostSettings;
            StorageSettings = storageSettings;
            _clients = new ConcurrentDictionary<string, EventHubProducerClient>();
        }

        public IHostSettings HostSettings { get; }
        public IStorageSettings StorageSettings { get; }

        public EventHubProducerClient CreateEventHubClient(string eventHubName)
        {
            EventHubProducerClient CreateClient(string name)
            {
                var options = new EventHubProducerClientOptions();
                _configureOptions?.Invoke(options);
                var client = !string.IsNullOrWhiteSpace(HostSettings.ConnectionString)
                    ? new EventHubProducerClient(HostSettings.ConnectionString, name, options)
                    : new EventHubProducerClient(HostSettings.FullyQualifiedNamespace, name, HostSettings.TokenCredential, options);
                return client;
            }

            return _clients.GetOrAdd(eventHubName, CreateClient);
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var client in _clients.Values)
                await client.DisposeAsync().ConfigureAwait(false);
        }
    }
}
