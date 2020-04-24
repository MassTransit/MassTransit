namespace MassTransit.ActiveMqTransport.Topology.Settings
{
    using Builders;
    using Configurators;


    public class ActiveMqDeadLetterSettings :
        QueueBindingConfigurator,
        DeadLetterSettings
    {
        public ActiveMqDeadLetterSettings(EntitySettings source, string queueName)
            : base(queueName, source.Durable, source.AutoDelete)
        {
        }

        public BrokerTopology GetBrokerTopology()
        {
            var builder = new PublishEndpointBrokerTopologyBuilder();

            builder.CreateQueue(EntityName, Durable, AutoDelete);

            return builder.BuildBrokerTopology();
        }
    }
}
