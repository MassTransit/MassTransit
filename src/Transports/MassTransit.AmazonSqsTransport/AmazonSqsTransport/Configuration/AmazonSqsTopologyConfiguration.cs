namespace MassTransit.AmazonSqsTransport.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Configuration;
    using Topology;


    public class AmazonSqsTopologyConfiguration :
        IAmazonSqsTopologyConfiguration
    {
        readonly IAmazonSqsConsumeTopologyConfigurator _consumeTopology;
        readonly IMessageTopologyConfigurator _messageTopology;
        readonly IAmazonSqsPublishTopologyConfigurator _publishTopology;
        readonly IAmazonSqsSendTopologyConfigurator _sendTopology;

        public AmazonSqsTopologyConfiguration(IMessageTopologyConfigurator messageTopology)
        {
            _messageTopology = messageTopology;

            _sendTopology = new AmazonSqsSendTopology(AmazonSqsEntityNameValidator.Validator);
            _sendTopology.ConnectSendTopologyConfigurationObserver(new DelegateSendTopologyConfigurationObserver(GlobalTopology.Send));

            _publishTopology = new AmazonSqsPublishTopology(messageTopology);
            _publishTopology.ConnectPublishTopologyConfigurationObserver(new DelegatePublishTopologyConfigurationObserver(GlobalTopology.Publish));

            var observer = new PublishToSendTopologyConfigurationObserver(_sendTopology);
            _publishTopology.ConnectPublishTopologyConfigurationObserver(observer);

            _consumeTopology = new AmazonSqsConsumeTopology(messageTopology, _publishTopology);
        }

        public AmazonSqsTopologyConfiguration(IAmazonSqsTopologyConfiguration topologyConfiguration)
        {
            _messageTopology = topologyConfiguration.Message;
            _sendTopology = topologyConfiguration.Send;
            _publishTopology = topologyConfiguration.Publish;

            _consumeTopology = new AmazonSqsConsumeTopology(topologyConfiguration.Message, topologyConfiguration.Publish);
        }

        IMessageTopologyConfigurator ITopologyConfiguration.Message => _messageTopology;
        ISendTopologyConfigurator ITopologyConfiguration.Send => _sendTopology;
        IPublishTopologyConfigurator ITopologyConfiguration.Publish => _publishTopology;
        IConsumeTopologyConfigurator ITopologyConfiguration.Consume => _consumeTopology;

        IAmazonSqsPublishTopologyConfigurator IAmazonSqsTopologyConfiguration.Publish => _publishTopology;
        IAmazonSqsSendTopologyConfigurator IAmazonSqsTopologyConfiguration.Send => _sendTopology;
        IAmazonSqsConsumeTopologyConfigurator IAmazonSqsTopologyConfiguration.Consume => _consumeTopology;

        public IEnumerable<ValidationResult> Validate()
        {
            return _sendTopology.Validate()
                .Concat(_publishTopology.Validate())
                .Concat(_consumeTopology.Validate());
        }
    }
}
