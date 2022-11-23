namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Transports;


    public class ScopedTopicProducer<TKey, TValue> :
        ITopicProducer<TKey, TValue>
        where TValue : class
    {
        readonly ITopicProducer<TKey, TValue> _producer;
        readonly IServiceProvider _provider;

        public ScopedTopicProducer(ITopicProducer<TKey, TValue> producer, IServiceProvider provider)
        {
            _producer = producer;
            _provider = provider;
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _producer.ConnectSendObserver(observer);
        }

        public Task Produce(TKey key, TValue value, CancellationToken cancellationToken = default)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return _producer.Produce(key, value, GetPipeProxy(), cancellationToken);
        }

        public Task Produce(TKey key, TValue value, IPipe<KafkaSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken = default)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return _producer.Produce(key, value, GetPipeProxy(pipe), cancellationToken);
        }

        public Task Produce(TKey key, object values, CancellationToken cancellationToken = default)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return _producer.Produce(key, values, GetPipeProxy(), cancellationToken);
        }

        public Task Produce(TKey key, object values, IPipe<KafkaSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken = default)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return _producer.Produce(key, values, GetPipeProxy(pipe), cancellationToken);
        }

        protected IPipe<KafkaSendContext<TKey, TValue>> GetPipeProxy(IPipe<KafkaSendContext<TKey, TValue>> pipe = default)
        {
            return new ScopedProducePipeAdapter(_provider, pipe);
        }


        class ScopedProducePipeAdapter :
            IPipe<KafkaSendContext<TKey, TValue>>,
            ISendPipe
        {
            readonly IPipe<KafkaSendContext<TKey, TValue>> _pipe;
            readonly IServiceProvider _provider;

            public ScopedProducePipeAdapter(IServiceProvider provider, IPipe<KafkaSendContext<TKey, TValue>> pipe)
            {
                _provider = provider;
                _pipe = pipe;
            }

            public void Probe(ProbeContext context)
            {
                _pipe?.Probe(context);
            }

            public Task Send(KafkaSendContext<TKey, TValue> context)
            {
                return _pipe.IsNotEmpty() ? _pipe.Send(context) : Task.CompletedTask;
            }

            public Task Send<T>(SendContext<T> context)
                where T : class
            {
                context.GetOrAddPayload(() => _provider);

                return _pipe is ISendContextPipe sendContextPipe
                    ? sendContextPipe.Send(context)
                    : Task.CompletedTask;
            }
        }
    }
}
