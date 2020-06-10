namespace MassTransit.KafkaIntegration.Registration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;


    public class KafkaProducerRegistrationConfigurator<TKey, TValue> :
        IKafkaProducerRegistration,
        IKafkaProducerRegistrationConfigurator<TKey, TValue>
        where TValue : class
    {
        readonly List<Action<IKafkaProducerConfigurator<TKey, TValue>>> _configurations = new List<Action<IKafkaProducerConfigurator<TKey, TValue>>>();
        readonly ProducerConfig _producerConfig;
        readonly string _topic;
        ISerializer<TKey> _keySerializer;
        ISerializer<TValue> _valueSerializer;

        public KafkaProducerRegistrationConfigurator(string topic, ProducerConfig producerConfig = null)
        {
            _topic = topic;
            _producerConfig = producerConfig;
        }

        public void Register(IKafkaFactoryConfigurator configurator)
        {
            if (_keySerializer != null)
                _configurations.Add(x => x.SetKeySerializer(_keySerializer));
            if (_valueSerializer != null)
                _configurations.Add(x => x.SetValueSerializer(_valueSerializer));

            if (_producerConfig != null)
                configurator.TopicProducer<TKey, TValue>(_topic, _producerConfig, c => _configurations.ForEach(x => x(c)));
            else
                configurator.TopicProducer<TKey, TValue>(_topic, c => _configurations.ForEach(x => x(c)));
        }

        public IKafkaProducerRegistrationConfigurator<TKey, TValue> SetKeySerializer(ISerializer<TKey> serializer)
        {
            _keySerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            return this;
        }

        public IKafkaProducerRegistrationConfigurator<TKey, TValue> SetValueSerializer(ISerializer<TValue> serializer)
        {
            _valueSerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            return this;
        }

        public IKafkaProducerRegistrationConfigurator<TKey, TValue> Configure(Action<IKafkaProducerConfigurator> configure)
        {
            _configurations.Add(configure ?? throw new ArgumentNullException(nameof(configure)));
            return this;
        }
    }
}
