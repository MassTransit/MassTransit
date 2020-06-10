namespace MassTransit.KafkaIntegration.Transport
{
    using System;


    public class KafkaProducerFactory<TKey, TValue> :
        IKafkaProducerFactory
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

        public void Dispose()
        {
            _context.Dispose();
        }

        public IKafkaProducer<TKey, TValue> CreateProducer(ConsumeContext consumeContext = null)
        {
            return new KafkaProducer<TKey, TValue>(_topicAddress, _context, consumeContext);
        }
    }
}
