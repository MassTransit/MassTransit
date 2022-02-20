namespace MassTransit.KafkaIntegration.Configuration
{
    using System;
    using Confluent.Kafka;


    public class KafkaProducerRegistration<TKey, TValue> :
        IKafkaProducerRegistration
        where TValue : class
    {
        readonly Action<IRiderRegistrationContext, IKafkaProducerConfigurator<TKey, TValue>> _configure;
        readonly ProducerConfig _producerConfig;
        readonly string _topic;

        public KafkaProducerRegistration(string topic, Action<IRiderRegistrationContext, IKafkaProducerConfigurator<TKey, TValue>> configure,
            ProducerConfig producerConfig = null)
        {
            _topic = topic;
            _producerConfig = producerConfig;
            _configure = configure;
        }

        public void Register(IKafkaFactoryConfigurator configurator, IRiderRegistrationContext context)
        {
            if (_producerConfig != null)
                configurator.TopicProducer<TKey, TValue>(_topic, _producerConfig, c => _configure?.Invoke(context, c));
            else
                configurator.TopicProducer<TKey, TValue>(_topic, c => _configure?.Invoke(context, c));
        }

        public Type Type => typeof(KafkaProducerRegistration<TKey, TValue>);
    }
}
