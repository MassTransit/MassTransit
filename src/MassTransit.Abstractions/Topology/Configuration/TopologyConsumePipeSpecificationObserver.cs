namespace MassTransit.Configuration
{
    public class TopologyConsumePipeSpecificationObserver :
        IConsumePipeSpecificationObserver
    {
        readonly IConsumeTopology _topology;

        public TopologyConsumePipeSpecificationObserver(IConsumeTopology topology)
        {
            _topology = topology;
        }

        void IConsumePipeSpecificationObserver.MessageSpecificationCreated<T>(IMessageConsumePipeSpecification<T> specification)
        {
            IMessageConsumeTopology<T> messagePublishTopology = _topology.GetMessageTopology<T>();

            var topologySpecification = new MessageConsumeTopologyPipeSpecification<T>(messagePublishTopology);

            specification.AddParentMessageSpecification(topologySpecification);
        }
    }
}
