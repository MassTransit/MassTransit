namespace MassTransit.Context
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using ConsumePipeSpecifications;
    using GreenPipes;
    using GreenPipes.Agents;
    using Pipeline;
    using Pipeline.Observables;
    using Topology;
    using Transports;

    public abstract class BaseReceiveEndpointContext :
        BasePipeContext,
        ReceiveEndpointContext
    {
        readonly IHostConfiguration _hostConfiguration;
        readonly ReceiveEndpointObservable _endpointObservers;
        readonly Lazy<IPublishPipe> _publishPipe;
        readonly IPublishTopologyConfigurator _publishTopology;
        readonly ReceiveObservable _receiveObservers;
        readonly Lazy<IReceivePipe> _receivePipe;
        readonly Lazy<ISendPipe> _sendPipe;
        readonly Lazy<IMessageSerializer> _serializer;
        readonly ReceiveTransportObservable _transportObservers;

        protected readonly PublishObservable PublishObservers;
        protected readonly SendObservable SendObservers;

        Lazy<IPublishEndpointProvider> _publishEndpointProvider;
        Lazy<IPublishTransportProvider> _publishTransportProvider;
        Lazy<ISendEndpointProvider> _sendEndpointProvider;
        Lazy<ISendTransportProvider> _sendTransportProvider;

        protected BaseReceiveEndpointContext(IHostConfiguration hostConfiguration, IReceiveEndpointConfiguration configuration)
        {
            _hostConfiguration = hostConfiguration;

            InputAddress = configuration.InputAddress;
            HostAddress = configuration.HostAddress;
            PublishFaults = configuration.PublishFaults;
            PrefetchCount = configuration.PrefetchCount;
            ConcurrentMessageLimit = configuration.ConcurrentMessageLimit;

            _publishTopology = configuration.Topology.Publish;

            ConsumePipeSpecification = configuration.Consume.Specification;

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

            Reset();

            hostConfiguration.ConnectReceiveEndpointContext(this);
        }

        protected Uri HostAddress { get; }

        protected IPublishPipe PublishPipe => _publishPipe.Value;
        public ISendPipe SendPipe => _sendPipe.Value;
        public IMessageSerializer Serializer => _serializer.Value;

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

        public Uri InputAddress { get; }

        public Task Dependencies { get; }

        public bool PublishFaults { get; }
        public int PrefetchCount { get; }
        public int? ConcurrentMessageLimit { get; }

        ILogContext ReceiveEndpointContext.LogContext => _hostConfiguration.ReceiveLogContext;

        IPublishTopology ReceiveEndpointContext.Publish => _publishTopology;

        public IReceivePipe ReceivePipe => _receivePipe.Value;

        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider.Value;

        public IPublishEndpointProvider PublishEndpointProvider => _publishEndpointProvider.Value;

        public IReceivePipeDispatcher CreateReceivePipeDispatcher()
        {
            return new ReceivePipeDispatcher(_receivePipe.Value, _receiveObservers, _hostConfiguration);
        }

        public void Reset()
        {
            _sendTransportProvider = new Lazy<ISendTransportProvider>(CreateDecoratedSendTransportProvider);
            _publishTransportProvider = new Lazy<IPublishTransportProvider>(CreateDecoratedPublishTransportProvider);

            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
            _publishEndpointProvider = new Lazy<IPublishEndpointProvider>(CreatePublishEndpointProvider);
        }

        public abstract void AddConsumeAgent(IAgent agent);

        public virtual void Probe(ProbeContext context)
        {
        }

        public abstract Exception ConvertException(Exception exception, string message);

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
        protected abstract ISendTransportProvider CreateDecoratedSendTransportProvider();

        protected abstract IPublishTransportProvider CreatePublishTransportProvider();
        protected abstract IPublishTransportProvider CreateDecoratedPublishTransportProvider();
    }
}
