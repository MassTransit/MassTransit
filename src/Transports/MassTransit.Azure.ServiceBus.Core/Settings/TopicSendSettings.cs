namespace MassTransit.Azure.ServiceBus.Core.Settings
{
    using global::Azure.Messaging.ServiceBus.Administration;
    using Topology;
    using Transport;


    public class TopicSendSettings :
        SendSettings
    {
        readonly BrokerTopology _brokerTopology;
        readonly CreateTopicOptions _description;

        public TopicSendSettings(CreateTopicOptions description, BrokerTopology brokerTopology)
        {
            _description = description;
            _brokerTopology = brokerTopology;
        }

        public string EntityPath => _description.Name;

        public BrokerTopology GetBrokerTopology()
        {
            return _brokerTopology;
        }
    }
}
