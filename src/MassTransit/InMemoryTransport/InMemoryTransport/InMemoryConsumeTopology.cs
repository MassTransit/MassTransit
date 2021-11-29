namespace MassTransit.InMemoryTransport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;


    public class InMemoryConsumeTopology :
        ConsumeTopology,
        IInMemoryConsumeTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;
        readonly IList<IInMemoryConsumeTopologySpecification> _specifications;

        public InMemoryConsumeTopology(IMessageTopology messageTopology)
        {
            _messageTopology = messageTopology;
            _specifications = new List<IInMemoryConsumeTopologySpecification>();
        }

        IInMemoryMessageConsumeTopology<T> IInMemoryConsumeTopology.GetMessageTopology<T>()
        {
            IMessageConsumeTopologyConfigurator<T> configurator = base.GetMessageTopology<T>();

            return configurator as IInMemoryMessageConsumeTopology<T>;
        }

        public void AddSpecification(IInMemoryConsumeTopologySpecification specification)
        {
            if (specification == null)
                throw new ArgumentNullException(nameof(specification));

            _specifications.Add(specification);
        }

        IInMemoryMessageConsumeTopologyConfigurator<T> IInMemoryConsumeTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IInMemoryMessageConsumeTopologyConfigurator<T>;
        }

        public void Apply(IInMemoryConsumeTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
                specification.Apply(builder);

            ForEach<IInMemoryMessageConsumeTopologyConfigurator>(x => x.Apply(builder));
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            return base.Validate().Concat(_specifications.SelectMany(x => x.Validate()));
        }

        protected override IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var topology = new InMemoryMessageConsumeTopology<T>(_messageTopology.GetMessageTopology<T>());

            OnMessageTopologyCreated(topology);

            return topology;
        }
    }
}
