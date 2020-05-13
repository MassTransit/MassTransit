namespace MassTransit.Mediator
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using Configuration;
    using Context;
    using Context.Converters;
    using Contexts;
    using Endpoints;
    using GreenPipes;
    using Pipeline;
    using Pipeline.Observables;
    using Transports;


    /// <summary>
    /// Sends messages directly to the <see cref="IReceivePipe"/>, without serialization
    /// </summary>
    public class MassTransitMediator :
        IMediator
    {
        readonly IClientFactory _clientFactory;
        readonly MediatorSendEndpoint _endpoint;

        public MassTransitMediator(ILogContext logContext, IReceiveEndpointConfiguration configuration, IReceivePipeDispatcher dispatcher,
            IReceiveEndpointConfiguration responseConfiguration, IReceivePipeDispatcher responseDispatcher)
        {
            var sendObservable = new SendObservable();

            _endpoint = new MediatorSendEndpoint(configuration, dispatcher, logContext, sendObservable, responseConfiguration, responseDispatcher);

            var clientFactoryContext = new MediatorClientFactoryContext(_endpoint, responseConfiguration.ConsumePipe, responseConfiguration.InputAddress);
            _clientFactory = new ClientFactory(clientFactoryContext);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _endpoint.ConnectSendObserver(observer);
        }

        Task ISendEndpoint.Send<T>(T message, CancellationToken cancellationToken)
        {
            return _endpoint.Send(message, cancellationToken);
        }

        Task ISendEndpoint.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            return _endpoint.Send(message, pipe, cancellationToken);
        }

        Task ISendEndpoint.Send<T>(T message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return _endpoint.Send(message, pipe, cancellationToken);
        }

        Task ISendEndpoint.Send(object message, CancellationToken cancellationToken)
        {
            return _endpoint.Send(message, cancellationToken);
        }

        Task ISendEndpoint.Send(object message, Type messageType, CancellationToken cancellationToken)
        {
            return _endpoint.Send(message, messageType, cancellationToken);
        }

        Task ISendEndpoint.Send(object message, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return _endpoint.Send(message, pipe, cancellationToken);
        }

        Task ISendEndpoint.Send(object message, Type messageType, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return _endpoint.Send(message, messageType, pipe, cancellationToken);
        }

        Task ISendEndpoint.Send<T>(object values, CancellationToken cancellationToken)
        {
            return _endpoint.Send<T>(values, cancellationToken);
        }

        Task ISendEndpoint.Send<T>(object values, IPipe<SendContext<T>> pipe, CancellationToken cancellationToken)
        {
            return _endpoint.Send(values, pipe, cancellationToken);
        }

        Task ISendEndpoint.Send<T>(object values, IPipe<SendContext> pipe, CancellationToken cancellationToken)
        {
            return _endpoint.Send<T>(values, pipe, cancellationToken);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
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

        async Task PublishInternal<T>(CancellationToken cancellationToken, T message, IPipe<PublishContext<T>> pipe = default)
            where T : class
        {
            var sendEndpoint = await _endpoint.GetPublishSendEndpoint<T>().ConfigureAwait(false);

            if (pipe.IsNotEmpty())
                await sendEndpoint.Send(message, new PublishContextPipeAdapter<T>(pipe), cancellationToken).ConfigureAwait(false);
            else
                await sendEndpoint.Send(message, cancellationToken).ConfigureAwait(false);
        }

        async Task PublishInternal<T>(CancellationToken cancellationToken, object values, IPipe<PublishContext<T>> pipe = default)
            where T : class
        {
            var sendEndpoint = await _endpoint.GetPublishSendEndpoint<T>().ConfigureAwait(false);

            if (pipe.IsNotEmpty())
                await sendEndpoint.Send(values, new PublishContextPipeAdapter<T>(pipe), cancellationToken).ConfigureAwait(false);
            else
                await sendEndpoint.Send<T>(values, cancellationToken).ConfigureAwait(false);
        }

        Task IAsyncDisposable.DisposeAsync(CancellationToken cancellationToken)
        {
            return _clientFactory.DisposeAsync(cancellationToken);
        }

        RequestHandle<T> IClientFactory.CreateRequest<T>(T message, CancellationToken cancellationToken, RequestTimeout timeout)
        {
            return _clientFactory.CreateRequest(message, cancellationToken, timeout);
        }

        RequestHandle<T> IClientFactory.CreateRequest<T>(Uri destinationAddress, T message, CancellationToken cancellationToken, RequestTimeout timeout)
        {
            return _clientFactory.CreateRequest(destinationAddress, message, cancellationToken, timeout);
        }

        RequestHandle<T> IClientFactory.CreateRequest<T>(ConsumeContext consumeContext, T message, CancellationToken cancellationToken, RequestTimeout timeout)
        {
            return _clientFactory.CreateRequest(consumeContext, message, cancellationToken, timeout);
        }

        RequestHandle<T> IClientFactory.CreateRequest<T>(ConsumeContext consumeContext, Uri destinationAddress, T message, CancellationToken cancellationToken,
            RequestTimeout timeout)
        {
            return _clientFactory.CreateRequest(consumeContext, destinationAddress, message, cancellationToken, timeout);
        }

        IRequestClient<T> IClientFactory.CreateRequestClient<T>(RequestTimeout timeout)
        {
            return _clientFactory.CreateRequestClient<T>(timeout);
        }

        IRequestClient<T> IClientFactory.CreateRequestClient<T>(ConsumeContext consumeContext, RequestTimeout timeout)
        {
            return _clientFactory.CreateRequestClient<T>(consumeContext, timeout);
        }

        IRequestClient<T> IClientFactory.CreateRequestClient<T>(Uri destinationAddress, RequestTimeout timeout)
        {
            return _clientFactory.CreateRequestClient<T>(destinationAddress, timeout);
        }

        IRequestClient<T> IClientFactory.CreateRequestClient<T>(ConsumeContext consumeContext, Uri destinationAddress, RequestTimeout timeout)
        {
            return _clientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout);
        }

        ClientFactoryContext IClientFactory.Context => _clientFactory.Context;
    }
}
