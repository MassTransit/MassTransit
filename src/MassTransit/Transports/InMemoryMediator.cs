namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using Clients.Contexts;
    using Configuration;
    using Context;
    using GreenPipes;
    using InMemory.Configuration;
    using Pipeline;
    using Pipeline.Observables;


    /// <summary>
    /// Sends messages directly to the <see cref="IReceivePipe"/>, without serialization
    /// </summary>
    public class InMemoryMediator :
        IMediator,
        IPublishEndpointProvider
    {
        readonly IClientFactory _clientFactory;
        readonly IPublishEndpoint _publishEndpoint;
        readonly ISendEndpoint _responseEndpoint;
        readonly ISendEndpoint _endpoint;
        readonly SendObservable _sendObservable;
        readonly PublishObservable _publishObservable;

        public InMemoryMediator(ILogContext logContext, IReceiveEndpointConfiguration configuration, IReceivePipeDispatcher dispatcher,
            IInMemoryReceiveEndpointConfiguration responseConfiguration, IReceivePipeDispatcher responseDispatcher)
        {
            _sendObservable = new SendObservable();
            _publishObservable = new PublishObservable();

            _endpoint = new DispatcherSendEndpoint(configuration, dispatcher, logContext,
                _sendObservable, responseConfiguration, responseDispatcher);
            _publishEndpoint = new PublishEndpoint(this);

            _responseEndpoint = new DispatcherSendEndpoint(responseConfiguration, responseDispatcher, logContext, _sendObservable, configuration, dispatcher);

            var clientFactoryContext = new MediatorClientFactoryContext(_endpoint, responseConfiguration.ConsumePipe, responseConfiguration.InputAddress);
            _clientFactory = new ClientFactory(clientFactoryContext);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservable.Connect(observer);
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
            return _publishObservable.Connect(observer);
        }

        Task IPublishEndpoint.Publish<T>(T message, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, messageType, cancellationToken);
        }

        Task IPublishEndpoint.Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(message, messageType, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish<T>(values, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish(values, publishPipe, cancellationToken);
        }

        Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken)
        {
            return _publishEndpoint.Publish<T>(values, publishPipe, cancellationToken);
        }

        Task<ISendEndpoint> IPublishEndpointProvider.GetPublishSendEndpoint<T>()
            where T : class
        {
            return Task.FromResult(_endpoint);
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
