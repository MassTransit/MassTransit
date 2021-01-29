namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System;
    using Configuration;
    using Context;
    using Exceptions;
    using GreenPipes;
    using GreenPipes.Agents;
    using Topology;
    using Topology.Builders;
    using Transport;
    using Util;


    public class SqsQueueReceiveEndpointContext :
        BaseReceiveEndpointContext,
        SqsReceiveEndpointContext
    {
        readonly Recycle<IClientContextSupervisor> _clientContext;
        readonly IAmazonSqsHostConfiguration _hostConfiguration;
        readonly ReceiveSettings _settings;

        public SqsQueueReceiveEndpointContext(IAmazonSqsHostConfiguration hostConfiguration, IAmazonSqsReceiveEndpointConfiguration configuration,
            BrokerTopology brokerTopology, ReceiveSettings settings)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;
            _settings = settings;
            BrokerTopology = brokerTopology;

            _clientContext = new Recycle<IClientContextSupervisor>(() => new ClientContextSupervisor(_hostConfiguration.ConnectionContextSupervisor));
        }

        public BrokerTopology BrokerTopology { get; }

        public IClientContextSupervisor ClientContextSupervisor => _clientContext.Supervisor;

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
                _settings.EntityName,
                _settings.Durable,
                _settings.AutoDelete,
                _settings.PrefetchCount,
                _settings.WaitTimeSeconds,
                _settings.PurgeOnStartup
            });

            var topologyScope = context.CreateScope("topology");
            BrokerTopology.Probe(topologyScope);
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new AmazonSqsSendTransportProvider(_hostConfiguration.ConnectionContextSupervisor, ClientContextSupervisor);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new AmazonSqsPublishTransportProvider(_hostConfiguration.ConnectionContextSupervisor, ClientContextSupervisor);
        }
    }
}
