namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// When configuring multiple bus instances in a single container (MultiBus), this base class should be used
    /// as a the base for the additional bus instance type.
    /// </summary>
    /// <typeparam name="TBus">The specific bus interface type for this bus instance</typeparam>
    // ReSharper disable once UnusedTypeParameter
    public abstract class BusInstance<TBus> :
        IBusControl
        where TBus : class, IBus
    {
        readonly IBusControl _busControl;

        protected BusInstance(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _busControl.ConnectPublishObserver(observer);
        }

        public Task Publish<T>(T message, CancellationToken cancellationToken = default)
            where T : class
        {
            return _busControl.Publish(message, cancellationToken);
        }

        public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return _busControl.Publish(message, publishPipe, cancellationToken);
        }

        public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return _busControl.Publish(message, publishPipe, cancellationToken);
        }

        public Task Publish(object message, CancellationToken cancellationToken = default)
        {
            return _busControl.Publish(message, cancellationToken);
        }

        public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
        {
            return _busControl.Publish(message, publishPipe, cancellationToken);
        }

        public Task Publish(object message, Type messageType, CancellationToken cancellationToken = default)
        {
            return _busControl.Publish(message, messageType, cancellationToken);
        }

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
        {
            return _busControl.Publish(message, messageType, publishPipe, cancellationToken);
        }

        public Task Publish<T>(object values, CancellationToken cancellationToken = default)
            where T : class
        {
            return _busControl.Publish<T>(values, cancellationToken);
        }

        public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return _busControl.Publish(values, publishPipe, cancellationToken);
        }

        public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default)
            where T : class
        {
            return _busControl.Publish<T>(values, publishPipe, cancellationToken);
        }

        public Task<ISendEndpoint> GetPublishSendEndpoint<T>()
            where T : class
        {
            return _busControl.GetPublishSendEndpoint<T>();
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _busControl.ConnectSendObserver(observer);
        }

        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return _busControl.GetSendEndpoint(address);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _busControl.ConnectConsumePipe(pipe);
        }

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
            where T : class
        {
            return _busControl.ConnectConsumePipe(pipe, options);
        }

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe)
            where T : class
        {
            return _busControl.ConnectRequestPipe(requestId, pipe);
        }

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
            where T : class
        {
            return _busControl.ConnectConsumeMessageObserver(observer);
        }

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _busControl.ConnectConsumeObserver(observer);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _busControl.ConnectReceiveObserver(observer);
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _busControl.ConnectReceiveEndpointObserver(observer);
        }

        public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return _busControl.ConnectEndpointConfigurationObserver(observer);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter,
            Action<IReceiveEndpointConfigurator>? configureEndpoint = null)
        {
            return _busControl.ConnectReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator>? configureEndpoint)
        {
            return _busControl.ConnectReceiveEndpoint(queueName, configureEndpoint);
        }

        public void Probe(ProbeContext context)
        {
            _busControl.Probe(context);
        }

        public Uri Address => _busControl.Address;

        public IBusTopology Topology => _busControl.Topology;

        public Task<BusHandle> StartAsync(CancellationToken cancellationToken = default)
        {
            return _busControl.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return _busControl.StopAsync(cancellationToken);
        }

        public BusHealthResult CheckHealth()
        {
            return _busControl.CheckHealth();
        }
    }
}
