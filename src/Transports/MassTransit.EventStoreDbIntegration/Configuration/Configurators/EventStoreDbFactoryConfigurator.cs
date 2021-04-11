using System;
using System.Collections.Generic;
using EventStore.Client;
using GreenPipes;
using MassTransit.EventStoreDbIntegration.Contexts;
using MassTransit.EventStoreDbIntegration.Serializers;
using MassTransit.EventStoreDbIntegration.Specifications;
using MassTransit.Pipeline.Observables;
using MassTransit.Registration;
using MassTransit.SendPipeSpecifications;
using MassTransit.Transports;
using MassTransit.Util;

namespace MassTransit.EventStoreDbIntegration.Configurators
{
    public class EventStoreDbFactoryConfigurator :
        IEventStoreDbFactoryConfigurator,
        IEventStoreDbHostConfiguration
    {
        readonly Recycle<IConnectionContextSupervisor> _connectionContextSupervisor;
        readonly ReceiveEndpointObservable _endpointObservers;
        readonly List<IEventStoreDbReceiveEndpointSpecification> _endpoints;
        readonly HostSettings _hostSettings;
        readonly EventStoreDbProducerSpecification _producerSpecification;
        bool _isHostSettingsConfigured = false;

        public EventStoreDbFactoryConfigurator()
        {
            _endpointObservers = new ReceiveEndpointObservable();
            _endpoints = new List<IEventStoreDbReceiveEndpointSpecification>();
            _hostSettings = new HostSettings();
            _producerSpecification = new EventStoreDbProducerSpecification(this, _hostSettings);
            _connectionContextSupervisor = new Recycle<IConnectionContextSupervisor>(() =>
                new ConnectionContextSupervisor(_hostSettings));
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointObservers.Connect(observer);
        }

        public void Host(string connectionString, string connectionName)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException(nameof(connectionString));

            if (string.IsNullOrWhiteSpace(connectionName))
                throw new ArgumentException(nameof(connectionName));

            ThrowIfHostIsAlreadyConfigured();

            _hostSettings.ExistingClientFactory = null;
            _hostSettings.ConnectionString = connectionString;
            _hostSettings.ConnectionName = connectionName;
            _hostSettings.DefaultCredentials = null;
        }

        public void Host(string connectionString, string connectionName, UserCredentials userCredentials)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException(nameof(connectionString));

            if (string.IsNullOrWhiteSpace(connectionName))
                throw new ArgumentException(nameof(connectionName));

            if (userCredentials == null || string.IsNullOrWhiteSpace(userCredentials.Username) || string.IsNullOrWhiteSpace(userCredentials.Password))
                throw new ArgumentException(nameof(userCredentials));

            ThrowIfHostIsAlreadyConfigured();

            _hostSettings.ConnectionString = connectionString;
            _hostSettings.ConnectionName = connectionName;
            _hostSettings.DefaultCredentials = userCredentials;
        }

        public void UseExistingClient(EventStoreClientFactory eventStoreClientFactory)
        {
            ThrowIfHostIsAlreadyConfigured();

            _hostSettings.ExistingClientFactory = (provider) => provider.GetRequiredService<EventStoreClient>();
            //_hostSettings.ExistingClientFactory = eventStoreClientFactory;
            _hostSettings.ConnectionString = null;
            _hostSettings.ConnectionName = null;
            _hostSettings.DefaultCredentials = null;
        }

        public void StreamEndpoint(StreamCategory streamCategory, string subscriptionName, Action<IEventStoreDbReceiveEndpointConfigurator> configure)
        {
            var specification = CreateSpecification(streamCategory, subscriptionName, configure);
            _endpoints.Add(specification);
        }

        public void SetMessageSerializer(SerializerFactory serializerFactory)
        {
            _producerSpecification.SetMessageSerializer(serializerFactory);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _producerSpecification.ConnectSendObserver(observer);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _producerSpecification.ConfigureSend(callback);
        }

        public IEventStoreDbReceiveEndpointSpecification CreateSpecification(StreamCategory streamCategory, string subscriptionName,
            Action<IEventStoreDbReceiveEndpointConfigurator> configure)
        {
            if (streamCategory == null || string.IsNullOrWhiteSpace(streamCategory))
                throw new ArgumentException(nameof(streamCategory));
            if (string.IsNullOrWhiteSpace(subscriptionName))
                throw new ArgumentException(nameof(subscriptionName));

            var specification = new EventStoreDbReceiveEndpointSpecification(this, streamCategory, subscriptionName, _hostSettings, configure);
            specification.ConnectReceiveEndpointObserver(_endpointObservers);
            return specification;
        }

        public IConnectionContextSupervisor ConnectionContextSupervisor => _connectionContextSupervisor.Supervisor;

        public IEventStoreDbRider Build(IRiderRegistrationContext context, IBusInstance busInstance)
        {
            var endpoints = new ReceiveEndpointCollection();
            foreach (var endpoint in _endpoints)
                endpoints.Add(endpoint.EndpointName, endpoint.CreateReceiveEndpoint(busInstance));

            var producerProvider = _producerSpecification.CreateProducerProvider(busInstance);

            return new EventStoreDbRider(this, busInstance, endpoints, new CachedEventStoreDbProducerProvider(producerProvider), context);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            throw new NotImplementedException();
        }

        public IBusInstanceSpecification Build(IRiderRegistrationContext context)
        {
            return new EventStoreDbBusInstanceSpecification(context, this);
        }

        void ThrowIfHostIsAlreadyConfigured()
        {
            if (_isHostSettingsConfigured) throw new ConfigurationException("Host settings may not be specified more than once.");

            _isHostSettingsConfigured = true;
        }
    }
}
