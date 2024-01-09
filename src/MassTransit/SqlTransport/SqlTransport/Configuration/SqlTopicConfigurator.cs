namespace MassTransit.SqlTransport.Configuration
{
    using System;
    using Topology;


    public class SqlTopicConfigurator :
        ISqlTopicConfigurator,
        Topic
    {
        public SqlTopicConfigurator(string topicName)
        {
            TopicName = topicName;
        }

        public string TopicName { get; }

        public SqlEndpointAddress GetEndpointAddress(Uri hostAddress)
        {
            return new SqlEndpointAddress(hostAddress, TopicName, type: SqlEndpointAddress.AddressType.Topic);
        }
    }
}
