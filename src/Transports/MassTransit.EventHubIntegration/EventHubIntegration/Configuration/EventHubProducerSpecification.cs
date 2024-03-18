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
        readonly List<Action<ISendPipeConfigurator>> _configureSend;
        readonly IEventHubHostConfiguration _hostConfiguration;
        readonly IHostSettings _hostSettings;
        readonly SendObservable _sendObservers;
        readonly ISerializationConfiguration _serializationConfiguration;
        Action<EventHubProducerClientOptions> _configureOptions;

        public EventHubProducerSpecification(IEventHubHostConfiguration hostConfiguration, IHostSettings hostSettings)
        {
            _hostConfiguration = hostConfiguration;
            _hostSettings = hostSettings;
            _serializationConfiguration = new SerializationConfiguration();
            _sendObservers = new SendObservable();
            _configureSend = new List<Action<ISendPipeConfigurator>>();
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservers.Connect(observer);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _configureSend.Add(callback ?? throw new ArgumentNullException(nameof(callback)));
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

        public EventHubSendTransportContext CreateSendTransportContext(string eventHubName, IBusInstance busInstance)
        {
            var sendConfiguration = new SendPipeConfiguration(busInstance.HostConfiguration.Topology.SendTopology);
            for (var i = 0; i < _configureSend.Count; i++)
                _configureSend[i].Invoke(sendConfiguration.Configurator);

            var sendPipe = sendConfiguration.CreatePipe();

            var supervisor = new ProducerContextSupervisor(_hostConfiguration.ConnectionContextSupervisor, eventHubName);

            var transportContext = new EventHubProducerSendTransportContext(supervisor, sendPipe, busInstance.HostConfiguration, eventHubName,
                _serializationConfiguration.CreateSerializerCollection());
            transportContext.ConnectSendObserver(_sendObservers);

            return transportContext;
        }
    }
}
