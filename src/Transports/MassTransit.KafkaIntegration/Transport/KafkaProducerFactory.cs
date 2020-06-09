namespace MassTransit.KafkaIntegration.Transport
{
    public class KafkaProducerFactory<TKey, TValue> :
        IKafkaProducerFactory
        where TValue : class
    {
        readonly IKafkaProducerContext<TKey, TValue> _context;

        public KafkaProducerFactory(string topicName, IKafkaProducerContext<TKey, TValue> context)
        {
            _context = context;
            TopicName = topicName;
        }

        public string TopicName { get; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IKafkaProducer<TKey, TValue> CreateProducer(KafkaTopicAddress topicAddress, ConsumeContext consumeContext = null)
        {
            return new KafkaProducer<TKey, TValue>(topicAddress, _context, consumeContext);
        }
    }
}
