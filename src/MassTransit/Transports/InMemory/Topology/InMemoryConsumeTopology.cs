namespace MassTransit.Transports.InMemory.Topology
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using MassTransit.Topology;
    using MassTransit.Topology.Configuration;


    public class InMemoryConsumeTopology :
        ConsumeTopology,
        IInMemoryConsumeTopologyConfigurator
    {
        readonly IList<IInMemoryConsumeTopologySpecification> _specifications;

        public InMemoryConsumeTopology(IEntityNameFormatter entityNameFormatter)
            : base(entityNameFormatter)
        {
            _specifications = new List<IInMemoryConsumeTopologySpecification>();
        }

        IInMemoryMessageConsumeTopologyConfigurator<T> IInMemoryConsumeTopology.GetMessageTopology<T>()
        {
            IMessageConsumeTopologyConfigurator<T> configurator = base.GetMessageTopology<T>();

            return configurator as IInMemoryMessageConsumeTopologyConfigurator<T>;
        }

        public void Apply(IInMemoryConsumeTopologyBuilder builder)
        {
            foreach (var specification in _specifications)
            {
                specification.Apply(builder);
            }

            ForEach<IInMemoryMessageConsumeTopologyConfigurator>(x => x.Apply(builder));
        }
        protected override IMessageConsumeTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var entityNameFormatter = new MessageEntityNameFormatter<T>(EntityNameFormatter);

            var messageTopology = new InMemoryMessageConsumeTopology<T>(entityNameFormatter);

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}