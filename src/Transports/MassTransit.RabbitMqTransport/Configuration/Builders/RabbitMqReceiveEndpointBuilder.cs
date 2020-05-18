namespace MassTransit.RabbitMqTransport.Builders
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Contexts;
    using GreenPipes;
    using Integration;
    using MassTransit.Builders;
    using Pipeline;
    using Topology;
    using Topology.Builders;
    using Transport;
    using Transports;


    public class RabbitMqReceiveEndpointBuilder :
        ReceiveEndpointBuilder
    {
        readonly IRabbitMqHostControl _host;
        readonly IRabbitMqReceiveEndpointConfiguration _configuration;

        public RabbitMqReceiveEndpointBuilder(IRabbitMqHostControl host, IRabbitMqReceiveEndpointConfiguration configuration)
            : base(configuration)
        {
            _host = host;
            _configuration = configuration;
        }

        public override ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe)
        {
            if (_configuration.ConfigureConsumeTopology)
            {
                _configuration.Topology.Consume
                    .GetMessageTopology<T>()
                    .Bind();
            }

            return base.ConnectConsumePipe(pipe);
        }

        public RabbitMqReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var brokerTopology = BuildTopology(_configuration.Settings);

            IDeadLetterTransport deadLetterTransport = CreateDeadLetterTransport();
            IErrorTransport errorTransport = CreateErrorTransport();

            IModelContextSupervisor supervisor = new ModelContextSupervisor(_host.ConnectionContextSupervisor);

            var receiveEndpointContext = new RabbitMqQueueReceiveEndpointContext(_host, supervisor, _configuration, brokerTopology);

            receiveEndpointContext.GetOrAddPayload(() => deadLetterTransport);
            receiveEndpointContext.GetOrAddPayload(() => errorTransport);

            return receiveEndpointContext;
        }

        IErrorTransport CreateErrorTransport()
        {
            var errorSettings = _configuration.Topology.Send.GetErrorSettings(_configuration.Settings);
            var filter = new ConfigureTopologyFilter<ErrorSettings>(errorSettings, errorSettings.GetBrokerTopology());

            return new RabbitMqErrorTransport(errorSettings.ExchangeName, filter);
        }

        IDeadLetterTransport CreateDeadLetterTransport()
        {
            var deadLetterSettings = _configuration.Topology.Send.GetDeadLetterSettings(_configuration.Settings);
            var filter = new ConfigureTopologyFilter<DeadLetterSettings>(deadLetterSettings, deadLetterSettings.GetBrokerTopology());

            return new RabbitMqDeadLetterTransport(deadLetterSettings.ExchangeName, filter);
        }

        BrokerTopology BuildTopology(ReceiveSettings settings)
        {
            var topologyBuilder = new ReceiveEndpointBrokerTopologyBuilder();

            if (settings.QueueName.Equals(RabbitMqExchangeNames.ReplyTo, StringComparison.OrdinalIgnoreCase))
                return topologyBuilder.BuildBrokerTopology();

            var queueArguments = new Dictionary<string, object>(settings.QueueArguments);

            bool queueAutoDelete = settings.AutoDelete;
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
