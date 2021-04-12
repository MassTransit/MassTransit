using System;
using System.Collections.Generic;
using EventStore.Client;
using GreenPipes;
using MassTransit.Configuration;
using MassTransit.Pipeline.Observables;
using MassTransit.Registration;

namespace MassTransit.EventStoreDbIntegration.Specifications
{
    public class EventStoreDbProducerSpecification :
        IEventStoreDbProducerConfigurator,
        IEventStoreDbProducerSpecification
    {
        readonly IEventStoreDbHostConfiguration _hostConfiguration;
        readonly IHostSettings _hostSettings;
        readonly SendObservable _sendObservers;
        readonly ISerializationConfiguration _serializationConfiguration;
        Action<ISendPipeConfigurator> _configureSend;

        public EventStoreDbProducerSpecification(IEventStoreDbHostConfiguration hostConfiguration, IHostSettings hostSettings)
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

        public void SetMessageSerializer(SerializerFactory serializerFactory)
        {
            _serializationConfiguration.SetSerializer(serializerFactory);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (!_hostSettings.UseExistingClient
                && (string.IsNullOrWhiteSpace(_hostSettings.ConnectionString) || string.IsNullOrWhiteSpace(_hostSettings.ConnectionName)))
                yield return this.Failure("HostSettings", "is invalid");
        }

        public IEventStoreDbProducerProvider CreateProducerProvider(IBusInstance busInstance)
        {
            var sendConfiguration = new SendPipeConfiguration(busInstance.HostConfiguration.HostTopology.SendTopology);
            _configureSend?.Invoke(sendConfiguration.Configurator);
            var sendPipe = sendConfiguration.CreatePipe();

            return new EventStoreDbProducerProvider(_hostConfiguration, busInstance, sendPipe, _sendObservers, _serializationConfiguration.Serializer);
        }
    }
}
