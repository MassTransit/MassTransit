namespace MassTransit.EventHubIntegration.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Azure.Core;
    using Azure.Messaging.EventHubs.Producer;
    using Azure.Storage;
    using Azure.Storage.Blobs;
    using MassTransit.Configuration;
    using Observables;
    using Transports;
    using Util;


    public class EventHubFactoryConfigurator :
        IEventHubFactoryConfigurator,
        IEventHubHostConfiguration
    {
        readonly Recycle<IConnectionContextSupervisor> _connectionContextSupervisor;
        readonly ReceiveEndpointObservable _endpointObservers;
        readonly List<IEventHubReceiveEndpointSpecification> _endpoints;
        readonly HostSettings _hostSettings;
        readonly EventHubProducerSpecification _producerSpecification;
        readonly StorageSettings _storageSettings;
        bool _isHostSettingsConfigured;
        bool _isStorageSettingsConfigured;

        public EventHubFactoryConfigurator()
        {
            _endpointObservers = new ReceiveEndpointObservable();
            _endpoints = new List<IEventHubReceiveEndpointSpecification>();
            _hostSettings = new HostSettings();
            _storageSettings = new StorageSettings();

            _producerSpecification = new EventHubProducerSpecification(this, _hostSettings);

            _connectionContextSupervisor = new Recycle<IConnectionContextSupervisor>(() =>
                new ConnectionContextSupervisor(_hostSettings, _storageSettings, _producerSpecification.ConfigureOptions));
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointObservers.Connect(observer);
        }

        public void Host(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException(nameof(connectionString));

            ThrowIfHostIsAlreadyConfigured();

            _hostSettings.ConnectionString = connectionString;
        }

        public void Host(string fullyQualifiedNamespace, TokenCredential tokenCredential)
        {
            if (string.IsNullOrWhiteSpace(fullyQualifiedNamespace))
                throw new ArgumentException(nameof(fullyQualifiedNamespace));
            if (tokenCredential == null)
                throw new ArgumentNullException(nameof(tokenCredential));

            ThrowIfHostIsAlreadyConfigured();

            _hostSettings.FullyQualifiedNamespace = fullyQualifiedNamespace;
            _hostSettings.TokenCredential = tokenCredential;
        }

        public void Storage(string connectionString, Action<BlobClientOptions> configure = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException(nameof(connectionString));

            ThrowIfStorageIsAlreadyConfigured();

            _storageSettings.ConnectionString = connectionString;
            _storageSettings.Configure = configure;
        }

        public void Storage(Uri containerUri, Action<BlobClientOptions> configure = null)
        {
            if (containerUri == null)
                throw new ArgumentException(nameof(containerUri));

            ThrowIfStorageIsAlreadyConfigured();

            _storageSettings.ContainerUri = containerUri;
            _storageSettings.Configure = configure;
        }

        public void Storage(Uri containerUri, TokenCredential credential, Action<BlobClientOptions> configure = null)
        {
            if (containerUri == null)
                throw new ArgumentException(nameof(containerUri));
            if (credential == null)
                throw new ArgumentNullException(nameof(credential));

            ThrowIfStorageIsAlreadyConfigured();

            _storageSettings.ContainerUri = containerUri;
            _storageSettings.TokenCredential = credential;
            _storageSettings.Configure = configure;
        }

        public void Storage(Uri containerUri, StorageSharedKeyCredential credential, Action<BlobClientOptions> configure = null)
        {
            if (containerUri == null)
                throw new ArgumentException(nameof(containerUri));
            if (credential == null)
                throw new ArgumentNullException(nameof(credential));

            ThrowIfStorageIsAlreadyConfigured();

            _storageSettings.ContainerUri = containerUri;
            _storageSettings.SharedKeyCredential = credential;
            _storageSettings.Configure = configure;
        }

        public void ReceiveEndpoint(string eventHubName, string consumerGroup, Action<IEventHubReceiveEndpointConfigurator> configure)
        {
            var specification = CreateSpecification(eventHubName, consumerGroup, configure);
            _endpoints.Add(specification);
        }

        public void AddSerializer(ISerializerFactory factory, bool isSerializer = true)
        {
            _producerSpecification.AddSerializer(factory, isSerializer);
        }

        public void ConfigureProducerOptions(Action<EventHubProducerClientOptions> configure)
        {
            if (_producerSpecification.ConfigureOptions != null)
                throw new ConfigurationException("ProducerOptions configurator may not be specified more than once.");
            _producerSpecification.ConfigureOptions = configure;
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _producerSpecification.ConnectSendObserver(observer);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _producerSpecification.ConfigureSend(callback);
        }

        public IEventHubReceiveEndpointSpecification CreateSpecification(string eventHubName, string consumerGroup,
            Action<IEventHubReceiveEndpointConfigurator> configure)
        {
            if (string.IsNullOrWhiteSpace(eventHubName))
                throw new ArgumentException(nameof(eventHubName));
            if (string.IsNullOrWhiteSpace(consumerGroup))
                throw new ArgumentException(nameof(consumerGroup));

            var specification = new EventHubReceiveEndpointSpecification(this, eventHubName, consumerGroup, _hostSettings, _storageSettings, configure);
            specification.ConnectReceiveEndpointObserver(_endpointObservers);
            return specification;
        }

        public IConnectionContextSupervisor ConnectionContextSupervisor => _connectionContextSupervisor.Supervisor;

        public IEventHubRider Build(IRiderRegistrationContext context, IBusInstance busInstance)
        {
            ConnectSendObserver(busInstance.HostConfiguration.SendObservers);

            var endpoints = new ReceiveEndpointCollection();
            foreach (var endpoint in _endpoints)
                endpoints.Add(endpoint.EndpointName, endpoint.CreateReceiveEndpoint(busInstance));

            var producerProvider = _producerSpecification.CreateProducerProvider(busInstance);

            return new EventHubRider(this, busInstance, endpoints, new CachedEventHubProducerProvider(producerProvider), context);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            foreach (KeyValuePair<string, IEventHubReceiveEndpointSpecification[]> kv in _endpoints.GroupBy(x => x.EndpointName)
                         .ToDictionary(x => x.Key, x => x.ToArray()))
            {
                if (kv.Value.Length > 1)
                    yield return this.Failure($"EventHub: {kv.Key} was added more than once.");

                foreach (var result in kv.Value.SelectMany(x => x.Validate()))
                    yield return result;
            }

            foreach (var result in _producerSpecification.Validate())
                yield return result;
        }

        public IBusInstanceSpecification Build(IRiderRegistrationContext context)
        {
            return new EventHubBusInstanceSpecification(context, this);
        }

        void ThrowIfHostIsAlreadyConfigured()
        {
            if (_isHostSettingsConfigured)
                throw new ConfigurationException("Host settings may not be specified more than once.");
            _isHostSettingsConfigured = true;
        }

        void ThrowIfStorageIsAlreadyConfigured()
        {
            if (_isStorageSettingsConfigured)
                throw new ConfigurationException("Storage settings may not be specified more than once.");
            _isStorageSettingsConfigured = true;
        }
    }
}
