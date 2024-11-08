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
        readonly Recycle<IChannelContextSupervisor> _channelContext;

        public RabbitMqQueueReceiveEndpointContext(IRabbitMqHostConfiguration hostConfiguration, IRabbitMqReceiveEndpointConfiguration configuration,
            BrokerTopology brokerTopology)
            : base(hostConfiguration, configuration)
        {
            _hostConfiguration = hostConfiguration;
            _configuration = configuration;

            ExclusiveConsumer = configuration.Settings.ExclusiveConsumer;
            BrokerTopology = brokerTopology;

            IsNotReplyTo = configuration.Settings.QueueName != RabbitMqExchangeNames.ReplyTo;

            var concurrentMessageLimit = ConcurrentMessageLimit ?? PrefetchCount;
            if (concurrentMessageLimit > ushort.MaxValue)
                concurrentMessageLimit = ushort.MaxValue;

            _channelContext = new Recycle<IChannelContextSupervisor>(() =>
                new ChannelContextSupervisor(hostConfiguration.ConnectionContextSupervisor, (ushort)concurrentMessageLimit));
        }

        public BrokerTopology BrokerTopology { get; }

        public bool ExclusiveConsumer { get; }
        public bool IsNotReplyTo { get; }

        public IChannelContextSupervisor ChannelContextSupervisor => _channelContext.Supervisor;

        public override void AddSendAgent(IAgent agent)
        {
            _channelContext.Supervisor.AddSendAgent(agent);
        }

        public override void AddConsumeAgent(IAgent agent)
        {
            _channelContext.Supervisor.AddConsumeAgent(agent);
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
