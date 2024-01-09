namespace MassTransit.SqlTransport
{
    using System;
    using Configuration;
    using Topology;
    using Transports;
    using Util;


    public class QueueSqlReceiveEndpointContext :
        BaseReceiveEndpointContext,
        SqlReceiveEndpointContext
    {
        readonly Recycle<IClientContextSupervisor> _clientContext;
        readonly ISqlReceiveEndpointConfiguration _configuration;
        readonly ISqlHostConfiguration _hostConfiguration;

        public QueueSqlReceiveEndpointContext(ISqlHostConfiguration hostConfiguration, ISqlReceiveEndpointConfiguration configuration,
            BrokerTopology brokerTopology)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;
            _configuration = configuration;

            BrokerTopology = brokerTopology;

            _clientContext = new Recycle<IClientContextSupervisor>(() => new ClientContextSupervisor(_hostConfiguration.ConnectionContextSupervisor));
        }

        public IClientContextSupervisor ClientContextSupervisor => _clientContext.Supervisor;

        public BrokerTopology BrokerTopology { get; }

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
            return new ConnectionException(message + _hostConfiguration.HostAddress, exception);
        }

        public override void Probe(ProbeContext context)
        {
            context.Add("type", "Sql");
            context.Set(new
            {
                _configuration.Settings.QueueName,
                _configuration.Settings.AutoDeleteOnIdle,
                _configuration.Settings.PrefetchCount,
                ConcurrentMessageLimit,
                _configuration.Settings.ConcurrentDeliveryLimit,
                _configuration.Settings.ReceiveMode,
                _configuration.Settings.PollingInterval,
                _configuration.Settings.PurgeOnStartup
            });

            var topologyScope = context.CreateScope("topology");
            BrokerTopology.Probe(topologyScope);
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new SqlSendTransportProvider(_hostConfiguration.ConnectionContextSupervisor, this);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new SqlPublishTransportProvider(_hostConfiguration.ConnectionContextSupervisor, this);
        }
    }
}
