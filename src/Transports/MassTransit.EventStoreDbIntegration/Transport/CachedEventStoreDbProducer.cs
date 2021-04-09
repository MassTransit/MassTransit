using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using GreenPipes.Caching;

namespace MassTransit.EventStoreDbIntegration
{
    public class CachedEventStoreDbProducer<TKey> :
        IEventStoreDbProducer,
        INotifyValueUsed,
        IAsyncDisposable
    {
        readonly IEventStoreDbProducer _producer;

        public CachedEventStoreDbProducer(TKey key, IEventStoreDbProducer producer)
        {
            Key = key;
            _producer = producer;
        }

        public TKey Key { get; }

        public async ValueTask DisposeAsync()
        {
            switch (_producer)
            {
                case IAsyncDisposable disposable:
                    await disposable.DisposeAsync().ConfigureAwait(false);
                    break;

                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            Used?.Invoke();
            return _producer.ConnectSendObserver(observer);
        }

        public Task Produce<T>(T message, CancellationToken cancellationToken = default)
            where T : class
        {
            Used?.Invoke();
            return _producer.Produce(message, cancellationToken);
        }

        public Task Produce<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default)
            where T : class
        {
            Used?.Invoke();
            return _producer.Produce(messages, cancellationToken);
        }

        public Task Produce<T>(T message, IPipe<EventStoreDbSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            Used?.Invoke();
            return _producer.Produce(message, pipe, cancellationToken);
        }

        public Task Produce<T>(IEnumerable<T> messages, IPipe<EventStoreDbSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            Used?.Invoke();
            return _producer.Produce(messages, pipe, cancellationToken);
        }

        public Task Produce<T>(object values, CancellationToken cancellationToken = default)
            where T : class
        {
            Used?.Invoke();
            return _producer.Produce<T>(values, cancellationToken);
        }

        public Task Produce<T>(IEnumerable<object> values, CancellationToken cancellationToken = default)
            where T : class
        {
            Used?.Invoke();
            return _producer.Produce<T>(values, cancellationToken);
        }

        public Task Produce<T>(object values, IPipe<EventStoreDbSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            Used?.Invoke();
            return _producer.Produce(values, pipe, cancellationToken);
        }

        public Task Produce<T>(IEnumerable<object> values, IPipe<EventStoreDbSendContext<T>> pipe, CancellationToken cancellationToken = default)
            where T : class
        {
            Used?.Invoke();
            return _producer.Produce(values, pipe, cancellationToken);
        }

        public event Action Used;
    }
}
