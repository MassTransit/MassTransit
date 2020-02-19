namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using ConsumePipeSpecifications;
    using GreenPipes;
    using Logging;
    using Pipeline;
    using Pipeline.Observables;
    using Topology;
    using Transports;


    public abstract class BaseReceiveEndpointContext :
        BasePipeContext,
        ReceiveEndpointContext
    {
        readonly ILogContext _logContext;
        readonly IPublishTopologyConfigurator _publishTopology;
        readonly Lazy<IPublishEndpointProvider> _publishEndpointProvider;
        readonly Lazy<IPublishPipe> _publishPipe;
        readonly Lazy<IReceivePipe> _receivePipe;
        readonly Lazy<ISendEndpointProvider> _sendEndpointProvider;
        readonly Lazy<ISendPipe> _sendPipe;
        readonly Lazy<IMessageSerializer> _serializer;
        readonly Lazy<ISendTransportProvider> _sendTransportProvider;
        readonly Lazy<IPublishTransportProvider> _publishTransportProvider;
        protected readonly PublishObservable PublishObservers;
        protected readonly SendObservable SendObservers;
        readonly ReceiveObservable _receiveObservers;
        readonly ReceiveTransportObservable _transportObservers;
        readonly ReceiveEndpointObservable _endpointObservers;

        protected BaseReceiveEndpointContext(IReceiveEndpointConfiguration configuration)
        {
            InputAddress = configuration.InputAddress;
            HostAddress = configuration.HostAddress;

            _publishTopology = configuration.Topology.Publish;

            ConsumePipeSpecification = configuration.Consume.Specification;

            _logContext = LogContext.Current.CreateLogContext(LogCategoryName.Transport.Receive);

            SendObservers = new SendObservable();
            PublishObservers = new PublishObservable();

            _endpointObservers = configuration.EndpointObservers;
            _receiveObservers = configuration.ReceiveObservers;
            _transportObservers = configuration.TransportObservers;

            Dependencies = configuration.Dependencies;

            _sendPipe = new Lazy<ISendPipe>(() => configuration.Send.CreatePipe());
            _publishPipe = new Lazy<IPublishPipe>(() => configuration.Publish.CreatePipe());
            _receivePipe = new Lazy<IReceivePipe>(configuration.CreateReceivePipe);

            _serializer = new Lazy<IMessageSerializer>(() => configuration.Serialization.Serializer);
            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
            _publishEndpointProvider = new Lazy<IPublishEndpointProvider>(CreatePublishEndpointProvider);
            _sendTransportProvider = new Lazy<ISendTransportProvider>(CreateSendTransportProvider);
            _publishTransportProvider = new Lazy<IPublishTransportProvider>(CreatePublishTransportProvider);
        }

        protected IPublishPipe PublishPipe => _publishPipe.Value;
        public ISendPipe SendPipe => _sendPipe.Value;
        public IMessageSerializer Serializer => _serializer.Value;

        protected Uri HostAddress { get; }

        public IConsumePipeSpecification ConsumePipeSpecification { get; }

        ReceiveObservable ReceiveEndpointContext.ReceiveObservers => _receiveObservers;

        IReceiveTransportObserver ReceiveEndpointContext.TransportObservers => _transportObservers;

        IReceiveEndpointObserver ReceiveEndpointContext.EndpointObservers => _endpointObservers;

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return SendObservers.Connect(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return PublishObservers.Connect(observer);
        }

        ConnectHandle IReceiveTransportObserverConnector.ConnectReceiveTransportObserver(IReceiveTransportObserver observer)
        {
            return _transportObservers.Connect(observer);
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservers.Connect(observer);
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointObservers.Connect(observer);
        }

        public Uri InputAddress { get; protected set; }

        public Task Dependencies { get; }

        ILogContext ReceiveEndpointContext.LogContext => _logContext;

        IPublishTopology ReceiveEndpointContext.Publish => _publishTopology;

        public IReceivePipe ReceivePipe => _receivePipe.Value;

        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider.Value;

        public IPublishEndpointProvider PublishEndpointProvider => _publishEndpointProvider.Value;

        public IReceivePipeDispatcher CreateReceivePipeDispatcher()
        {
            return new ReceivePipeDispatcher(_receivePipe.Value, _receiveObservers, _logContext);
        }

        protected virtual ISendEndpointProvider CreateSendEndpointProvider()
        {
            return new SendEndpointProvider(_sendTransportProvider.Value, SendObservers, Serializer, InputAddress, SendPipe);
        }

        protected virtual IPublishEndpointProvider CreatePublishEndpointProvider()
        {
            return new PublishEndpointProvider(_publishTransportProvider.Value, HostAddress, PublishObservers, Serializer, InputAddress, PublishPipe,
                _publishTopology);
        }

        protected abstract ISendTransportProvider CreateSendTransportProvider();

        protected abstract IPublishTransportProvider CreatePublishTransportProvider();

        protected ISendTransportProvider SendTransportProvider => _sendTransportProvider.Value;
    }
}
