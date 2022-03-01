namespace MassTransit.AmazonSqsTransport
{
    using System;
    using Configuration;
    using Topology;
    using Transports;
    using Util;


    public class QueueSqsReceiveEndpointContext :
        BaseReceiveEndpointContext,
        SqsReceiveEndpointContext
    {
        readonly Recycle<IClientContextSupervisor> _clientContext;
        readonly IAmazonSqsReceiveEndpointConfiguration _configuration;
        readonly IAmazonSqsHostConfiguration _hostConfiguration;

        public QueueSqsReceiveEndpointContext(IAmazonSqsHostConfiguration hostConfiguration, IAmazonSqsReceiveEndpointConfiguration configuration,
            BrokerTopology brokerTopology)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;
            _configuration = configuration;
            BrokerTopology = brokerTopology;

            _clientContext = new Recycle<IClientContextSupervisor>(() => new ClientContextSupervisor(_hostConfiguration.ConnectionContextSupervisor));
        }

        public BrokerTopology BrokerTopology { get; }

        public IClientContextSupervisor ClientContextSupervisor => _clientContext.Supervisor;

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
            return new AmazonSqsConnectionException(message + _hostConfiguration.Settings, exception);
        }

        public override void Probe(ProbeContext context)
        {
            context.Add("type", "AmazonSQS");
            context.Set(new
            {
                _configuration.Settings.EntityName,
                _configuration.Settings.Durable,
                _configuration.Settings.AutoDelete,
                _configuration.Settings.PrefetchCount,
                ConcurrentMessageLimit,
                _configuration.Settings.WaitTimeSeconds,
                _configuration.Settings.PurgeOnStartup
            });

            var topologyScope = context.CreateScope("topology");
            BrokerTopology.Probe(topologyScope);
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new AmazonSqsSendTransportProvider(_hostConfiguration.ConnectionContextSupervisor, this);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new AmazonSqsPublishTransportProvider(_hostConfiguration.ConnectionContextSupervisor, this);
        }
    }
}
