namespace MassTransit.RabbitMqTransport.Topology.Settings
{
    using System.Collections.Generic;
    using Builders;
    using Configurators;


    public class RabbitMqErrorSettings :
        QueueBindingConfigurator,
        ErrorSettings
    {
        public RabbitMqErrorSettings(EntitySettings source, string name)
            : base(name, source.ExchangeType, source.Durable, source.AutoDelete)
        {
            QueueName = name;

            foreach (KeyValuePair<string, object> argument in source.ExchangeArguments)
                SetExchangeArgument(argument.Key, argument.Value);
        }

        public BrokerTopology GetBrokerTopology()
        {
            var builder = new PublishEndpointBrokerTopologyBuilder();

            builder.Exchange = builder.ExchangeDeclare(ExchangeName, ExchangeType, Durable, AutoDelete, ExchangeArguments);

            var queue = builder.QueueDeclare(QueueName, Durable, AutoDelete, false, QueueArguments);

            builder.QueueBind(builder.Exchange, queue, RoutingKey, BindingArguments);

            return builder.BuildBrokerTopology();
        }
    }
}
