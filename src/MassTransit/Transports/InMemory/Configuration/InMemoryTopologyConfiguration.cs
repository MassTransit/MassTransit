namespace MassTransit.Transports.InMemory
{
    using EndpointSpecifications;
    using GreenPipes;
    using MassTransit.Topology.Configuration;
    using MassTransit.Topology.Observers;
    using MassTransit.Topology.Topologies;
    using Topology.Configurators;
    using Topology.Topologies;


    public class InMemoryTopologyConfiguration :
        IInMemoryTopologyConfiguration
    {
        readonly IMessageTopologyConfigurator _messageTopology;
        readonly IInMemoryPublishTopologyConfigurator _publishTopology;
        readonly ConnectHandle _publishToSendTopologyHandle;
        readonly ISendTopologyConfigurator _sendTopology;
        readonly InMemoryConsumeTopology _consumeTopology;

        public InMemoryTopologyConfiguration(IMessageTopologyConfigurator messageTopology)
        {
            _messageTopology = messageTopology;

            _sendTopology = new SendTopology();
            _sendTopology.Connect(new DelegateSendTopologyConfigurationObserver(GlobalTopology.Send));

            _publishTopology = new InMemoryPublishTopology(messageTopology);
            _publishTopology.Connect(new DelegatePublishTopologyConfigurationObserver(GlobalTopology.Publish));

            var observer = new PublishToSendTopologyConfigurationObserver(_sendTopology);
            _publishToSendTopologyHandle = _publishTopology.Connect(observer);

            _consumeTopology = new InMemoryConsumeTopology(messageTopology);
        }

        public InMemoryTopologyConfiguration(IInMemoryTopologyConfiguration topologyConfiguration)
        {
            _messageTopology = topologyConfiguration.Message;
            _sendTopology = topologyConfiguration.Send;
            _publishTopology = topologyConfiguration.Publish;

            _consumeTopology = new InMemoryConsumeTopology(topologyConfiguration.Message);
        }

        IMessageTopologyConfigurator ITopologyConfiguration.Message => _messageTopology;
        ISendTopologyConfigurator ITopologyConfiguration.Send => _sendTopology;
        IPublishTopologyConfigurator ITopologyConfiguration.Publish => _publishTopology;
        IConsumeTopologyConfigurator ITopologyConfiguration.Consume => _consumeTopology;

        IInMemoryPublishTopologyConfigurator IInMemoryTopologyConfiguration.Publish => _publishTopology;
        IInMemoryConsumeTopologyConfigurator IInMemoryTopologyConfiguration.Consume => _consumeTopology;

        public void SeparatePublishFromSendTopology()
        {
            _publishToSendTopologyHandle?.Disconnect();
        }
    }
}