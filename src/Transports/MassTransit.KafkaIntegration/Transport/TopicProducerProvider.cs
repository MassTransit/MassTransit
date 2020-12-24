namespace MassTransit.KafkaIntegration.Transport
{
    using System;
    using MassTransit.Registration;


    public class TopicProducerProvider :
        ITopicProducerProvider
    {
        readonly IBusInstance _busInstance;
        readonly IKafkaHostConfiguration _hostConfiguration;

        public TopicProducerProvider(IBusInstance busInstance, IKafkaHostConfiguration hostConfiguration)
        {
            _busInstance = busInstance;
            _hostConfiguration = hostConfiguration;
        }

        public ITopicProducer<TKey, TValue> GetProducer<TKey, TValue>(Uri address)
            where TValue : class
        {
            return _hostConfiguration.ClientContextSupervisor.CreateProducer<TKey, TValue>(_busInstance, address);
        }
    }
}
