namespace MassTransit.KafkaIntegration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using MassTransit.Middleware;


    public class SharedProducerContext :
        ProxyPipeContext,
        ProducerContext
    {
        readonly ProducerContext _context;

        public SharedProducerContext(ProducerContext context, CancellationToken cancellationToken)
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

        public Task Produce(TopicPartition partition, Message<byte[], byte[]> message, CancellationToken cancellationToken)
        {
            return _context.Produce(partition, message, cancellationToken);
        }
    }
}
