namespace MassTransit.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;


    public class MessageSendTopology<TMessage> :
        IMessageSendTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly IList<IMessageSendTopologyConvention<TMessage>> _conventions;
        readonly IList<IMessageSendTopology<TMessage>> _delegateTopologies;
        readonly IList<IMessageSendTopology<TMessage>> _topologies;

        public MessageSendTopology()
        {
            _conventions = new List<IMessageSendTopologyConvention<TMessage>>();
            _topologies = new List<IMessageSendTopology<TMessage>>();
            _delegateTopologies = new List<IMessageSendTopology<TMessage>>();
        }

        public void Add(IMessageSendTopology<TMessage> sendTopology)
        {
            _topologies.Add(sendTopology);
        }

        public void AddDelegate(IMessageSendTopology<TMessage> configuration)
        {
            _delegateTopologies.Add(configuration);
        }

        public void Apply(ITopologyPipeBuilder<SendContext<TMessage>> builder)
        {
            ITopologyPipeBuilder<SendContext<TMessage>> delegatedBuilder = builder.CreateDelegatedBuilder();

            foreach (IMessageSendTopology<TMessage> topology in _delegateTopologies)
                topology.Apply(delegatedBuilder);

            foreach (IMessageSendTopologyConvention<TMessage> convention in _conventions)
            {
                if (convention.TryGetMessageSendTopology(out IMessageSendTopology<TMessage> topology))
                    topology.Apply(builder);
            }

            foreach (IMessageSendTopology<TMessage> topology in _topologies)
                topology.Apply(builder);
        }

        public bool TryGetConvention<TConvention>(out TConvention? convention)
            where TConvention : class, IMessageSendTopologyConvention<TMessage>
        {
            for (var i = 0; i < _conventions.Count; i++)
            {
                convention = _conventions[i] as TConvention;
                if (convention != null)
                {
                    return true;
                }
            }

            convention = default;
            return false;
        }

        public bool TryAddConvention(IMessageSendTopologyConvention<TMessage> convention)
        {
            if (_conventions.Any(x => x.GetType() == convention.GetType()))
                return false;

            _conventions.Add(convention);
            return true;
        }

        public void UpdateConvention<TConvention>(Func<TConvention, TConvention> update)
            where TConvention : class, IMessageSendTopologyConvention<TMessage>
        {
            for (var i = 0; i < _conventions.Count; i++)
            {
                var convention = _conventions[i] as TConvention;
                if (convention != null)
                {
                    var updatedConvention = update(convention);
                    _conventions[i] = updatedConvention;
                    return;
                }
            }
        }

        public void AddOrUpdateConvention<TConvention>(Func<TConvention> add, Func<TConvention, TConvention> update)
            where TConvention : class, IMessageSendTopologyConvention<TMessage>
        {
            for (var i = 0; i < _conventions.Count; i++)
            {
                var convention = _conventions[i] as TConvention;
                if (convention != null)
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

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}
