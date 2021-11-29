namespace MassTransit.ActiveMqTransport.Topology
{
    using Configuration;


    public class ActiveMqDeadLetterSettings :
        ActiveMqQueueBindingConfigurator,
        DeadLetterSettings
    {
        public ActiveMqDeadLetterSettings(EntitySettings source, string queueName)
            : base(queueName, source.Durable, source.AutoDelete)
        {
        }

        public BrokerTopology GetBrokerTopology()
        {
            var builder = new PublishEndpointBrokerTopologyBuilder();

            builder.CreateQueue(EntityName, AutoDelete);

            return builder.BuildBrokerTopology();
        }
    }
}
