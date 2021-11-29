namespace MassTransit.RabbitMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Configuration;
    using Middleware;
    using Topology;
    using Transports;


    public class RabbitMqReceiveEndpointBuilder :
        ReceiveEndpointBuilder
    {
        readonly IRabbitMqReceiveEndpointConfiguration _configuration;
        readonly IRabbitMqHostConfiguration _hostConfiguration;

        public RabbitMqReceiveEndpointBuilder(IRabbitMqHostConfiguration hostConfiguration, IRabbitMqReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _hostConfiguration = hostConfiguration;
            _configuration = configuration;
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options)
        {
            if (_configuration.ConfigureConsumeTopology && options.HasFlag(ConnectPipeOptions.ConfigureConsumeTopology))
            {
                IRabbitMqMessageConsumeTopologyConfigurator<T> topology = _configuration.Topology.Consume.GetMessageTopology<T>();
                if (topology.ConfigureConsumeTopology)
                    topology.Bind();
            }

            return base.ConnectConsumePipe(pipe, options);
        }

        public RabbitMqReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var brokerTopology = BuildTopology(_configuration.Settings);

            var deadLetterTransport = CreateDeadLetterTransport();
            var errorTransport = CreateErrorTransport();

            var context = new RabbitMqQueueReceiveEndpointContext(_hostConfiguration, _configuration, brokerTopology);

            context.GetOrAddPayload(() => deadLetterTransport);
            context.GetOrAddPayload(() => errorTransport);
            context.GetOrAddPayload(() => _hostConfiguration.Topology);

            return context;
        }

        IErrorTransport CreateErrorTransport()
        {
            var errorSettings = _configuration.Topology.Send.GetErrorSettings(_configuration.Settings);
            var filter = new ConfigureRabbitMqTopologyFilter<ErrorSettings>(errorSettings, errorSettings.GetBrokerTopology());

            return new RabbitMqErrorTransport(errorSettings.ExchangeName, filter);
        }

        IDeadLetterTransport CreateDeadLetterTransport()
        {
            var deadLetterSettings = _configuration.Topology.Send.GetDeadLetterSettings(_configuration.Settings);
            var filter = new ConfigureRabbitMqTopologyFilter<DeadLetterSettings>(deadLetterSettings, deadLetterSettings.GetBrokerTopology());

            return new RabbitMqDeadLetterTransport(deadLetterSettings.ExchangeName, filter);
        }

        BrokerTopology BuildTopology(ReceiveSettings settings)
        {
            var topologyBuilder = new ReceiveEndpointBrokerTopologyBuilder();

            if (settings.QueueName.Equals(RabbitMqExchangeNames.ReplyTo, StringComparison.OrdinalIgnoreCase))
                return topologyBuilder.BuildBrokerTopology();

            var queueArguments = new Dictionary<string, object>(settings.QueueArguments);

            var queueAutoDelete = settings.AutoDelete;
            if (settings.QueueExpiration.HasValue)
            {
                queueArguments["x-expires"] = (long)settings.QueueExpiration.Value.TotalMilliseconds;
                queueAutoDelete = false;
            }

            topologyBuilder.Exchange = topologyBuilder.ExchangeDeclare(settings.ExchangeName ?? settings.QueueName, settings.ExchangeType, settings.Durable,
                settings.AutoDelete, settings.ExchangeArguments);

            if (settings.BindQueue)
            {
                topologyBuilder.Queue = topologyBuilder.QueueDeclare(settings.QueueName, settings.Durable, queueAutoDelete, settings.Exclusive, queueArguments);

                topologyBuilder.QueueBind(topologyBuilder.Exchange, topologyBuilder.Queue, settings.RoutingKey, settings.BindingArguments);
            }

            _configuration.Topology.Consume.Apply(topologyBuilder);

            return topologyBuilder.BuildBrokerTopology();
        }
    }
}
