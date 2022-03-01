#nullable enable
namespace MassTransit.Transports
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Logging;
    using Middleware;
    using Observables;


    public abstract class BaseReceiveEndpointContext :
        BasePipeContext,
        ReceiveEndpointContext
    {
        readonly ReceiveEndpointObservable _endpointObservers;
        readonly IHostConfiguration _hostConfiguration;
        readonly PublishObservable _publishObservers;
        readonly Lazy<IPublishPipe> _publishPipe;
        readonly IPublishTopologyConfigurator _publishTopology;
        readonly ReceiveObservable _receiveObservers;
        readonly Lazy<IReceivePipe> _receivePipe;
        readonly SendObservable _sendObservers;
        readonly Lazy<ISendPipe> _sendPipe;
        readonly ReceiveTransportObservable _transportObservers;

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

            _sendObservers = new SendObservable();
            _publishObservers = new PublishObservable();

            _endpointObservers = configuration.EndpointObservers;
            _receiveObservers = configuration.ReceiveObservers;
            _transportObservers = configuration.TransportObservers;

            Dependencies = configuration.Dependencies;

            Serialization = configuration.Serialization.CreateSerializerCollection();

            _sendPipe = new Lazy<ISendPipe>(() => configuration.Send.CreatePipe());
            _publishPipe = new Lazy<IPublishPipe>(() => configuration.Publish.CreatePipe());
            _receivePipe = new Lazy<IReceivePipe>(configuration.CreateReceivePipe);

            _sendTransportProvider = new Lazy<ISendTransportProvider>(CreateSendTransportProvider);
            _publishTransportProvider = new Lazy<IPublishTransportProvider>(CreatePublishTransportProvider);

            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
            _publishEndpointProvider = new Lazy<IPublishEndpointProvider>(CreatePublishEndpointProvider);

            hostConfiguration.ConnectReceiveEndpointContext(this);
        }

        Uri HostAddress { get; }

        IPublishPipe PublishPipe => _publishPipe.Value;
        ISendPipe SendPipe => _sendPipe.Value;

        public IReceiveObserver ReceiveObservers => _receiveObservers;

        public IReceiveTransportObserver TransportObservers => _transportObservers;

        public IReceiveEndpointObserver EndpointObservers => _endpointObservers;

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservers.Connect(observer);
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
            return _publishObservers.Connect(observer);
        }

        public ConnectHandle ConnectReceiveTransportObserver(IReceiveTransportObserver observer)
        {
            return _transportObservers.Connect(observer);
        }

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveObservers.Connect(observer);
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointObservers.Connect(observer);
        }

        public Uri InputAddress { get; }

        public Task Dependencies { get; }

        public bool PublishFaults { get; }

        public int PrefetchCount { get; }
        public int? ConcurrentMessageLimit { get; }

        public ILogContext LogContext => _hostConfiguration.ReceiveLogContext ?? throw new InvalidOperationException("ReceiveLogContext should not be null");

        public IPublishTopology Publish => _publishTopology;

        public IReceivePipe ReceivePipe => _receivePipe.Value;

        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider.Value;

        public IPublishEndpointProvider PublishEndpointProvider => _publishEndpointProvider.Value;

        public IReceivePipeDispatcher CreateReceivePipeDispatcher()
        {
            return new ReceivePipeDispatcher(_receivePipe.Value, _receiveObservers, _hostConfiguration, InputAddress);
        }

        public void Reset()
        {
            _sendTransportProvider = new Lazy<ISendTransportProvider>(CreateSendTransportProvider);
            _publishTransportProvider = new Lazy<IPublishTransportProvider>(CreatePublishTransportProvider);

            _sendEndpointProvider = new Lazy<ISendEndpointProvider>(CreateSendEndpointProvider);
            _publishEndpointProvider = new Lazy<IPublishEndpointProvider>(CreatePublishEndpointProvider);
        }

        public abstract void AddSendAgent(IAgent agent);
        public abstract void AddConsumeAgent(IAgent agent);

        public virtual void Probe(ProbeContext context)
        {
        }

        public abstract Exception ConvertException(Exception exception, string message);

        public ISerialization Serialization { get; }

        protected virtual ISendEndpointProvider CreateSendEndpointProvider()
        {
            return new SendEndpointProvider(_sendTransportProvider.Value, _sendObservers, this, SendPipe);
        }

        protected virtual IPublishEndpointProvider CreatePublishEndpointProvider()
        {
            return new PublishEndpointProvider(_publishTransportProvider.Value, HostAddress, _publishObservers, this, PublishPipe, _publishTopology);
        }

        protected abstract ISendTransportProvider CreateSendTransportProvider();

        protected abstract IPublishTransportProvider CreatePublishTransportProvider();
    }
}
