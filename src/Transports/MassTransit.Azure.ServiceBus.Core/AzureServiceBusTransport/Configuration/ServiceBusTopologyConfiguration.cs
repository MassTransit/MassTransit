namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Configuration;
    using Topology;


    public class ServiceBusTopologyConfiguration :
        IServiceBusTopologyConfiguration
    {
        readonly ServiceBusConsumeTopology _consumeTopology;
        readonly IMessageTopologyConfigurator _messageTopology;
        readonly IServiceBusPublishTopologyConfigurator _publishTopology;
        readonly IServiceBusSendTopologyConfigurator _sendTopology;

        public ServiceBusTopologyConfiguration(IMessageTopologyConfigurator messageTopology)
        {
            _messageTopology = messageTopology;

            _sendTopology = new ServiceBusSendTopology();
            _sendTopology.ConnectSendTopologyConfigurationObserver(new DelegateSendTopologyConfigurationObserver(GlobalTopology.Send));
            _sendTopology.TryAddConvention(new SessionIdSendTopologyConvention());
            _sendTopology.TryAddConvention(new PartitionKeySendTopologyConvention());

            _publishTopology = new ServiceBusPublishTopology(messageTopology);
            _publishTopology.ConnectPublishTopologyConfigurationObserver(new DelegatePublishTopologyConfigurationObserver(GlobalTopology.Publish));

            var observer = new PublishToSendTopologyConfigurationObserver(_sendTopology);
            _publishTopology.ConnectPublishTopologyConfigurationObserver(observer);

            _consumeTopology = new ServiceBusConsumeTopology(messageTopology, _publishTopology);
        }

        public ServiceBusTopologyConfiguration(IServiceBusTopologyConfiguration topologyConfiguration)
        {
            _messageTopology = topologyConfiguration.Message;
            _sendTopology = topologyConfiguration.Send;
            _publishTopology = topologyConfiguration.Publish;

            _consumeTopology = new ServiceBusConsumeTopology(topologyConfiguration.Message, topologyConfiguration.Publish);
        }

        IMessageTopologyConfigurator ITopologyConfiguration.Message => _messageTopology;
        ISendTopologyConfigurator ITopologyConfiguration.Send => _sendTopology;
        IPublishTopologyConfigurator ITopologyConfiguration.Publish => _publishTopology;
        IConsumeTopologyConfigurator ITopologyConfiguration.Consume => _consumeTopology;

        IServiceBusPublishTopologyConfigurator IServiceBusTopologyConfiguration.Publish => _publishTopology;
        IServiceBusSendTopologyConfigurator IServiceBusTopologyConfiguration.Send => _sendTopology;
        IServiceBusConsumeTopologyConfigurator IServiceBusTopologyConfiguration.Consume => _consumeTopology;

        public IEnumerable<ValidationResult> Validate()
        {
            return _sendTopology.Validate()
                .Concat(_publishTopology.Validate())
                .Concat(_consumeTopology.Validate());
        }
    }
}
