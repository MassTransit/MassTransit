namespace MassTransit.MessageData.Conventions
{
    using Configuration;
    using MassTransit.Configuration;


    public class MessageDataMessageConsumeTopologyConvention<TMessage> :
        IMessageDataMessageConsumeTopologyConvention<TMessage>
        where TMessage : class
    {
        readonly IMessageDataRepository _repository;

        public MessageDataMessageConsumeTopologyConvention(IMessageDataRepository repository)
        {
            _repository = repository;
        }

        bool IMessageConsumeTopologyConvention.TryGetMessageConsumeTopologyConvention<T>(out IMessageConsumeTopologyConvention<T> convention)
        {
            convention = this as IMessageConsumeTopologyConvention<T>;

            return convention != null;
        }

        bool IMessageConsumeTopologyConvention<TMessage>.TryGetMessageConsumeTopology(out IMessageConsumeTopology<TMessage> messageConsumeTopology)
        {
            var specification = new GetMessageDataTransformSpecification<TMessage>(_repository);
            if (specification.TryGetConsumeTopology(out messageConsumeTopology))
                return true;

            messageConsumeTopology = null;
            return false;
        }
    }
}
