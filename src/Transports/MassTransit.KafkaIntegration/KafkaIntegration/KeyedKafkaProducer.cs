namespace MassTransit.KafkaIntegration
{
    using System.Threading;
    using System.Threading.Tasks;


    public class KeyedTopicProducer<TKey, TValue> :
        ITopicProducer<TValue>
        where TValue : class
    {
        readonly KafkaKeyResolver<TKey, TValue> _keyResolver;
        readonly ITopicProducer<TKey, TValue> _topicProducer;

        public KeyedTopicProducer(ITopicProducer<TKey, TValue> topicProducer, KafkaKeyResolver<TKey, TValue> keyResolver)
        {
            _topicProducer = topicProducer;
            _keyResolver = keyResolver;
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _topicProducer.ConnectSendObserver(observer);
        }

        public Task Produce(TValue message, CancellationToken cancellationToken = default)
        {
            return Produce(message, Pipe.Empty<KafkaSendContext<TValue>>(), cancellationToken);
        }

        public Task Produce(TValue message, IPipe<KafkaSendContext<TValue>> pipe, CancellationToken cancellationToken = default)
        {
            return _topicProducer.Produce(default, message, new SetKeyPipe(_keyResolver, pipe), cancellationToken);
        }

        public Task Produce(object values, CancellationToken cancellationToken = default)
        {
            return Produce(values, Pipe.Empty<KafkaSendContext<TValue>>(), cancellationToken);
        }

        public Task Produce(object values, IPipe<KafkaSendContext<TValue>> pipe, CancellationToken cancellationToken = default)
        {
            return _topicProducer.Produce(default, values, new SetKeyPipe(_keyResolver, pipe), cancellationToken);
        }


        class SetKeyPipe :
            IPipe<KafkaSendContext<TKey, TValue>>
        {
            readonly KafkaKeyResolver<TKey, TValue> _keyResolver;
            readonly IPipe<KafkaSendContext<TKey, TValue>> _pipe;

            public SetKeyPipe(KafkaKeyResolver<TKey, TValue> keyResolver, IPipe<KafkaSendContext<TKey, TValue>> pipe = null)
            {
                _keyResolver = keyResolver;
                _pipe = pipe;
            }

            public Task Send(KafkaSendContext<TKey, TValue> context)
            {
                context.Key = _keyResolver(context);
                return _pipe.IsNotEmpty() ? _pipe.Send(context) : Task.CompletedTask;
            }

            public void Probe(ProbeContext context)
            {
                _pipe?.Probe(context);
            }
        }
    }
}
