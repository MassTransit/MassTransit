namespace MassTransit.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Configuration;
    using Internals;


    public class MessagePublishTopology<TMessage> :
        IMessagePublishTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly List<IMessagePublishTopologyConvention<TMessage>> _conventions;
        readonly List<IMessagePublishTopology<TMessage>> _delegateTopologies;
        readonly IPublishTopology _publishTopology;
        readonly List<IMessagePublishTopology<TMessage>> _topologies;
        bool? _exclude;

        public MessagePublishTopology(IPublishTopology publishTopology)
        {
            _publishTopology = publishTopology;
            _conventions = new List<IMessagePublishTopologyConvention<TMessage>>(8);
            _topologies = new List<IMessagePublishTopology<TMessage>>(8);
            _delegateTopologies = new List<IMessagePublishTopology<TMessage>>(8);
        }

        public bool Exclude
        {
            get => _exclude ??= IsMessageTypeExcluded();
            set => _exclude = value;
        }

        public void Add(IMessagePublishTopology<TMessage> publishTopology)
        {
            _topologies.Add(publishTopology);
        }

        public void AddDelegate(IMessagePublishTopology<TMessage> configuration)
        {
            _delegateTopologies.Add(configuration);
        }

        public void Apply(ITopologyPipeBuilder<PublishContext<TMessage>> builder)
        {
            ITopologyPipeBuilder<PublishContext<TMessage>> delegatedBuilder = builder.CreateDelegatedBuilder();

            for (var i = 0; i < _delegateTopologies.Count; i++)
                _delegateTopologies[i].Apply(delegatedBuilder);

            for (var i = 0; i < _conventions.Count; i++)
            {
                if (_conventions[i].TryGetMessagePublishTopology(out IMessagePublishTopology<TMessage> topology))
                    topology.Apply(builder);
            }

            foreach (IMessagePublishTopology<TMessage> topology in _topologies)
                topology.Apply(builder);
        }

        public virtual bool TryGetPublishAddress(Uri baseAddress, [NotNullWhen(true)] out Uri? publishAddress)
        {
            publishAddress = null;
            return false;
        }

        public bool TryAddConvention(IMessagePublishTopologyConvention<TMessage> convention)
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

        public bool TryAddConvention(IPublishTopologyConvention convention)
        {
            return convention.TryGetMessagePublishTopologyConvention(out IMessagePublishTopologyConvention<TMessage> messagePublishTopologyConvention)
                && TryAddConvention(messagePublishTopologyConvention);
        }

        public void AddOrUpdateConvention<TConvention>(Func<TConvention> add, Func<TConvention, TConvention> update)
            where TConvention : class, IMessagePublishTopologyConvention<TMessage>
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
            yield break;
        }

        bool IsMessageTypeExcluded()
        {
            if (typeof(TMessage).GetCustomAttributes(typeof(ExcludeFromTopologyAttribute), false).Length > 0)
                return true;

            if (typeof(TMessage).ClosesType(typeof(Fault<>), out Type[] types) && _publishTopology.GetMessageTopology(types[0]).Exclude)
                return true;

            return false;
        }
    }
}
