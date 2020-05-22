namespace MassTransit.Transports
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Context;
    using EndpointConfigurators;
    using GreenPipes;
    using GreenPipes.Agents;
    using Pipeline;
    using Topology;


    public abstract class BaseHost :
        Supervisor,
        IHost
    {
        readonly IHostConfiguration _hostConfiguration;
        readonly IHostTopology _hostTopology;
        readonly IReceiveEndpointCollection _receiveEndpoints;
        HostHandle _handle;

        protected BaseHost(IHostConfiguration hostConfiguration, IHostTopology hostTopology)
        {
            _hostConfiguration = hostConfiguration;
            _hostTopology = hostTopology;

            _receiveEndpoints = new ReceiveEndpointCollection();
        }

        protected IReceiveEndpointCollection ReceiveEndpoints => _receiveEndpoints;

        Uri IHost.Address => _hostConfiguration.HostAddress;
        IHostTopology IHost.Topology => _hostTopology;

        public abstract HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null);

        public abstract HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint = null);

        ConnectHandle IConsumeMessageObserverConnector.ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer)
        {
            return _receiveEndpoints.ConnectConsumeMessageObserver(observer);
        }

        ConnectHandle IConsumeObserverConnector.ConnectConsumeObserver(IConsumeObserver observer)
        {
            return _receiveEndpoints.ConnectConsumeObserver(observer);
        }

        ConnectHandle IReceiveObserverConnector.ConnectReceiveObserver(IReceiveObserver observer)
        {
            return _receiveEndpoints.ConnectReceiveObserver(observer);
        }

        ConnectHandle IReceiveEndpointObserverConnector.ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _receiveEndpoints.ConnectReceiveEndpointObserver(observer);
        }

        public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer)
        {
            return _hostConfiguration.ConnectEndpointConfigurationObserver(observer);
        }

        ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer)
        {
            return _receiveEndpoints.ConnectPublishObserver(observer);
        }

        ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer)
        {
            return _receiveEndpoints.ConnectSendObserver(observer);
        }

        public virtual Task<HostHandle> Start(CancellationToken cancellationToken)
        {
            if (_handle != null)
                throw new MassTransitException($"The host was already started: {_hostConfiguration.HostAddress}");

            if (LogContext.Current == null)
                throw new ConfigurationException("No valid LogContext was configured.");

            _hostConfiguration.LogContext = LogContext.Current;

            LogContext.Debug?.Log("Starting host: {HostAddress}", _hostConfiguration.HostAddress);

            HostReceiveEndpointHandle[] handles = _receiveEndpoints.StartEndpoints(cancellationToken);

            _handle = new StartHostHandle(this, handles, GetAgentHandles());

            return Task.FromResult(_handle);
        }

        public void AddReceiveEndpoint(string endpointName, IReceiveEndpointControl receiveEndpoint)
        {
            _receiveEndpoints.Add(endpointName, receiveEndpoint);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("host");

            Probe(scope);

            _receiveEndpoints.Probe(scope);
        }

        async Task IAgent.Stop(StopContext context)
        {
            LogContext.Current = _hostConfiguration.LogContext;

            await _receiveEndpoints.Stop(context).ConfigureAwait(false);

            await base.Stop(context).ConfigureAwait(false);
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            await base.StopSupervisor(context).ConfigureAwait(false);

            foreach (var agent in GetAgentHandles())
            {
                await agent.Stop(context).ConfigureAwait(false);
            }
        }

        protected abstract void Probe(ProbeContext context);

        protected virtual IAgent[] GetAgentHandles()
        {
            return new IAgent[0];
        }
    }
}
