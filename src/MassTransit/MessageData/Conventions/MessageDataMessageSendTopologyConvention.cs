namespace MassTransit.MessageData.Conventions
{
    using Configuration;
    using MassTransit.Configuration;
    using Topology;


    public class MessageDataMessageSendTopologyConvention<TMessage> :
        IMessageDataMessageSendTopologyConvention<TMessage>
        where TMessage : class
    {
        readonly IMessageDataRepository _repository;

        public MessageDataMessageSendTopologyConvention(IMessageDataRepository repository)
        {
            _repository = repository;
        }

        bool IMessageSendTopologyConvention.TryGetMessageSendTopologyConvention<T>(out IMessageSendTopologyConvention<T> convention)
        {
            convention = this as IMessageSendTopologyConvention<T>;

            return convention != null;
        }

        bool IMessageSendTopologyConvention<TMessage>.TryGetMessageSendTopology(out IMessageSendTopology<TMessage> messageSendTopology)
        {
            var specification = new PutMessageDataTransformSpecification<TMessage>(_repository);
            if (specification.TryGetSendTopology(out messageSendTopology))
                return true;

            messageSendTopology = null;
            return false;
        }
    }
}
