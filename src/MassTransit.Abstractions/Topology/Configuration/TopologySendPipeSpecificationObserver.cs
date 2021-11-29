namespace MassTransit.Configuration
{
    public class TopologySendPipeSpecificationObserver :
        ISendPipeSpecificationObserver
    {
        readonly ISendTopology _topology;

        public TopologySendPipeSpecificationObserver(ISendTopology topology)
        {
            _topology = topology;
        }

        void ISendPipeSpecificationObserver.MessageSpecificationCreated<T>(IMessageSendPipeSpecification<T> specification)
        {
            IMessageSendTopology<T> messageSendTopology = _topology.GetMessageTopology<T>();

            var topologySpecification = new MessageSendTopologyPipeSpecification<T>(messageSendTopology);

            specification.AddParentMessageSpecification(topologySpecification);
        }
    }
}
