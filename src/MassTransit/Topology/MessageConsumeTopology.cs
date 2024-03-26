namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;


    public class MessageConsumeTopology<TMessage> :
        IMessageConsumeTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly List<IMessageConsumeTopologyConvention<TMessage>> _conventions;
        readonly List<IMessageConsumeTopology<TMessage>> _delegateTopologies;
        readonly List<IMessageConsumeTopology<TMessage>> _topologies;

        public MessageConsumeTopology()
        {
            _conventions = new List<IMessageConsumeTopologyConvention<TMessage>>(8);
            _topologies = new List<IMessageConsumeTopology<TMessage>>(8);
            _delegateTopologies = new List<IMessageConsumeTopology<TMessage>>(8);
        }

        protected bool IsBindableMessageType => GlobalTopology.IsConsumableMessageType(typeof(TMessage));

        public bool ConfigureConsumeTopology { get; set; } = true;

        public void Add(IMessageConsumeTopology<TMessage> consumeTopology)
        {
            _topologies.Add(consumeTopology);
        }

        public void AddDelegate(IMessageConsumeTopology<TMessage> configuration)
        {
            _delegateTopologies.Add(configuration);
        }

        public void Apply(ITopologyPipeBuilder<ConsumeContext<TMessage>> builder)
        {
            if (_delegateTopologies.Count > 0)
            {
                ITopologyPipeBuilder<ConsumeContext<TMessage>> delegatedBuilder = builder.CreateDelegatedBuilder();

                for (var index = 0; index < _delegateTopologies.Count; index++)
                    _delegateTopologies[index].Apply(delegatedBuilder);
            }

            for (var index = 0; index < _conventions.Count; index++)
            {
                if (_conventions[index].TryGetMessageConsumeTopology(out IMessageConsumeTopology<TMessage> topology))
                    topology.Apply(builder);
            }

            for (var index = 0; index < _topologies.Count; index++)
                _topologies[index].Apply(builder);
        }

        public bool TryAddConvention(IMessageConsumeTopologyConvention<TMessage> convention)
        {
            var conventionType = convention.GetType();

            for (var i = 0; i < _conventions.Count; i++)
            {
                if (_conventions[i].GetType() == conventionType)
                    return false;
            }

            _conventions.Add(convention);
            return true;
        }

        public void UpdateConvention<TConvention>(Func<TConvention, TConvention> update)
            where TConvention : class, IMessageConsumeTopologyConvention<TMessage>
        {
            for (var i = 0; i < _conventions.Count; i++)
            {
                if (_conventions[i] is TConvention convention)
                {
                    _conventions[i] = update(convention);
                    return;
                }
            }
        }

        public void AddOrUpdateConvention<TConvention>(Func<TConvention> add, Func<TConvention, TConvention> update)
            where TConvention : class, IMessageConsumeTopologyConvention<TMessage>
        {
            for (var i = 0; i < _conventions.Count; i++)
            {
                if (_conventions[i] is TConvention convention)
                {
                    _conventions[i] = update(convention);
                    return;
                }
            }

            var addedConvention = add();
            if (addedConvention != null)
                _conventions.Add(addedConvention);
        }

        public bool TryAddConvention(IConsumeTopologyConvention convention)
        {
            return convention.TryGetMessageConsumeTopologyConvention(out IMessageConsumeTopologyConvention<TMessage> messageConsumeTopologyConvention)
                && TryAddConvention(messageConsumeTopologyConvention);
        }

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}
