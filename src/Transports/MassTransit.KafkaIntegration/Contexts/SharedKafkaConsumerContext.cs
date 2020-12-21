namespace MassTransit.KafkaIntegration.Contexts
{
    using System.Threading;
    using Confluent.Kafka;
    using GreenPipes;
    using Serializers;
    using Transport;


    public class SharedKafkaConsumerContext<TKey, TValue> :
        ProxyPipeContext,
        IKafkaConsumerContext<TKey, TValue>
        where TValue : class
    {
        readonly IKafkaConsumerContext<TKey, TValue> _context;

        public SharedKafkaConsumerContext(IKafkaConsumerContext<TKey, TValue> context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        public ReceiveSettings ReceiveSettings => _context.ReceiveSettings;

        public IHeadersDeserializer HeadersDeserializer => _context.HeadersDeserializer;

        public ConsumerBuilder<TKey, TValue> CreateConsumerBuilder()
        {
            return _context.CreateConsumerBuilder();
        }
    }
}
