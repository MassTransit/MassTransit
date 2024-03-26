#nullable enable
namespace MassTransit.MessageData.Conventions
{
    using System.Diagnostics.CodeAnalysis;
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

        bool IMessageConsumeTopologyConvention.TryGetMessageConsumeTopologyConvention<T>(
            [NotNullWhen(true)] out IMessageConsumeTopologyConvention<T>? convention)
        {
            convention = this as IMessageConsumeTopologyConvention<T>;

            return convention != null;
        }

        public bool TryGetMessageConsumeTopology([NotNullWhen(true)] out IMessageConsumeTopology<TMessage>? messageConsumeTopology)
        {
            var specification = new GetMessageDataTransformSpecification<TMessage>(_repository);
            if (specification.TryGetConsumeTopology(out messageConsumeTopology))
                return true;

            messageConsumeTopology = null;
            return false;
        }
    }
}
