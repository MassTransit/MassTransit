namespace MassTransit.KafkaIntegration.Caching
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    public class CachedTopicProducer<T, TKey, TValue> :
        ICachedTopicProducer<T>,
        ITopicProducer<TKey, TValue>
        where TValue : class
    {
        readonly ITopicProducer<TKey, TValue> _topicProducer;

        public CachedTopicProducer(T key, ITopicProducer<TKey, TValue> topicProducer)
        {
            _topicProducer = topicProducer;
            Key = key;
        }

        public event Action Used;

        public async ValueTask DisposeAsync()
        {
            switch (_topicProducer)
            {
                case IAsyncDisposable disposable:
                    await disposable.DisposeAsync().ConfigureAwait(false);
                    break;
            }
        }

        public T Key { get; }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            Used?.Invoke();
            return _topicProducer.ConnectSendObserver(observer);
        }

        public Task Produce(TKey key, TValue value, CancellationToken cancellationToken = default)
        {
            Used?.Invoke();
            return _topicProducer.Produce(key, value, cancellationToken);
        }

        public Task Produce(TKey key, TValue value, IPipe<KafkaSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken = default)
        {
            Used?.Invoke();
            return _topicProducer.Produce(key, value, pipe, cancellationToken);
        }

        public Task Produce(TKey key, object values, CancellationToken cancellationToken = default)
        {
            Used?.Invoke();
            return _topicProducer.Produce(key, values, cancellationToken);
        }

        public Task Produce(TKey key, object values, IPipe<KafkaSendContext<TKey, TValue>> pipe, CancellationToken cancellationToken = default)
        {
            Used?.Invoke();
            return _topicProducer.Produce(key, values, pipe, cancellationToken);
        }
    }
}
