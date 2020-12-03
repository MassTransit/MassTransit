namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using Configuration;
    using Context;
    using GreenPipes;
    using GreenPipes.Agents;
    using Pipeline;
    using Topology;
    using Transport;
    using Util;


    public sealed class ServiceBusEntityReceiveEndpointContext :
        BaseReceiveEndpointContext,
        ServiceBusReceiveEndpointContext
    {
        readonly Recycle<IClientContextSupervisor> _clientContext;
        readonly IServiceBusHostConfiguration _hostConfiguration;
        readonly ClientSettings _settings;

        public ServiceBusEntityReceiveEndpointContext(IServiceBusHostConfiguration hostConfiguration, IServiceBusEntityEndpointConfiguration configuration,
            BrokerTopology brokerTopology, Func<IClientContextSupervisor> supervisorFactory, ClientSettings settings)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;
            _settings = settings;

            BrokerTopology = brokerTopology;

            GetOrAddPayload(() => _hostConfiguration.HostTopology);

            _clientContext = new Recycle<IClientContextSupervisor>(supervisorFactory);
        }

        public BrokerTopology BrokerTopology { get; }

        public IClientContextSupervisor ClientContextSupervisor => _clientContext.Supervisor;

        public override void Probe(ProbeContext context)
        {
            context.Set(new
            {
                Type = "Azure Service Bus",
                _settings.Path,
                _settings.PrefetchCount,
                _settings.MaxConcurrentCalls
            });

            BrokerTopology.Probe(context);
        }

        public override void AddAgent(IAgent agent)
        {
            _clientContext.Supervisor.AddAgent(agent);
        }

        public override Exception ConvertException(Exception exception, string message)
        {
            return new ServiceBusConnectionException(message + InputAddress, exception);
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return _hostConfiguration.ConnectionContextSupervisor;
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return _hostConfiguration.ConnectionContextSupervisor;
        }
    }
}
