namespace MassTransit.EventHubIntegration.Configuration
{
    using System;
    using System.Collections.Generic;
    using Azure.Messaging.EventHubs.Producer;
    using MassTransit.Configuration;
    using Observables;
    using Transports;


    public class EventHubProducerSpecification :
        IEventHubProducerConfigurator,
        IEventHubProducerSpecification
    {
        readonly IEventHubHostConfiguration _hostConfiguration;
        readonly IHostSettings _hostSettings;
        readonly SendObservable _sendObservers;
        readonly ISerializationConfiguration _serializationConfiguration;
        Action<EventHubProducerClientOptions> _configureOptions;
        Action<ISendPipeConfigurator> _configureSend;

        public EventHubProducerSpecification(IEventHubHostConfiguration hostConfiguration, IHostSettings hostSettings)
        {
            _hostConfiguration = hostConfiguration;
            _hostSettings = hostSettings;
            _serializationConfiguration = new SerializationConfiguration();
            _sendObservers = new SendObservable();
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservers.Connect(observer);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _configureSend = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        public Action<EventHubProducerClientOptions> ConfigureOptions
        {
            set => _configureOptions = value ?? throw new ArgumentNullException(nameof(value));
            get => _configureOptions;
        }

        public void AddSerializer(ISerializerFactory factory, bool isSerializer = true)
        {
            _serializationConfiguration.AddSerializer(factory, isSerializer);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrWhiteSpace(_hostSettings.ConnectionString)
                && (string.IsNullOrWhiteSpace(_hostSettings.FullyQualifiedNamespace) || _hostSettings.TokenCredential == null))
                yield return this.Failure("HostSettings", "is invalid");
        }

        public IEventHubProducerProvider CreateProducerProvider(IBusInstance busInstance)
        {
            var sendConfiguration = new SendPipeConfiguration(busInstance.HostConfiguration.Topology.SendTopology);
            _configureSend?.Invoke(sendConfiguration.Configurator);
            var sendPipe = sendConfiguration.CreatePipe();

            return new EventHubProducerProvider(_hostConfiguration, busInstance, sendPipe, _sendObservers,
                _serializationConfiguration.CreateSerializerCollection());
        }
    }
}
