namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading;
    using Azure.Messaging.EventHubs.Producer;
    using Configuration;
    using MassTransit.Middleware;


    public class EventHubConnectionContext :
        BasePipeContext,
        ConnectionContext
    {
        readonly Action<EventHubProducerClientOptions> _configureOptions;

        public EventHubConnectionContext(IHostSettings hostSettings, IStorageSettings storageSettings, Action<EventHubProducerClientOptions> configureOptions,
            CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _configureOptions = configureOptions;
            HostSettings = hostSettings;
            StorageSettings = storageSettings;
        }

        public IHostSettings HostSettings { get; }
        public IStorageSettings StorageSettings { get; }

        public EventHubProducerClient CreateEventHubClient(string eventHubName)
        {
            var options = new EventHubProducerClientOptions();
            _configureOptions?.Invoke(options);
            var client = !string.IsNullOrWhiteSpace(HostSettings.ConnectionString)
                ? new EventHubProducerClient(HostSettings.ConnectionString, eventHubName, options)
                : new EventHubProducerClient(HostSettings.FullyQualifiedNamespace, eventHubName, HostSettings.TokenCredential, options);
            return client;
        }
    }
}
