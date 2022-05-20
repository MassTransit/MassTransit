namespace MassTransit.InMemoryTransport.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Configuration;
    using Topology;


    public class InMemoryTopologyConfiguration :
        IInMemoryTopologyConfiguration
    {
        readonly InMemoryConsumeTopology _consumeTopology;
        readonly IMessageTopologyConfigurator _messageTopology;
        readonly IInMemoryPublishTopologyConfigurator _publishTopology;
        readonly ISendTopologyConfigurator _sendTopology;

        public InMemoryTopologyConfiguration(IMessageTopologyConfigurator messageTopology)
        {
            _messageTopology = messageTopology;

            _sendTopology = new SendTopology();
            _sendTopology.ConnectSendTopologyConfigurationObserver(new DelegateSendTopologyConfigurationObserver(GlobalTopology.Send));

            _publishTopology = new InMemoryPublishTopology(messageTopology);
            _publishTopology.ConnectPublishTopologyConfigurationObserver(new DelegatePublishTopologyConfigurationObserver(GlobalTopology.Publish));

            var observer = new PublishToSendTopologyConfigurationObserver(_sendTopology);
            _publishTopology.ConnectPublishTopologyConfigurationObserver(observer);

            _consumeTopology = new InMemoryConsumeTopology(messageTopology, _publishTopology);
        }

        public InMemoryTopologyConfiguration(IInMemoryTopologyConfiguration topologyConfiguration)
        {
            _messageTopology = topologyConfiguration.Message;
            _sendTopology = topologyConfiguration.Send;
            _publishTopology = topologyConfiguration.Publish;

            _consumeTopology = new InMemoryConsumeTopology(topologyConfiguration.Message, _publishTopology);
        }

        IMessageTopologyConfigurator ITopologyConfiguration.Message => _messageTopology;
        ISendTopologyConfigurator ITopologyConfiguration.Send => _sendTopology;
        IPublishTopologyConfigurator ITopologyConfiguration.Publish => _publishTopology;
        IConsumeTopologyConfigurator ITopologyConfiguration.Consume => _consumeTopology;

        IInMemoryPublishTopologyConfigurator IInMemoryTopologyConfiguration.Publish => _publishTopology;
        IInMemoryConsumeTopologyConfigurator IInMemoryTopologyConfiguration.Consume => _consumeTopology;

        public IEnumerable<ValidationResult> Validate()
        {
            return _sendTopology.Validate()
                .Concat(_publishTopology.Validate())
                .Concat(_consumeTopology.Validate());
        }
    }
}
