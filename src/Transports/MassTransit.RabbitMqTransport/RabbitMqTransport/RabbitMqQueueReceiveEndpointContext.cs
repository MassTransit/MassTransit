namespace MassTransit.RabbitMqTransport
{
    using System;
    using Configuration;
    using Topology;
    using Transports;
    using Util;


    public class RabbitMqQueueReceiveEndpointContext :
        BaseReceiveEndpointContext,
        RabbitMqReceiveEndpointContext
    {
        readonly IRabbitMqReceiveEndpointConfiguration _configuration;
        readonly IRabbitMqHostConfiguration _hostConfiguration;
        readonly Recycle<IModelContextSupervisor> _modelContext;

        public RabbitMqQueueReceiveEndpointContext(IRabbitMqHostConfiguration hostConfiguration, IRabbitMqReceiveEndpointConfiguration configuration,
            BrokerTopology brokerTopology)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;
            _configuration = configuration;

            ExclusiveConsumer = configuration.Settings.ExclusiveConsumer;
            BrokerTopology = brokerTopology;

            _modelContext = new Recycle<IModelContextSupervisor>(() => new ModelContextSupervisor(hostConfiguration.ConnectionContextSupervisor));
        }

        public BrokerTopology BrokerTopology { get; }

        public bool ExclusiveConsumer { get; }

        public IModelContextSupervisor ModelContextSupervisor => _modelContext.Supervisor;

        public override void AddSendAgent(IAgent agent)
        {
            _modelContext.Supervisor.AddSendAgent(agent);
        }

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
            context.Add("concurrentMessageLimit", ConcurrentMessageLimit);
            context.Set(_configuration.Settings);

            var topologyScope = context.CreateScope("topology");
            BrokerTopology.Probe(topologyScope);
        }

        protected override ISendTransportProvider CreateSendTransportProvider()
        {
            return new RabbitMqSendTransportProvider(_hostConfiguration.ConnectionContextSupervisor, this);
        }

        protected override IPublishTransportProvider CreatePublishTransportProvider()
        {
            return new RabbitMqPublishTransportProvider(_hostConfiguration.ConnectionContextSupervisor, this);
        }
    }
}
