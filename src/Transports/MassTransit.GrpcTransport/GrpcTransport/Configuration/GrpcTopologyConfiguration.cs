namespace MassTransit.GrpcTransport.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit.Configuration;
    using MassTransit.Topology;
    using Topology;


    public class GrpcTopologyConfiguration :
        IGrpcTopologyConfiguration
    {
        readonly GrpcConsumeTopology _consumeTopology;
        readonly IMessageTopologyConfigurator _messageTopology;
        readonly IGrpcPublishTopologyConfigurator _publishTopology;
        readonly ISendTopologyConfigurator _sendTopology;

        public GrpcTopologyConfiguration(IMessageTopologyConfigurator messageTopology)
        {
            _messageTopology = messageTopology;

            _sendTopology = new SendTopology();
            _sendTopology.ConnectSendTopologyConfigurationObserver(new DelegateSendTopologyConfigurationObserver(GlobalTopology.Send));
            _sendTopology.TryAddConvention(new RoutingKeySendTopologyConvention());

            _publishTopology = new GrpcPublishTopology(messageTopology);
            _publishTopology.ConnectPublishTopologyConfigurationObserver(new DelegatePublishTopologyConfigurationObserver(GlobalTopology.Publish));

            var observer = new PublishToSendTopologyConfigurationObserver(_sendTopology);
            _publishTopology.ConnectPublishTopologyConfigurationObserver(observer);

            _consumeTopology = new GrpcConsumeTopology(messageTopology, _publishTopology);
        }

        public GrpcTopologyConfiguration(IGrpcTopologyConfiguration topologyConfiguration)
        {
            _messageTopology = topologyConfiguration.Message;
            _sendTopology = topologyConfiguration.Send;
            _publishTopology = topologyConfiguration.Publish;

            _consumeTopology = new GrpcConsumeTopology(topologyConfiguration.Message, _publishTopology);
        }

        IMessageTopologyConfigurator ITopologyConfiguration.Message => _messageTopology;
        ISendTopologyConfigurator ITopologyConfiguration.Send => _sendTopology;
        IPublishTopologyConfigurator ITopologyConfiguration.Publish => _publishTopology;
        IConsumeTopologyConfigurator ITopologyConfiguration.Consume => _consumeTopology;

        IGrpcPublishTopologyConfigurator IGrpcTopologyConfiguration.Publish => _publishTopology;
        IGrpcConsumeTopologyConfigurator IGrpcTopologyConfiguration.Consume => _consumeTopology;

        public IEnumerable<ValidationResult> Validate()
        {
            return _sendTopology.Validate()
                .Concat(_publishTopology.Validate())
                .Concat(_consumeTopology.Validate());
        }
    }
}
