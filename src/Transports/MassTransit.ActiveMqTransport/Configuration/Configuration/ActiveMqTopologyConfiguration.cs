namespace MassTransit.ActiveMqTransport.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using MassTransit.Configuration;
    using MassTransit.Topology;
    using MassTransit.Topology.Observers;
    using MassTransit.Topology.Topologies;
    using Topology;
    using Topology.Topologies;


    public class ActiveMqTopologyConfiguration :
        IActiveMqTopologyConfiguration
    {
        readonly IActiveMqConsumeTopologyConfigurator _consumeTopology;
        readonly IMessageTopologyConfigurator _messageTopology;
        readonly IActiveMqPublishTopologyConfigurator _publishTopology;
        readonly IActiveMqSendTopologyConfigurator _sendTopology;
        private IActiveMqBusConfiguration _busConfiguration;

        public ActiveMqTopologyConfiguration(IMessageTopologyConfigurator messageTopology, IActiveMqBusConfiguration busConfiguration)
        {
            _messageTopology = messageTopology;

            _sendTopology = new ActiveMqSendTopology();
            _sendTopology.ConnectSendTopologyConfigurationObserver(new DelegateSendTopologyConfigurationObserver(GlobalTopology.Send));

            _publishTopology = new ActiveMqPublishTopology(messageTopology);
            _publishTopology.ConnectPublishTopologyConfigurationObserver(new DelegatePublishTopologyConfigurationObserver(GlobalTopology.Publish));

            var observer = new PublishToSendTopologyConfigurationObserver(_sendTopology);
            _publishTopology.ConnectPublishTopologyConfigurationObserver(observer);

            _consumeTopology = new ActiveMqConsumeTopology(messageTopology, _publishTopology, topology: this);
            _busConfiguration = busConfiguration;
        }

        public ActiveMqTopologyConfiguration(IActiveMqTopologyConfiguration topologyConfiguration)
        {
            _messageTopology = topologyConfiguration.Message;
            _sendTopology = topologyConfiguration.Send;
            _publishTopology = topologyConfiguration.Publish;

            _consumeTopology = new ActiveMqConsumeTopology(topologyConfiguration.Message, topologyConfiguration.Publish, topology: this);

            _busConfiguration = topologyConfiguration.BusConfiguration;
        }

        IMessageTopologyConfigurator ITopologyConfiguration.Message => _messageTopology;
        ISendTopologyConfigurator ITopologyConfiguration.Send => _sendTopology;
        IPublishTopologyConfigurator ITopologyConfiguration.Publish => _publishTopology;
        IConsumeTopologyConfigurator ITopologyConfiguration.Consume => _consumeTopology;
        public IActiveMqBusConfiguration BusConfiguration
        {
            get => _busConfiguration;
            set => _busConfiguration = value;
        }

        IActiveMqPublishTopologyConfigurator IActiveMqTopologyConfiguration.Publish => _publishTopology;
        IActiveMqSendTopologyConfigurator IActiveMqTopologyConfiguration.Send => _sendTopology;
        IActiveMqConsumeTopologyConfigurator IActiveMqTopologyConfiguration.Consume => _consumeTopology;

        public IEnumerable<ValidationResult> Validate()
        {
            return _sendTopology.Validate()
                .Concat(_publishTopology.Validate())
                .Concat(_consumeTopology.Validate());
        }
    }
}
