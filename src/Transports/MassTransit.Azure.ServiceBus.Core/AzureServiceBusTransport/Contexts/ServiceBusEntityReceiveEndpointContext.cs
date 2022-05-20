namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using Configuration;
    using Topology;
    using Transports;
    using Util;


    public sealed class ServiceBusEntityReceiveEndpointContext :
        BaseReceiveEndpointContext,
        ServiceBusReceiveEndpointContext
    {
        readonly Recycle<IClientContextSupervisor> _clientContext;
        readonly IServiceBusHostConfiguration _hostConfiguration;

        public ServiceBusEntityReceiveEndpointContext(IServiceBusHostConfiguration hostConfiguration, IServiceBusEntityEndpointConfiguration configuration,
            BrokerTopology brokerTopology, Func<IClientContextSupervisor> supervisorFactory)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;

            BrokerTopology = brokerTopology;

            GetOrAddPayload(() => _hostConfiguration.Topology);

            _clientContext = new Recycle<IClientContextSupervisor>(supervisorFactory);
        }

        public BrokerTopology BrokerTopology { get; }

        public IClientContextSupervisor ClientContextSupervisor => _clientContext.Supervisor;

        public override void Probe(ProbeContext context)
        {
            context.Set(new
            {
                Type = "Azure Service Bus",
                PrefetchCount,
                ConcurrentMessageLimit
            });

            BrokerTopology.Probe(context);
        }

        public override void AddSendAgent(IAgent agent)
        {
            _clientContext.Supervisor.AddSendAgent(agent);
        }

        public override void AddConsumeAgent(IAgent agent)
        {
            _clientContext.Supervisor.AddConsumeAgent(agent);
        }

        public override Exception ConvertException(Exception exception, string message)
        {
            return new ServiceBusConnectionException(message + InputAddress, exception);
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new ServiceBusSendTransportProvider(_hostConfiguration.ConnectionContextSupervisor, this);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new ServiceBusPublishTransportProvider(_hostConfiguration.ConnectionContextSupervisor, this);
        }
    }
}
