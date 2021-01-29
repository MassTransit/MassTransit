namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using Configuration;
    using Context;
    using GreenPipes;
    using GreenPipes.Agents;
    using Integration;
    using Topology;
    using Topology.Builders;
    using Transport;
    using Util;


    public class RabbitMqQueueReceiveEndpointContext :
        BaseReceiveEndpointContext,
        RabbitMqReceiveEndpointContext
    {
        readonly IRabbitMqHostConfiguration _hostConfiguration;
        readonly Recycle<IModelContextSupervisor> _modelContext;
        readonly ReceiveSettings _settings;

        public RabbitMqQueueReceiveEndpointContext(IRabbitMqHostConfiguration hostConfiguration, IRabbitMqReceiveEndpointConfiguration configuration,
            BrokerTopology brokerTopology, ReceiveSettings settings)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;
            _settings = settings;

            ExclusiveConsumer = configuration.Settings.ExclusiveConsumer;
            BrokerTopology = brokerTopology;

            _modelContext = new Recycle<IModelContextSupervisor>(() => new ModelContextSupervisor(hostConfiguration.ConnectionContextSupervisor));
        }

        public BrokerTopology BrokerTopology { get; }

        public bool ExclusiveConsumer { get; }

        public IModelContextSupervisor ModelContextSupervisor => _modelContext.Supervisor;

        public override void AddConsumeAgent(IAgent agent)
        {
            _modelContext.Supervisor.AddConsumeAgent(agent);
        }

        public override Exception ConvertException(Exception exception, string message)
        {
            return new RabbitMqConnectionException(message + _hostConfiguration.Settings.ToDescription(), exception);
        }

        public override void Probe(ProbeContext context)
        {
            context.Add("type", "RabbitMQ");
            context.Set(_settings);

            var topologyScope = context.CreateScope("topology");
            BrokerTopology.Probe(topologyScope);
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new RabbitMqSendTransportProvider(_hostConfiguration.ConnectionContextSupervisor, ModelContextSupervisor);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new RabbitMqPublishTransportProvider(_hostConfiguration.ConnectionContextSupervisor, ModelContextSupervisor);
        }
    }
}
