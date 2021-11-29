namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;


    public abstract class BaseHost :
        IHost
    {
        readonly IHostConfiguration _hostConfiguration;
        HostHandle _handle;

        protected BaseHost(IHostConfiguration hostConfiguration, IBusTopology busTopology)
        {
            _hostConfiguration = hostConfiguration;
            Topology = busTopology;

            ReceiveEndpoints = new ReceiveEndpointCollection();
            Riders = new RiderCollection();
        }

        protected IReceiveEndpointCollection ReceiveEndpoints { get; }
        IRiderCollection Riders { get; }

        public Uri Address => _hostConfiguration.HostAddress;

        public IBusTopology Topology { get; }

        public abstract HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null);

        public abstract HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint = null);

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return ReceiveEndpoints.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _hostConfiguration.ConnectConsumeObserver(observer);
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _hostConfiguration.ConnectReceiveObserver(observer);
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return ReceiveEndpoints.ConnectReceiveEndpointObserver(observer);
        }

        public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return _hostConfiguration.ConnectEndpointConfigurationObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _hostConfiguration.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _hostConfiguration.ConnectSendObserver(observer);
        }

        public HostHandle Start(CancellationToken cancellationToken)
        {
            if (_handle != null)
            {
                LogContext.Warning?.Log("Start called, but the host was already started: {Address} ({Reason})", _hostConfiguration.HostAddress,
                    "Already Started");

                return _handle;
            }

            LogContext.SetCurrentIfNull(_hostConfiguration.LogContext);

            LogContext.Debug?.Log("Starting bus: {HostAddress}", _hostConfiguration.HostAddress);

            HostReceiveEndpointHandle[] handles = ReceiveEndpoints.StartEndpoints(cancellationToken);

            HostRiderHandle[] riders = Riders.StartRiders(cancellationToken);

            _handle = new StartHostHandle(this, handles, riders);

            return _handle;
        }

        public void AddReceiveEndpoint(string endpointName, ReceiveEndpoint receiveEndpoint)
        {
            ReceiveEndpoints.Add(endpointName, receiveEndpoint);
        }

        public IRider GetRider(string name)
        {
            return Riders.Get(name);
        }

        public void AddRider(string name, IRiderControl riderControl)
        {
            Riders.Add(name, riderControl);
        }

        public BusHealthResult CheckHealth(BusState busState, string healthMessage)
        {
            return ReceiveEndpoints.CheckHealth(busState, healthMessage);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("host");

            Probe(scope);

            ReceiveEndpoints.Probe(scope);
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            LogContext.Current = _hostConfiguration.LogContext;

            LogContext.Debug?.Log("Stopping bus: {HostAddress}", Address);

            await Riders.Stop(cancellationToken).ConfigureAwait(false);

            await ReceiveEndpoints.Stop(cancellationToken).ConfigureAwait(false);

            foreach (var agent in GetAgentHandles())
                await agent.Stop("Bus stopped", cancellationToken).ConfigureAwait(false);

            _handle = null;
        }

        protected abstract void Probe(ProbeContext context);

        protected virtual IAgent[] GetAgentHandles()
        {
            return new IAgent[0];
        }
    }
}
