namespace MassTransit.RabbitMqTransport.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Configuration;
    using Topology;


    public class RabbitMqTopologyConfiguration :
        IRabbitMqTopologyConfiguration
    {
        readonly IRabbitMqConsumeTopologyConfigurator _consumeTopology;
        readonly IMessageTopologyConfigurator _messageTopology;
        readonly IRabbitMqPublishTopologyConfigurator _publishTopology;
        readonly IRabbitMqSendTopologyConfigurator _sendTopology;

        public RabbitMqTopologyConfiguration(IMessageTopologyConfigurator messageTopology)
        {
            _messageTopology = messageTopology;

            _sendTopology = new RabbitMqSendTopology(RabbitMqEntityNameValidator.Validator);
            _sendTopology.ConnectSendTopologyConfigurationObserver(new DelegateSendTopologyConfigurationObserver(GlobalTopology.Send));
            _sendTopology.TryAddConvention(new RoutingKeySendTopologyConvention());

            _publishTopology = new RabbitMqPublishTopology(messageTopology);
            _publishTopology.ConnectPublishTopologyConfigurationObserver(new DelegatePublishTopologyConfigurationObserver(GlobalTopology.Publish));

            var observer = new PublishToSendTopologyConfigurationObserver(_sendTopology);
            _publishTopology.ConnectPublishTopologyConfigurationObserver(observer);

            _consumeTopology = new RabbitMqConsumeTopology(messageTopology, _publishTopology);
        }

        public RabbitMqTopologyConfiguration(IRabbitMqTopologyConfiguration topologyConfiguration)
        {
            _messageTopology = topologyConfiguration.Message;
            _sendTopology = topologyConfiguration.Send;
            _publishTopology = topologyConfiguration.Publish;

            _consumeTopology = new RabbitMqConsumeTopology(topologyConfiguration.Message, topologyConfiguration.Publish);
        }

        IMessageTopologyConfigurator ITopologyConfiguration.Message => _messageTopology;
        ISendTopologyConfigurator ITopologyConfiguration.Send => _sendTopology;
        IPublishTopologyConfigurator ITopologyConfiguration.Publish => _publishTopology;
        IConsumeTopologyConfigurator ITopologyConfiguration.Consume => _consumeTopology;

        IRabbitMqPublishTopologyConfigurator IRabbitMqTopologyConfiguration.Publish => _publishTopology;
        IRabbitMqSendTopologyConfigurator IRabbitMqTopologyConfiguration.Send => _sendTopology;
        IRabbitMqConsumeTopologyConfigurator IRabbitMqTopologyConfiguration.Consume => _consumeTopology;

        public IEnumerable<ValidationResult> Validate()
        {
            return _sendTopology.Validate()
                .Concat(_publishTopology.Validate())
                .Concat(_consumeTopology.Validate());
        }
    }
}
