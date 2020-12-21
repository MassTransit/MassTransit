namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using System.Threading;
    using Confluent.Kafka;
    using GreenPipes;
    using Serializers;
    using Transport;


    public class KafkaConsumerContext<TKey, TValue> :
        BasePipeContext,
        IKafkaConsumerContext<TKey, TValue>
        where TValue : class
    {
        readonly Func<ConsumerBuilder<TKey, TValue>> _consumerBuilderFactory;

        public KafkaConsumerContext(ReceiveSettings receiveSettings, IHeadersDeserializer headersDeserializer,
            Func<ConsumerBuilder<TKey, TValue>> consumerBuilderFactory, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _consumerBuilderFactory = consumerBuilderFactory;
            ReceiveSettings = receiveSettings;
            HeadersDeserializer = headersDeserializer;
        }

        public ReceiveSettings ReceiveSettings { get; }

        public IHeadersDeserializer HeadersDeserializer { get; }

        public ConsumerBuilder<TKey, TValue> CreateConsumerBuilder()
        {
            return _consumerBuilderFactory();
        }
    }
}
