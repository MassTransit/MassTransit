namespace MassTransit.Azure.ServiceBus.Core.Settings
{
    using Microsoft.Azure.ServiceBus.Management;
    using Topology;
    using Transport;


    public class TopicSendSettings :
        SendSettings
    {
        readonly BrokerTopology _brokerTopology;
        readonly TopicDescription _description;

        public TopicSendSettings(TopicDescription description, BrokerTopology brokerTopology)
        {
            _description = description;
            _brokerTopology = brokerTopology;
        }

        public string EntityPath => _description.Path;

        public BrokerTopology GetBrokerTopology()
        {
            return _brokerTopology;
        }
    }
}
