using System;
using System.Collections.Generic;
using System.Linq;
using EventStore.Client;
using GreenPipes;
using MassTransit.EventStoreDbIntegration.Contexts;
using MassTransit.EventStoreDbIntegration.Serializers;
using MassTransit.EventStoreDbIntegration.Specifications;
using MassTransit.Pipeline.Observables;
using MassTransit.Registration;
using MassTransit.Transports;
using MassTransit.Util;

namespace MassTransit.EventStoreDbIntegration.Configurators
{
    public class EventStoreDbFactoryConfigurator :
        IEventStoreDbFactoryConfigurator,
        IEventStoreDbHostConfiguration
    {   
        readonly ReceiveEndpointObservable _endpointObservers;
        readonly List<IEventStoreDbSubscriptionSpecification> _endpoints;
        readonly HostSettings _hostSettings;
        readonly EventStoreDbProducerSpecification _producerSpecification;
        Recycle<IConnectionContextSupervisor> _connectionContextSupervisor;
        IHeadersDeserializer _headersDeserializer;
        bool _isHostSettingsConfigured = false;

        public EventStoreDbFactoryConfigurator()
        {
            _endpointObservers = new ReceiveEndpointObservable();
            _endpoints = new List<IEventStoreDbSubscriptionSpecification>();
            _hostSettings = new HostSettings();
            _producerSpecification = new EventStoreDbProducerSpecification(this);

            SetHeadersDeserializer(DictionaryHeadersSerde.Deserializer);
            SetHeadersSerializer(DictionaryHeadersSerde.Serializer);
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

        public void CatchupSubscription(StreamName streamName, string subscriptionName, Action<IEventStoreDbCatchupSubscriptionConfigurator> configure)
        {
            var specification = CreateCatchupSubscriptionSpecification(streamName, subscriptionName, configure);
            _endpoints.Add(specification);
        }

        public void SetHeadersDeserializer(IHeadersDeserializer deserializer)
        {
            _headersDeserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        public void SetHeadersSerializer(IHeadersSerializer serializer)
        {
            _producerSpecification.SetHeadersSerializer(serializer);
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

        public IEventStoreDbSubscriptionSpecification CreateCatchupSubscriptionSpecification(StreamName streamName, string subscriptionName,
            Action<IEventStoreDbCatchupSubscriptionConfigurator> configure)
        {
            if (streamName == null)
                throw new ArgumentNullException(nameof(streamName));
            if (string.IsNullOrWhiteSpace(subscriptionName))
                throw new ArgumentException(nameof(subscriptionName));

            var specification = new EventStoreDbCatchupSubscriptionSpecification(this, streamName, subscriptionName, _headersDeserializer, configure);
            specification.ConnectReceiveEndpointObserver(_endpointObservers);
            return specification;
        }

        public IConnectionContextSupervisor ConnectionContextSupervisor => _connectionContextSupervisor.Supervisor;

        public IEventStoreDbRider Build(IRiderRegistrationContext context, IBusInstance busInstance)
        {
            _connectionContextSupervisor = new Recycle<IConnectionContextSupervisor>(() => new ConnectionContextSupervisor(CreateEventStoreClientFactory(context)));

            var endpoints = new ReceiveEndpointCollection();
            foreach (var endpoint in _endpoints)
                endpoints.Add(endpoint.EndpointName, endpoint.CreateReceiveEndpoint(busInstance));

            var producerProvider = _producerSpecification.CreateProducerProvider(busInstance);

            return new EventStoreDbRider(this, busInstance, endpoints, new CachedEventStoreDbProducerProvider(producerProvider));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            foreach (KeyValuePair<string, IEventStoreDbSubscriptionSpecification[]> kv in _endpoints.GroupBy(x => x.EndpointName)
                .ToDictionary(x => x.Key, x => x.ToArray()))
            {
                if (kv.Value.Length > 1)
                    yield return this.Failure($"EventStoreDB: {kv.Key} was added more than once.");

                foreach (var result in kv.Value.SelectMany(x => x.Validate()))
                    yield return result;
            }

            foreach (var result in _producerSpecification.Validate())
                yield return result;
        }

        public IBusInstanceSpecification Build(IRiderRegistrationContext context)
        {
            return new EventStoreDbBusInstanceSpecification(context, this);
        }

        Func<EventStoreClient> CreateEventStoreClientFactory(IRiderRegistrationContext context)
        {
            if (_isHostSettingsConfigured)
            {
                //Create a client for this rider instance with the provided settings.
                var client = CreateEventStoreClient();
                return () => client;
            }
            else
            {
                //Use a client that was registered outside of MassTransit, this is the preferred approach.
                return () => context.GetRequiredService<EventStoreClient>();
            }
        }

        EventStoreClient CreateEventStoreClient()
        {
            var settings = EventStoreClientSettings.Create(_hostSettings.ConnectionString);
            settings.ConnectionName = _hostSettings.ConnectionName;

            return new EventStoreClient(settings);
        }

        void ThrowIfHostIsAlreadyConfigured()
        {
            if (_isHostSettingsConfigured)
                throw new ConfigurationException("Host settings may not be specified more than once.");

            _isHostSettingsConfigured = true;
        }
    }
}
