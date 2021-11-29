namespace MassTransit.InMemoryTransport
{
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;


    public class InMemoryMessageConsumeTopology<TMessage> :
        MessageConsumeTopology<TMessage>,
        IInMemoryMessageConsumeTopologyConfigurator<TMessage>,
        IInMemoryMessageConsumeTopologyConfigurator
        where TMessage : class
    {
        readonly IMessageTopology<TMessage> _messageTopology;
        readonly IList<IInMemoryConsumeTopologySpecification> _specifications;

        public InMemoryMessageConsumeTopology(IMessageTopology<TMessage> messageTopology)
        {
            _messageTopology = messageTopology;
            _specifications = new List<IInMemoryConsumeTopologySpecification>();
        }

        public void Apply(IInMemoryConsumeTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);
        }

        public void Bind()
        {

            if (!IsBindableMessageType)
            {
                _specifications.Add(new InvalidInMemoryConsumeTopologySpecification(TypeCache<TMessage>.ShortName, "Is not a bindable message type"));
                return;
            }

            var specification = new ExchangeBindingConsumeTopologySpecification(_messageTopology.EntityName);

            _specifications.Add(specification);
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }
    }
}
