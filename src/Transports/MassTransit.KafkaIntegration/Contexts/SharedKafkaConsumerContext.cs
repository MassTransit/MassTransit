namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
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

        public event Action<IConsumer<TKey, TValue>, Error> ErrorHandler
        {
            add => _context.ErrorHandler += value;
            remove => _context.ErrorHandler -= value;
        }

        public ReceiveSettings ReceiveSettings => _context.ReceiveSettings;

        public IHeadersDeserializer HeadersDeserializer => _context.HeadersDeserializer;

        public Task Subscribe()
        {
            return _context.Subscribe();
        }

        public Task Close()
        {
            return _context.Close();
        }

        public Task<ConsumeResult<TKey, TValue>> Consume(CancellationToken cancellationToken)
        {
            return _context.Consume(cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return _context.DisposeAsync();
        }

        public async Task Complete(ConsumeResult<TKey, TValue> result)
        {
            await _context.Complete(result);
        }
    }
}
