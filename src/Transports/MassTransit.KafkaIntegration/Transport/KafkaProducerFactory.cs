namespace MassTransit.KafkaIntegration.Transport
{
    using System;


    public class KafkaProducerFactory<TKey, TValue> :
        IKafkaProducerFactory<TKey, TValue>
        where TValue : class
    {
        readonly IKafkaProducerContext<TKey, TValue> _context;
        readonly KafkaTopicAddress _topicAddress;

        public KafkaProducerFactory(KafkaTopicAddress topicAddress, IKafkaProducerContext<TKey, TValue> context)
        {
            _context = context;
            _topicAddress = topicAddress;
        }

        public Uri TopicAddress => _topicAddress;

        public ITopicProducer<TKey, TValue> CreateProducer(ConsumeContext consumeContext = null)
        {
            return new TopicProducer<TKey, TValue>(_topicAddress, _context, consumeContext);
        }
    }
}
