namespace MassTransit.Transactions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Topology;
    using Transports;


    public abstract class BaseTransactionalBus :
        IBus
    {
        readonly IBus _bus;
        readonly IPublishEndpoint _publishEndpoint;
        readonly TransactionalBusPublishEndpointProvider _publishEndpointProvider;

        public BaseTransactionalBus(IBus bus)
        {
            _bus = bus;

            _publishEndpointProvider = new TransactionalBusPublishEndpointProvider(this, bus);
            _publishEndpoint = new PublishEndpoint(_publishEndpointProvider);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _bus.ConnectPublishObserver(observer);
        }

        public Task<ISendEndpoint> GetPublishSendEndpoint<T>()
            where T : class
        {
            return _publishEndpointProvider.GetPublishSendEndpoint<T>();
        }

        public Task Publish<T>(T message, CancellationToken cancellationToken = default)
            where T : class
        {
            return Add(() => _publishEndpoint.Publish(message, cancellationToken));
        }

        public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return Add(() => _publishEndpoint.Publish(message, publishPipe, cancellationToken));
        }

        public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return Add(() => _publishEndpoint.Publish(message, publishPipe, cancellationToken));
        }

        public Task Publish(object message, CancellationToken cancellationToken = default)
        {
            return Add(() => _publishEndpoint.Publish(message, cancellationToken));
        }

        public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
        {
            return Add(() => _publishEndpoint.Publish(message, publishPipe, cancellationToken));
        }

        public Task Publish(object message, Type messageType, CancellationToken cancellationToken = default)
        {
            return Add(() => _publishEndpoint.Publish(message, messageType, cancellationToken));
        }

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
        {
            return Add(() => _publishEndpoint.Publish(message, messageType, publishPipe, cancellationToken));
        }

        public Task Publish<T>(object values, CancellationToken cancellationToken = default)
            where T : class
        {
            return Add(() => _publishEndpoint.Publish<T>(values, cancellationToken));
        }

        public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return Add(() => _publishEndpoint.Publish(values, publishPipe, cancellationToken));
        }

        public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return Add(() => _publishEndpoint.Publish<T>(values, publishPipe, cancellationToken));
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _bus.ConnectSendObserver(observer);
        }

        public async Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return new TransactionalBusSendEndpoint(this, await _bus.GetSendEndpoint(address));
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _bus.ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
            where T : class
        {
            return _bus.ConnectConsumePipe(pipe, options);
        }

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _bus.ConnectRequestPipe(requestId, pipe);
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class
        {
            return _bus.ConnectConsumeMessageObserver(observer);
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _bus.ConnectConsumeObserver(observer);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _bus.ConnectReceiveObserver(observer);
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _bus.ConnectReceiveEndpointObserver(observer);
        }

        public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return _bus.ConnectEndpointConfigurationObserver(observer);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null)
        {
            return _bus.ConnectReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            return _bus.ConnectReceiveEndpoint(queueName, configureEndpoint);
        }

        public void Probe(ProbeContext context)
        {
            _bus.Probe(context);
        }

        public Uri Address => _bus.Address;
        public IBusTopology Topology => _bus.Topology;

        public abstract Task Add(Func<Task> action);
    }
}
