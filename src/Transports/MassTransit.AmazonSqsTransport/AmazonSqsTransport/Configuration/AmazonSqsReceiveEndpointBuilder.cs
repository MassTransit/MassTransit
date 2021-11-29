namespace MassTransit.AmazonSqsTransport.Configuration
{
    using Amazon.SQS.Model;
    using MassTransit.Configuration;
    using Middleware;
    using Topology;
    using Transports;


    public class AmazonSqsReceiveEndpointBuilder :
        ReceiveEndpointBuilder
    {
        readonly IAmazonSqsReceiveEndpointConfiguration _configuration;
        readonly IAmazonSqsHostConfiguration _hostConfiguration;

        public AmazonSqsReceiveEndpointBuilder(IAmazonSqsHostConfiguration hostConfiguration, IAmazonSqsReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _hostConfiguration = hostConfiguration;
            _configuration = configuration;
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
        {
            if (_configuration.ConfigureConsumeTopology && options.HasFlag(ConnectPipeOptions.ConfigureConsumeTopology))
            {
                IAmazonSqsMessageConsumeTopologyConfigurator<T> topology = _configuration.Topology.Consume.GetMessageTopology<T>();
                if (topology.ConfigureConsumeTopology)
                    topology.Subscribe();
            }

            return base.ConnectConsumePipe(pipe, options);
        }

        public SqsReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var brokerTopology = BuildTopology(_configuration.Settings);

            var headerAdapter = new TransportSetHeaderAdapter<MessageAttributeValue>(
                new SqsHeaderValueConverter(_hostConfiguration.Settings.AllowTransportHeader), TransportHeaderOptions.IncludeFaultMessage);

            var deadLetterTransport = CreateDeadLetterTransport(headerAdapter);

            var errorTransport = CreateErrorTransport(headerAdapter);

            var context = new QueueSqsReceiveEndpointContext(_hostConfiguration, _configuration, brokerTopology);

            context.GetOrAddPayload(() => deadLetterTransport);
            context.GetOrAddPayload(() => errorTransport);
            context.GetOrAddPayload(() => _hostConfiguration.Topology);

            return context;
        }

        BrokerTopology BuildTopology(ReceiveSettings settings)
        {
            var builder = new ReceiveEndpointBrokerTopologyBuilder();

            builder.Queue = builder.CreateQueue(settings.EntityName, settings.Durable, settings.AutoDelete, settings.QueueAttributes,
                settings.QueueSubscriptionAttributes, settings.Tags);

            _configuration.Topology.Consume.Apply(builder);

            return builder.BuildTopologyLayout();
        }

        IErrorTransport CreateErrorTransport(TransportSetHeaderAdapter<MessageAttributeValue> headerAdapter)
        {
            var errorSettings = _configuration.Topology.Send.GetErrorSettings(_configuration.Settings);
            var filter = new ConfigureAmazonSqsTopologyFilter<ErrorSettings>(errorSettings, errorSettings.GetBrokerTopology());

            return new SqsErrorTransport(errorSettings.EntityName, headerAdapter, filter);
        }

        IDeadLetterTransport CreateDeadLetterTransport(TransportSetHeaderAdapter<MessageAttributeValue> headerAdapter)
        {
            var deadLetterSettings = _configuration.Topology.Send.GetDeadLetterSettings(_configuration.Settings);
            var filter = new ConfigureAmazonSqsTopologyFilter<DeadLetterSettings>(deadLetterSettings, deadLetterSettings.GetBrokerTopology());

            return new SqsDeadLetterTransport(deadLetterSettings.EntityName, headerAdapter, filter);
        }
    }
}
