namespace MassTransit.KafkaIntegration.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Confluent.Kafka;
    using Context;
    using MassTransit.Registration;
    using Pipeline.Observables;
    using Serializers;


    public class KafkaProducerFactory<TKey, TValue> :
        IKafkaProducerFactory
        where TValue : class
    {
        readonly KafkaProducerContext _context;
        readonly IProducer<TKey, TValue> _producer;

        public KafkaProducerFactory(string topicName, IProducer<TKey, TValue> producer, IBusInstance busInstance, IHeadersSerializer headersSerializer,
            SendObservable sendObservers)
        {
            TopicName = topicName;
            _producer = producer;

            _context = new KafkaProducerContext(_producer, busInstance.HostConfiguration, sendObservers, headersSerializer);
        }

        public string TopicName { get; }

        public void Dispose()
        {
            var flushTimeout = TimeSpan.FromSeconds(30);

            _producer.Flush(flushTimeout);
            _producer.Dispose();
        }

        public IKafkaProducer<TKey, TValue> CreateProducer(KafkaTopicAddress topicAddress, ConsumeContext consumeContext = null)
        {
            //TODO apply all specifications, etc, using a builder like most other things
            return new KafkaProducer<TKey, TValue>(topicAddress, _context, consumeContext);
        }


        class KafkaProducerContext :
            IKafkaProducerContext<TKey, TValue>
        {
            readonly IProducer<TKey, TValue> _producer;

            public KafkaProducerContext(IProducer<TKey, TValue> producer, IHostConfiguration hostConfiguration, SendObservable sendObservers,
                IHeadersSerializer headersSerializer)
            {
                _producer = producer;
                SendObservers = sendObservers;
                HostAddress = hostConfiguration.HostAddress;
                LogContext = hostConfiguration.SendLogContext;
                HeadersSerializer = headersSerializer;
            }

            public Uri HostAddress { get; }
            public ILogContext LogContext { get; }
            public SendObservable SendObservers { get; }

            public IHeadersSerializer HeadersSerializer { get; }

            public Task Produce(TopicPartition partition, Message<TKey, TValue> message, CancellationToken cancellationToken)
            {
                return _producer.ProduceAsync(partition, message, cancellationToken);
            }
        }
    }
}
