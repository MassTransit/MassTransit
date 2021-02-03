namespace MassTransit.ActiveMqTransport.Builders
{
    using Configuration;
    using Contexts;
    using GreenPipes;
    using MassTransit.Builders;
    using MassTransit.Pipeline;
    using Pipeline;
    using Topology;
    using Topology.Builders;
    using Transport;
    using Transports;


    public class ActiveMqReceiveEndpointBuilder :
        ReceiveEndpointBuilder
    {
        readonly IActiveMqReceiveEndpointConfiguration _configuration;
        readonly IActiveMqHostConfiguration _hostConfiguration;

        public ActiveMqReceiveEndpointBuilder(IActiveMqHostConfiguration hostConfiguration, IActiveMqReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _hostConfiguration = hostConfiguration;
            _configuration = configuration;
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
        {
            if (_configuration.ConfigureConsumeTopology && options.HasFlag(ConnectPipeOptions.ConfigureConsumeTopology))
            {
                _configuration.Topology.Consume
                    .GetMessageTopology<T>()
                    .Bind();
            }

            return base.ConnectConsumePipe(pipe, options);
        }

        public ActiveMqReceiveEndpointContext CreateReceiveEndpointContext(ReceiveSettings receiveSettings)
        {
            var brokerTopology = BuildTopology(_configuration.Settings);

            var deadLetterTransport = CreateDeadLetterTransport();
            var errorTransport = CreateErrorTransport();

            var context = new ActiveMqConsumerReceiveEndpointContext(_hostConfiguration, _configuration, brokerTopology, receiveSettings);

            context.GetOrAddPayload(() => deadLetterTransport);
            context.GetOrAddPayload(() => errorTransport);
            context.GetOrAddPayload(() => _hostConfiguration.HostTopology);

            return context;
        }

        IErrorTransport CreateErrorTransport()
        {
            var errorSettings = _configuration.Topology.Send.GetErrorSettings(_configuration.Settings);
            var filter = new ConfigureTopologyFilter<ErrorSettings>(errorSettings, errorSettings.GetBrokerTopology());

            return new ActiveMqErrorTransport(errorSettings.EntityName, filter);
        }

        IDeadLetterTransport CreateDeadLetterTransport()
        {
            var deadLetterSettings = _configuration.Topology.Send.GetDeadLetterSettings(_configuration.Settings);
            var filter = new ConfigureTopologyFilter<DeadLetterSettings>(deadLetterSettings, deadLetterSettings.GetBrokerTopology());

            return new ActiveMqDeadLetterTransport(deadLetterSettings.EntityName, filter);
        }

        BrokerTopology BuildTopology(ReceiveSettings settings)
        {
            var topologyBuilder = new ReceiveEndpointBrokerTopologyBuilder();

            topologyBuilder.Queue = topologyBuilder.CreateQueue(settings.EntityName, settings.Durable, settings.AutoDelete);

            _configuration.Topology.Consume.Apply(topologyBuilder);

            return topologyBuilder.BuildTopologyLayout();
        }
    }
}
