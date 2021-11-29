namespace MassTransit.ActiveMqTransport.Topology
{
    using Configuration;


    public class ActiveMqErrorSettings :
        ActiveMqQueueBindingConfigurator,
        ErrorSettings
    {
        public ActiveMqErrorSettings(EntitySettings source, string queueName)
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
