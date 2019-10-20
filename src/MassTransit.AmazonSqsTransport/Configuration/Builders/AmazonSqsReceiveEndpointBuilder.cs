namespace MassTransit.AmazonSqsTransport.Configuration.Builders
{
    using Configuration;
    using Contexts;
    using GreenPipes;
    using MassTransit.Builders;
    using Pipeline;
    using Topology;
    using Topology.Builders;
    using Transport;
    using Transports;


    public class AmazonSqsReceiveEndpointBuilder :
        ReceiveEndpointBuilder,
        IReceiveEndpointBuilder
    {
        readonly IAmazonSqsHostControl _host;
        readonly IAmazonSqsReceiveEndpointConfiguration _configuration;

        public AmazonSqsReceiveEndpointBuilder(IAmazonSqsHostControl host, IAmazonSqsReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _host = host;
            _configuration = configuration;
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            if (_configuration.SubscribeMessageTopics)
            {
                _configuration.Topology.Consume
                    .GetMessageTopology<T>()
                    .Subscribe();
            }

            return base.ConnectConsumePipe(pipe);
        }

        public SqsReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var brokerTopology = BuildTopology(_configuration.Settings);

            IDeadLetterTransport deadLetterTransport = CreateDeadLetterTransport();
            IErrorTransport errorTransport = CreateErrorTransport();

            var receiveEndpointContext = new SqsQueueReceiveEndpointContext(_host, _configuration, brokerTopology);

            receiveEndpointContext.GetOrAddPayload(() => deadLetterTransport);
            receiveEndpointContext.GetOrAddPayload(() => errorTransport);

            return receiveEndpointContext;
        }

        BrokerTopology BuildTopology(ReceiveSettings settings)
        {
            var builder = new ReceiveEndpointBrokerTopologyBuilder();

            builder.Queue = builder.CreateQueue(settings.EntityName, settings.Durable, settings.AutoDelete, settings.QueueAttributes, settings.QueueSubscriptionAttributes, settings.Tags);

            _configuration.Topology.Consume.Apply(builder);

            return builder.BuildTopologyLayout();
        }

        IErrorTransport CreateErrorTransport()
        {
            var errorSettings = _configuration.Topology.Send.GetErrorSettings(_configuration.Settings);
            var filter = new ConfigureTopologyFilter<ErrorSettings>(errorSettings, errorSettings.GetBrokerTopology());

            return new SqsErrorTransport(errorSettings.EntityName, filter);
        }

        IDeadLetterTransport CreateDeadLetterTransport()
        {
            var deadLetterSettings = _configuration.Topology.Send.GetDeadLetterSettings(_configuration.Settings);
            var filter = new ConfigureTopologyFilter<DeadLetterSettings>(deadLetterSettings, deadLetterSettings.GetBrokerTopology());

            return new SqsDeadLetterTransport(deadLetterSettings.EntityName, filter);
        }
    }
}
