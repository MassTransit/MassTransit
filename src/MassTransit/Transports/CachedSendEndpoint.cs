namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Caching;


    public class CachedSendEndpoint<TKey> :
        ISendEndpoint,
        INotifyValueUsed,
        IAsyncDisposable
    {
        readonly ISendEndpoint _endpoint;

        public CachedSendEndpoint(TKey key, ISendEndpoint endpoint)
        {
            Key = key;
            _endpoint = endpoint;
        }

        public TKey Key { get; }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            switch (_endpoint)
            {
                case IAsyncDisposable disposable:
                    await disposable.DisposeAsync().ConfigureAwait(false);
                    break;
            }
        }

        public event Action Used;

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            Used?.Invoke();
            return _endpoint.ConnectSendObserver(observer);
        }

        public Task Send<T>(T message, CancellationToken cancellationToken = new CancellationToken())
            where T : class
        {
            Used?.Invoke();
            return _endpoint.Send(message, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = new CancellationToken())
            where T : class
        {
            Used?.Invoke();
            return _endpoint.Send(message, pipe, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken = new CancellationToken())
            where T : class
        {
            Used?.Invoke();
            return _endpoint.Send(message, pipe, cancellationToken);
        }

        public Task Send(object message, CancellationToken cancellationToken = new CancellationToken())
        {
            Used?.Invoke();
            return _endpoint.Send(message, cancellationToken);
        }

        public Task Send(object message, Type messageType, CancellationToken cancellationToken = new CancellationToken())
        {
            Used?.Invoke();
            return _endpoint.Send(message, messageType, cancellationToken);
        }

        public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken = new CancellationToken())
        {
            Used?.Invoke();
            return _endpoint.Send(message, pipe, cancellationToken);
        }

        public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken = new CancellationToken())
        {
            Used?.Invoke();
            return _endpoint.Send(message, messageType, pipe, cancellationToken);
        }

        public Task Send<T>(object values, CancellationToken cancellationToken = new CancellationToken())
            where T : class
        {
            Used?.Invoke();
            return _endpoint.Send<T>(values, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken = new CancellationToken())
            where T : class
        {
            Used?.Invoke();
            return _endpoint.Send(values, pipe, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken = new CancellationToken())
            where T : class
        {
            Used?.Invoke();
            return _endpoint.Send<T>(values, pipe, cancellationToken);
        }
    }
}
