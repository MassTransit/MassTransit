namespace MassTransit.SqlTransport.Configuration
{
    using MassTransit.Configuration;
    using Topology;
    using Transports;


    public class SqlReceiveEndpointBuilder :
        ReceiveEndpointBuilder
    {
        readonly ISqlReceiveEndpointConfiguration _configuration;
        readonly ISqlHostConfiguration _hostConfiguration;

        public SqlReceiveEndpointBuilder(ISqlHostConfiguration hostConfiguration, ISqlReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _hostConfiguration = hostConfiguration;
            _configuration = configuration;
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
        {
            if (_configuration.ConfigureConsumeTopology && options.HasFlag(ConnectPipeOptions.ConfigureConsumeTopology))
            {
                ISqlMessageConsumeTopologyConfigurator<T> topology = _configuration.Topology.Consume.GetMessageTopology<T>();
                if (topology.ConfigureConsumeTopology)
                    topology.Subscribe();
            }

            return base.ConnectConsumePipe(pipe, options);
        }

        public SqlReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var brokerTopology = BuildTopology(_configuration.Settings);

            var deadLetterTransport = CreateDeadLetterTransport();
            var errorTransport = CreateErrorTransport();

            var context = new QueueSqlReceiveEndpointContext(_hostConfiguration, _configuration, brokerTopology);

            context.GetOrAddPayload(() => deadLetterTransport);
            context.GetOrAddPayload(() => errorTransport);
            context.GetOrAddPayload(() => _hostConfiguration.Topology);

            return context;
        }

        IErrorTransport CreateErrorTransport()
        {
            return new SqlQueueErrorTransport(_configuration.Settings.QueueName, SqlQueueType.ErrorQueue);
        }

        IDeadLetterTransport CreateDeadLetterTransport()
        {
            return new SqlQueueDeadLetterTransport(_configuration.Settings.QueueName, SqlQueueType.DeadLetterQueue);
        }

        BrokerTopology BuildTopology(ReceiveSettings settings)
        {
            var topologyBuilder = new ReceiveEndpointBrokerTopologyBuilder(settings);

            _configuration.Topology.Consume.Apply(topologyBuilder);

            return topologyBuilder.BuildBrokerTopology();
        }
    }
}
