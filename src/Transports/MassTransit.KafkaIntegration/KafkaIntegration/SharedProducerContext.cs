namespace MassTransit.KafkaIntegration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using MassTransit.Middleware;
    using Serializers;


    public class SharedProducerContext<TKey, TValue> :
        ProxyPipeContext,
        ProducerContext<TKey, TValue>
        where TValue : class
    {
        readonly ProducerContext<TKey, TValue> _context;

        public SharedProducerContext(ProducerContext<TKey, TValue> context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;
            CancellationToken = cancellationToken;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public override CancellationToken CancellationToken { get; }

        public IHeadersSerializer HeadersSerializer => _context.HeadersSerializer;

        public async Task Produce(TopicPartition partition, Message<TKey, TValue> message, CancellationToken cancellationToken)
        {
            await _context.Produce(partition, message, cancellationToken);
        }
    }
}
