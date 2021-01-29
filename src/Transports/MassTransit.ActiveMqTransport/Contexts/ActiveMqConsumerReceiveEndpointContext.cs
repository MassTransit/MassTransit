namespace MassTransit.ActiveMqTransport.Contexts
{
    using System;
    using Configuration;
    using Context;
    using GreenPipes;
    using GreenPipes.Agents;
    using Topology;
    using Topology.Builders;
    using Transport;
    using Util;


    public class ActiveMqConsumerReceiveEndpointContext :
        BaseReceiveEndpointContext,
        ActiveMqReceiveEndpointContext
    {
        readonly IActiveMqHostConfiguration _hostConfiguration;
        readonly Recycle<ISessionContextSupervisor> _sessionContext;
        readonly ReceiveSettings _settings;

        public ActiveMqConsumerReceiveEndpointContext(IActiveMqHostConfiguration hostConfiguration, IActiveMqReceiveEndpointConfiguration configuration,
            BrokerTopology brokerTopology, ReceiveSettings settings)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;
            _settings = settings;
            BrokerTopology = brokerTopology;

            _sessionContext = new Recycle<ISessionContextSupervisor>(() => new SessionContextSupervisor(hostConfiguration.ConnectionContextSupervisor));
        }

        public BrokerTopology BrokerTopology { get; }

        public ISessionContextSupervisor SessionContextSupervisor => _sessionContext.Supervisor;

        public override void AddConsumeAgent(IAgent agent)
        {
            _sessionContext.Supervisor.AddConsumeAgent(agent);
        }

        public override Exception ConvertException(Exception exception, string message)
        {
            return new ActiveMqConnectionException(message + _hostConfiguration.Settings.ToDescription(), exception);
        }

        public override void Probe(ProbeContext context)
        {
            context.Add("type", "ActiveMQ");
            context.Set(new
            {
                _settings.EntityName,
                _settings.Durable,
                _settings.AutoDelete,
                _settings.PrefetchCount
            });
            var topologyScope = context.CreateScope("topology");

            BrokerTopology.Probe(topologyScope);
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new ActiveMqSendTransportProvider(_hostConfiguration.ConnectionContextSupervisor, SessionContextSupervisor);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new ActiveMqPublishTransportProvider(_hostConfiguration.ConnectionContextSupervisor, SessionContextSupervisor);
        }
    }
}
