using System;
using System.Collections.Generic;
using GreenPipes;
using MassTransit.Configuration;
using MassTransit.EventStoreDbIntegration.Serializers;
using MassTransit.Pipeline.Observables;
using MassTransit.Registration;

namespace MassTransit.EventStoreDbIntegration.Specifications
{
    public sealed class EventStoreDbProducerSpecification :
        IEventStoreDbProducerConfigurator,
        IEventStoreDbProducerSpecification
    {
        readonly IEventStoreDbHostConfiguration _hostConfiguration;
        readonly SendObservable _sendObservers;
        readonly ISerializationConfiguration _serializationConfiguration;
        Action<ISendPipeConfigurator> _configureSend;
        IHeadersSerializer _headersSerializer;

        public EventStoreDbProducerSpecification(IEventStoreDbHostConfiguration hostConfiguration)
        {
            _hostConfiguration = hostConfiguration;
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

        public void SetHeadersSerializer(IHeadersSerializer serializer)
        {
            _headersSerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void SetMessageSerializer(SerializerFactory serializerFactory)
        {
            _serializationConfiguration.SetSerializer(serializerFactory);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_headersSerializer == null)
                yield return this.Failure("HeadersSerializer", "should not be null");
        }

        public IEventStoreDbProducerProvider CreateProducerProvider(IBusInstance busInstance)
        {
            var sendConfiguration = new SendPipeConfiguration(busInstance.HostConfiguration.HostTopology.SendTopology);
            _configureSend?.Invoke(sendConfiguration.Configurator);
            var sendPipe = sendConfiguration.CreatePipe();

            return new EventStoreDbProducerProvider(_hostConfiguration, busInstance, sendPipe, _sendObservers, _headersSerializer,
                _serializationConfiguration.Serializer);
        }
    }
}
