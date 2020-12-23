namespace MassTransit.EventHubIntegration.Specifications
{
    using System;
    using System.Collections.Generic;
    using Azure.Messaging.EventHubs.Producer;
    using Configuration;
    using GreenPipes;
    using MassTransit.Registration;
    using Pipeline.Observables;


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

        public void SetMessageSerializer(SerializerFactory serializerFactory)
        {
            _serializationConfiguration.SetSerializer(serializerFactory);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrWhiteSpace(_hostSettings.ConnectionString)
                && (string.IsNullOrWhiteSpace(_hostSettings.FullyQualifiedNamespace) || _hostSettings.TokenCredential == null))
                yield return this.Failure("HostSettings", "is invalid");
        }

        public IEvenHubProducerProviderFactory CreateProducerProviderFactory(IBusInstance busInstance)
        {
            var sendConfiguration = new SendPipeConfiguration(busInstance.HostConfiguration.HostTopology.SendTopology);
            _configureSend?.Invoke(sendConfiguration.Configurator);
            var sendPipe = sendConfiguration.CreatePipe();

            return new EvenHubProducerProviderFactory(_hostConfiguration, busInstance, sendPipe, _sendObservers, _serializationConfiguration.Serializer);
        }
    }
}
