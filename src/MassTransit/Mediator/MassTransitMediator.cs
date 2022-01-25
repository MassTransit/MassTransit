namespace MassTransit.Mediator
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using Configuration;
    using Context;
    using Contexts;
    using Logging;
    using Observables;
    using Transports;
    using Util;


    /// <summary>
    /// Sends messages directly to the <see cref="IReceivePipe" />, without serialization
    /// </summary>
    public class MassTransitMediator :
        IMediator,
        IAsyncDisposable
    {
        readonly ClientFactory _clientFactory;
        readonly IReceivePipeDispatcher _dispatcher;
        readonly MediatorSendEndpoint _endpoint;
        readonly IReceivePipeDispatcher _responseDispatcher;

        public MassTransitMediator(ILogContext logContext, IReceiveEndpointConfiguration configuration, IReceivePipeDispatcher dispatcher,
            IReceiveEndpointConfiguration responseConfiguration, IReceivePipeDispatcher responseDispatcher)
        {
            _responseDispatcher = responseDispatcher;
            _dispatcher = dispatcher;
            var sendObservable = new SendObservable();

            _endpoint = new MediatorSendEndpoint(configuration, dispatcher, logContext, sendObservable, responseConfiguration, responseDispatcher);

            var clientFactoryContext = new MediatorClientFactoryContext(_endpoint, responseConfiguration.ConsumePipe, responseConfiguration.InputAddress);
            _clientFactory = new ClientFactory(clientFactoryContext);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _endpoint.ConnectSendObserver(observer);
        }

        public Task Send<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            return _endpoint.Send(message, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            return _endpoint.Send(message, pipe, cancellationToken);
        }

        public Task Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            return _endpoint.Send(message, pipe, cancellationToken);
        }

        public Task Send(object message, CancellationToken cancellationToken)
        {
            return _endpoint.Send(message, cancellationToken);
        }

        public Task Send(object message, Type messageType, CancellationToken cancellationToken)
        {
            return _endpoint.Send(message, messageType, cancellationToken);
        }

        public Task Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return _endpoint.Send(message, pipe, cancellationToken);
        }

        public Task Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return _endpoint.Send(message, messageType, pipe, cancellationToken);
        }

        public Task Send<T>(object values, CancellationToken cancellationToken)
            where T : class
        {
            return _endpoint.Send<T>(values, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
            where T : class
        {
            return _endpoint.Send(values, pipe, cancellationToken);
        }

        public Task Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
            where T : class
        {
            return _endpoint.Send<T>(values, pipe, cancellationToken);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _endpoint.ConnectPublishObserver(observer);
        }

        public Task Publish<T>(T message, CancellationToken cancellationToken)
            where T : class
        {
            return PublishInternal(cancellationToken, message);
        }

        public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            return PublishInternal(cancellationToken, message, publishPipe);
        }

        public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            return PublishInternal(cancellationToken, message, publishPipe);
        }

        public Task Publish(object message, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            return PublishEndpointConverterCache.Publish(this, message, messageType, cancellationToken);
        }

        public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();

            return PublishEndpointConverterCache.Publish(this, message, messageType, publishPipe, cancellationToken);
        }

        public Task Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return PublishEndpointConverterCache.Publish(this, message, messageType, cancellationToken);
        }

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return PublishEndpointConverterCache.Publish(this, message, messageType, publishPipe, cancellationToken);
        }

        public Task Publish<T>(object values, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return PublishInternal<T>(cancellationToken, values);
        }

        public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return PublishInternal(cancellationToken, values, publishPipe);
        }

        public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
            where T : class
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return PublishInternal<T>(cancellationToken, values, publishPipe);
        }

        public ValueTask DisposeAsync()
        {
            return _clientFactory.DisposeAsync();
        }

        public RequestHandle<T> CreateRequest<T>(T message, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            return _clientFactory.CreateRequest(message, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(Uri destinationAddress, T message, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            return _clientFactory.CreateRequest(destinationAddress, message, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, T message, CancellationToken cancellationToken, RequestTimeout timeout)
            where T : class
        {
            return _clientFactory.CreateRequest(consumeContext, message, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, Uri destinationAddress, T message, CancellationToken cancellationToken,
            RequestTimeout timeout)
            where T : class
        {
            return _clientFactory.CreateRequest(consumeContext, destinationAddress, message, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T : class
        {
            return _clientFactory.CreateRequest<T>(values, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(Uri destinationAddress, object values, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class
        {
            return _clientFactory.CreateRequest<T>(destinationAddress, values, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, object values, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class
        {
            return _clientFactory.CreateRequest<T>(consumeContext, values, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, Uri destinationAddress, object values,
            CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class
        {
            return _clientFactory.CreateRequest<T>(consumeContext, destinationAddress, values, cancellationToken, timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(RequestTimeout timeout)
            where T : class
        {
            return _clientFactory.CreateRequestClient<T>(timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, RequestTimeout timeout)
            where T : class
        {
            return _clientFactory.CreateRequestClient<T>(consumeContext, timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
            where T : class
        {
            return _clientFactory.CreateRequestClient<T>(destinationAddress, timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, Uri destinationAddress, RequestTimeout timeout)
            where T : class
        {
            return _clientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout);
        }

        public ClientFactoryContext Context => _clientFactory.Context;

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _dispatcher.ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
            where T : class
        {
            return _dispatcher.ConnectConsumePipe(pipe, options);
        }

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _dispatcher.ConnectRequestPipe(requestId, pipe);
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return new MultipleConnectHandle(_dispatcher.ConnectConsumeObserver(observer), _responseDispatcher.ConnectConsumeObserver(observer));
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class
        {
            return new MultipleConnectHandle(_dispatcher.ConnectConsumeMessageObserver(observer), _responseDispatcher.ConnectConsumeMessageObserver(observer));
        }

        async Task PublishInternal<T>(CancellationToken cancellationToken, T message, IPipe<PublishContext<T>> pipe = default)
            where T : class
        {
            var sendEndpoint = await _endpoint.GetPublishSendEndpoint<T>().ConfigureAwait(false);

            if (pipe.IsNotEmpty())
                await sendEndpoint.Send(message, new PublishSendPipeAdapter<T>(pipe), cancellationToken).ConfigureAwait(false);
            else
                await sendEndpoint.Send(message, cancellationToken).ConfigureAwait(false);
        }

        async Task PublishInternal<T>(CancellationToken cancellationToken, object values, IPipe<PublishContext<T>> pipe = default)
            where T : class
        {
            var sendEndpoint = await _endpoint.GetPublishSendEndpoint<T>().ConfigureAwait(false);

            if (pipe.IsNotEmpty())
                await sendEndpoint.Send(values, new PublishSendPipeAdapter<T>(pipe), cancellationToken).ConfigureAwait(false);
            else
                await sendEndpoint.Send<T>(values, cancellationToken).ConfigureAwait(false);
        }
    }
}
