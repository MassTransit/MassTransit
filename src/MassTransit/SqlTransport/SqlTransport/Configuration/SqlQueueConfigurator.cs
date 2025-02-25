namespace MassTransit.SqlTransport.Configuration
{
    using System;
    using Topology;


    public class SqlQueueConfigurator :
        ISqlQueueConfigurator,
        Queue
    {
        protected SqlQueueConfigurator(string queueName, TimeSpan? autoDeleteOnIdle = null)
        {
            QueueName = queueName;
            AutoDeleteOnIdle = autoDeleteOnIdle;
        }

        public TimeSpan? AutoDeleteOnIdle { get; set; }

        public int? MaxDeliveryCount { get; set; }

        public string QueueName { get; set; }

        protected SqlEndpointAddress GetEndpointAddress(Uri hostAddress)
        {
            return new SqlEndpointAddress(hostAddress, QueueName, AutoDeleteOnIdle);
        }
    }
}
