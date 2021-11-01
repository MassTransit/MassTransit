namespace MassTransit.Azure.ServiceBus.Core.Settings
{
    using global::Azure.Messaging.ServiceBus.Administration;
    using Topology;
    using Transport;


    public class TopicSendSettings :
        SendSettings
    {
        readonly BrokerTopology _brokerTopology;
        readonly CreateTopicOptions _createTopicOptions;

        public TopicSendSettings(CreateTopicOptions createTopicOptions, BrokerTopology brokerTopology)
        {
            _createTopicOptions = createTopicOptions;
            _brokerTopology = brokerTopology;
        }

        public string EntityPath => _createTopicOptions.Name;

        public BrokerTopology GetBrokerTopology()
        {
            return _brokerTopology;
        }
    }
}
