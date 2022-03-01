namespace MassTransit.ActiveMqTransport
{
    using System;
    using Configuration;
    using Topology;
    using Transports;
    using Util;


    public class ActiveMqConsumerReceiveEndpointContext :
        BaseReceiveEndpointContext,
        ActiveMqReceiveEndpointContext
    {
        readonly IActiveMqReceiveEndpointConfiguration _configuration;
        readonly IActiveMqHostConfiguration _hostConfiguration;
        readonly Recycle<ISessionContextSupervisor> _sessionContext;

        public ActiveMqConsumerReceiveEndpointContext(IActiveMqHostConfiguration hostConfiguration, IActiveMqReceiveEndpointConfiguration configuration,
            BrokerTopology brokerTopology)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;
            _configuration = configuration;
            BrokerTopology = brokerTopology;

            _sessionContext = new Recycle<ISessionContextSupervisor>(() => new SessionContextSupervisor(hostConfiguration.ConnectionContextSupervisor));
        }

        public BrokerTopology BrokerTopology { get; }

        public ISessionContextSupervisor SessionContextSupervisor => _sessionContext.Supervisor;

        public override void AddSendAgent(IAgent agent)
        {
            _sessionContext.Supervisor.AddSendAgent(agent);
        }

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
                _configuration.Settings.EntityName,
                _configuration.Settings.Durable,
                _configuration.Settings.AutoDelete,
                _configuration.Settings.PrefetchCount,
                _configuration.Settings.ConcurrentMessageLimit
            });

            var topologyScope = context.CreateScope("topology");

            BrokerTopology.Probe(topologyScope);
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new ActiveMqSendTransportProvider(_hostConfiguration.ConnectionContextSupervisor, this);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new ActiveMqPublishTransportProvider(_hostConfiguration.ConnectionContextSupervisor, this);
        }
    }
}
