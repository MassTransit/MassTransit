namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Generic;
    using Azure.Core;
    using Azure.Storage;
    using Azure.Storage.Blobs;
    using Configuration;
    using GreenPipes;
    using Pipeline.Observables;
    using Processors;
    using Registration;
    using Riders;


    public class EventHubFactoryConfigurator :
        IEventHubFactoryConfigurator
    {
        readonly List<IEventHubDefinition> _definitions;
        readonly ReceiveEndpointObservable _endpointObservers;
        readonly RiderObservable _observers;
        HostSettings _hostSettings;
        IStorageSettings _storageSettings;

        public EventHubFactoryConfigurator()
        {
            _observers = new RiderObservable();
            _endpointObservers = new ReceiveEndpointObservable();
            _definitions = new List<IEventHubDefinition>();
        }

        public ConnectHandle ConnectRiderObserver(IRiderObserver observer)
        {
            return _observers.Connect(observer);
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointObservers.Connect(observer);
        }

        public void Host(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException(nameof(connectionString));

            if (_hostSettings != null)
                throw new InvalidOperationException("Host settings may not be specified more than once.");

            _hostSettings = new HostSettings(connectionString);
        }

        public void Host(string fullyQualifiedNamespace, TokenCredential tokenCredential)
        {
            if (string.IsNullOrWhiteSpace(fullyQualifiedNamespace))
                throw new ArgumentException(nameof(fullyQualifiedNamespace));
            if (tokenCredential == null)
                throw new ArgumentNullException(nameof(tokenCredential));

            if (_hostSettings != null)
                throw new InvalidOperationException("Host settings may not be specified more than once.");

            _hostSettings = new HostSettings(fullyQualifiedNamespace, tokenCredential);
        }

        public void Storage(string connectionString, Action<BlobClientOptions> configure = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException(nameof(connectionString));

            if (_storageSettings != null)
                throw new InvalidOperationException("Storage settings may not be specified more than once.");

            _storageSettings = new StorageSettings(connectionString, configure);
        }

        public void Storage(Uri containerUri, Action<BlobClientOptions> configure = null)
        {
            if (containerUri == null)
                throw new ArgumentException(nameof(containerUri));

            if (_storageSettings != null)
                throw new InvalidOperationException("Storage settings may not be specified more than once.");

            _storageSettings = new StorageSettings(containerUri, configure);
        }

        public void Storage(Uri containerUri, TokenCredential credential, Action<BlobClientOptions> configure = null)
        {
            if (containerUri == null)
                throw new ArgumentException(nameof(containerUri));
            if (credential == null)
                throw new ArgumentNullException(nameof(credential));

            if (_storageSettings != null)
                throw new InvalidOperationException("Storage settings may not be specified more than once.");

            _storageSettings = new StorageSettings(containerUri, credential, configure);
        }

        public void Storage(Uri containerUri, StorageSharedKeyCredential credential, Action<BlobClientOptions> configure = null)
        {
            if (containerUri == null)
                throw new ArgumentException(nameof(containerUri));
            if (credential == null)
                throw new ArgumentNullException(nameof(credential));

            if (_storageSettings != null)
                throw new InvalidOperationException("Storage settings may not be specified more than once.");

            _storageSettings = new StorageSettings(containerUri, credential, configure);
        }

        public void Subscribe(string eventHubName, string consumerGroup, Action<IEventHubConfigurator> configure)
        {
            if (string.IsNullOrWhiteSpace(eventHubName))
                throw new ArgumentException(nameof(eventHubName));
            if (string.IsNullOrWhiteSpace(consumerGroup))
                throw new ArgumentException(nameof(consumerGroup));

            var definition = new EventHubDefinition(eventHubName, consumerGroup, _hostSettings, _storageSettings, configure);
            definition.ConnectReceiveEndpointObserver(_endpointObservers);
            _definitions.Add(definition);
        }

        public IBusInstanceSpecification Build()
        {
            var specification = new EventHubBusInstanceSpecification(_definitions, _observers);
            return specification;
        }
    }
}
