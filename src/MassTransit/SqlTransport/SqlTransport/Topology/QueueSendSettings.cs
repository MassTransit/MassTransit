namespace MassTransit.SqlTransport.Topology
{
    using System;
    using Configuration;


    public class QueueSendSettings :
        SqlQueueConfigurator,
        SendSettings
    {
        public QueueSendSettings(SqlEndpointAddress address)
            : base(address.Name, address.AutoDeleteOnIdle)
        {
        }

        public QueueSendSettings(EntitySettings settings, string queueName)
            : base(queueName, settings.AutoDeleteOnIdle)
        {
        }

        public SqlEndpointAddress GetSendAddress(Uri hostAddress)
        {
            return new SqlEndpointAddress(hostAddress, QueueName, AutoDeleteOnIdle);
        }

        public BrokerTopology GetBrokerTopology()
        {
            var builder = new PublishEndpointBrokerTopologyBuilder();

            builder.CreateQueue(QueueName, AutoDeleteOnIdle);

            return builder.BuildBrokerTopology();
        }

        public string EntityName => QueueName;
    }
}
