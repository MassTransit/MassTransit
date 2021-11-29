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
        readonly IList<IMessageConsumeTopologyConvention<TMessage>> _conventions;
        readonly IList<IMessageConsumeTopology<TMessage>> _delegateTopologies;
        readonly IList<IMessageConsumeTopology<TMessage>> _topologies;

        public MessageConsumeTopology()
        {
            _conventions = new List<IMessageConsumeTopologyConvention<TMessage>>();
            _topologies = new List<IMessageConsumeTopology<TMessage>>();
            _delegateTopologies = new List<IMessageConsumeTopology<TMessage>>();
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
            if (_conventions.Any(x => x.GetType() == convention.GetType()))
                return false;

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
                    var updatedConvention = update(convention);
                    _conventions[i] = updatedConvention;
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
                    var updatedConvention = update(convention);
                    _conventions[i] = updatedConvention;
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
