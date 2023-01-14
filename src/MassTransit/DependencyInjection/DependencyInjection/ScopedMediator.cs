namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using Context;
    using Mediator;
    using Transports;


    public class ScopedMediator :
        SendEndpointProxy,
        IScopedMediator
    {
        readonly IMediator _mediator;
        readonly IServiceProvider _provider;
        IClientFactory _clientFactory;

        public ScopedMediator(IMediator mediator, IServiceProvider provider)
            : base(mediator)
        {
            _mediator = mediator;
            _provider = provider;
        }

        IClientFactory ClientFactory => _clientFactory ??= new ClientFactory(new ScopedClientFactoryContext(_mediator, _provider));

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _mediator.ConnectPublishObserver(observer);
        }

        public async Task<ISendEndpoint> GetPublishSendEndpoint<T>()
            where T : class
        {
            var endpoint = await _mediator.GetPublishSendEndpoint<T>().ConfigureAwait(false);
            return new ScopedSendEndpoint(endpoint, _provider);
        }

        public Task Publish<T>(T message, CancellationToken cancellationToken = default)
            where T : class
        {
            return PublishInternal(cancellationToken, message);
        }

        public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return PublishInternal(cancellationToken, message, publishPipe);
        }

        public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return PublishInternal(cancellationToken, message, publishPipe);
        }

        public Task Publish(object message, CancellationToken cancellationToken = default)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();
            return PublishEndpointConverterCache.Publish(this, message, messageType, cancellationToken);
        }

        public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageType = message.GetType();
            return PublishEndpointConverterCache.Publish(this, message, messageType, publishPipe, cancellationToken);
        }

        public Task Publish(object message, Type messageType, CancellationToken cancellationToken = default)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return PublishEndpointConverterCache.Publish(this, message, messageType, cancellationToken);
        }

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            return PublishEndpointConverterCache.Publish(this, message, messageType, publishPipe, cancellationToken);
        }

        public Task Publish<T>(object values, CancellationToken cancellationToken = default)
            where T : class
        {
            return PublishInternal<T>(cancellationToken, values);
        }

        public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return PublishInternal(cancellationToken, values, publishPipe);
        }

        public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return PublishInternal<T>(cancellationToken, values, publishPipe);
        }

        public ClientFactoryContext Context => ClientFactory.Context;

        public RequestHandle<T> CreateRequest<T>(T message, CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T : class
        {
            return ClientFactory.CreateRequest(message, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(Uri destinationAddress, T message, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class
        {
            return ClientFactory.CreateRequest(destinationAddress, message, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, T message, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class
        {
            return ClientFactory.CreateRequest(consumeContext, message, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, Uri destinationAddress, T message,
            CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class
        {
            return ClientFactory.CreateRequest(consumeContext, destinationAddress, message, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default)
            where T : class
        {
            return ClientFactory.CreateRequest<T>(values, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(Uri destinationAddress, object values, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class
        {
            return ClientFactory.CreateRequest<T>(destinationAddress, values, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, object values, CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class
        {
            return ClientFactory.CreateRequest<T>(consumeContext, values, cancellationToken, timeout);
        }

        public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, Uri destinationAddress, object values,
            CancellationToken cancellationToken = default,
            RequestTimeout timeout = default)
            where T : class
        {
            return ClientFactory.CreateRequest<T>(consumeContext, destinationAddress, values, cancellationToken, timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(RequestTimeout timeout = default)
            where T : class
        {
            return ClientFactory.CreateRequestClient<T>(timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, RequestTimeout timeout = default)
            where T : class
        {
            return ClientFactory.CreateRequestClient<T>(consumeContext, timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class
        {
            return ClientFactory.CreateRequestClient<T>(destinationAddress, timeout);
        }

        public IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, Uri destinationAddress, RequestTimeout timeout = default)
            where T : class
        {
            return ClientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _mediator.ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
            where T : class
        {
            return _mediator.ConnectConsumePipe(pipe, options);
        }

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _mediator.ConnectRequestPipe(requestId, pipe);
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _mediator.ConnectConsumeObserver(observer);
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class
        {
            return _mediator.ConnectConsumeMessageObserver(observer);
        }

        protected override IPipe<SendContext<T>> GetPipeProxy<T>(IPipe<SendContext<T>> pipe = default)
        {
            return new ScopedSendPipeAdapter<T>(_provider, pipe);
        }

        Task PublishInternal<T>(CancellationToken cancellationToken, T message, IPipe<PublishContext<T>> pipe = null)
            where T : class
        {
            Task<ISendEndpoint> sendEndpointTask = GetPublishSendEndpoint<T>();
            if (sendEndpointTask.Status == TaskStatus.RanToCompletion)
            {
                var sendEndpoint = sendEndpointTask.Result;

                return pipe != null && pipe.IsNotEmpty()
                    ? sendEndpoint.Send(message, new PublishSendPipeAdapter<T>(pipe), cancellationToken)
                    : sendEndpoint.Send(message, cancellationToken);
            }

            async Task PublishAsync()
            {
                var sendEndpoint = await sendEndpointTask.ConfigureAwait(false);

                if (pipe != null && pipe.IsNotEmpty())
                    await sendEndpoint.Send(message, new PublishSendPipeAdapter<T>(pipe), cancellationToken).ConfigureAwait(false);
                else
                    await sendEndpoint.Send(message, cancellationToken).ConfigureAwait(false);
            }

            return PublishAsync();
        }

        Task PublishInternal<T>(CancellationToken cancellationToken, object values, IPipe<PublishContext<T>> pipe = null)
            where T : class
        {
            Task<ISendEndpoint> sendEndpointTask = GetPublishSendEndpoint<T>();
            if (sendEndpointTask.Status == TaskStatus.RanToCompletion)
            {
                var sendEndpoint = sendEndpointTask.Result;

                return pipe != null && pipe.IsNotEmpty()
                    ? sendEndpoint.Send(values, new PublishSendPipeAdapter<T>(pipe), cancellationToken)
                    : sendEndpoint.Send<T>(values, cancellationToken);
            }

            async Task PublishAsync()
            {
                var sendEndpoint = await sendEndpointTask.ConfigureAwait(false);

                if (pipe != null && pipe.IsNotEmpty())
                    await sendEndpoint.Send(values, new PublishSendPipeAdapter<T>(pipe), cancellationToken).ConfigureAwait(false);
                else
                    await sendEndpoint.Send<T>(values, cancellationToken).ConfigureAwait(false);
            }

            return PublishAsync();
        }
    }
}
