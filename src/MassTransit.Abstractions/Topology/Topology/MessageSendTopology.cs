namespace MassTransit.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Configuration;


    public class MessageSendTopology<TMessage> :
        IMessageSendTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly List<IMessageSendTopologyConvention<TMessage>> _conventions;
        readonly List<IMessageSendTopology<TMessage>> _delegateTopologies;
        readonly List<IMessageSendTopology<TMessage>> _topologies;

        public MessageSendTopology()
        {
            _conventions = new List<IMessageSendTopologyConvention<TMessage>>(8);
            _topologies = new List<IMessageSendTopology<TMessage>>(8);
            _delegateTopologies = new List<IMessageSendTopology<TMessage>>(8);
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

            for (var i = 0; i < _delegateTopologies.Count; i++)
                _delegateTopologies[i].Apply(delegatedBuilder);

            for (var i = 0; i < _conventions.Count; i++)
            {
                if (_conventions[i].TryGetMessageSendTopology(out IMessageSendTopology<TMessage> topology))
                    topology.Apply(builder);
            }

            for (var i = 0; i < _topologies.Count; i++)
                _topologies[i].Apply(builder);
        }

        public bool TryGetConvention<TConvention>([NotNullWhen(true)] out TConvention? convention)
            where TConvention : class, IMessageSendTopologyConvention<TMessage>
        {
            for (var i = 0; i < _conventions.Count; i++)
            {
                convention = _conventions[i] as TConvention;
                if (convention != null)
                    return true;
            }

            convention = default;
            return false;
        }

        public bool TryAddConvention(IMessageSendTopologyConvention<TMessage> convention)
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

        public bool TryAddConvention(ISendTopologyConvention convention)
        {
            return convention.TryGetMessageSendTopologyConvention(out IMessageSendTopologyConvention<TMessage> messageSendTopologyConvention)
                && TryAddConvention(messageSendTopologyConvention);
        }

        public void UpdateConvention<TConvention>(Func<TConvention, TConvention> update)
            where TConvention : class, IMessageSendTopologyConvention<TMessage>
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
            where TConvention : class, IMessageSendTopologyConvention<TMessage>
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

        public virtual IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}
