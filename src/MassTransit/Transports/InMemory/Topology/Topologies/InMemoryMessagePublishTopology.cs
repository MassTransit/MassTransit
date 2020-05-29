namespace MassTransit.Transports.InMemory.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;


    public class InMemoryMessagePublishTopology<TMessage> :
        MessagePublishTopology<TMessage>,
        IInMemoryMessagePublishTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly IList<IInMemoryMessagePublishTopology> _implementedMessageTypes;
        readonly IMessageTopology<TMessage> _messageTopology;

        public InMemoryMessagePublishTopology(IMessageTopology<TMessage> messageTopology)
        {
            _messageTopology = messageTopology;
            _implementedMessageTypes = new List<IInMemoryMessagePublishTopology>();
        }

        public void Apply(IInMemoryPublishTopologyBuilder builder)
        {
            var exchangeHandle = ExchangeDeclare(builder);

            if (builder.ExchangeName != null)
                builder.ExchangeBind(builder.ExchangeName, exchangeHandle);
            else
                builder.ExchangeName = exchangeHandle;

            foreach (var configurator in _implementedMessageTypes)
                configurator.Apply(builder);
        }

        public override bool TryGetPublishAddress(Uri baseAddress, out Uri publishAddress)
        {
            var exchangeName = _messageTopology.EntityName;

            publishAddress = new Uri($"{baseAddress}{exchangeName}");
            return true;
        }

        string ExchangeDeclare(IInMemoryTopologyBuilder builder)
        {
            var exchangeName = _messageTopology.EntityName;

            builder.ExchangeDeclare(exchangeName);

            return exchangeName;
        }

        public void AddImplementedMessageConfigurator<T>(IInMemoryMessagePublishTopologyConfigurator<T> configurator, bool direct)
            where T : class
        {
            var adapter = new TypeAdapter<T>(configurator, direct);

            _implementedMessageTypes.Add(adapter);
        }


        class TypeAdapter<T> :
            IInMemoryMessagePublishTopology
            where T : class
        {
            readonly IInMemoryMessagePublishTopologyConfigurator<T> _configurator;
            readonly bool _direct;

            public TypeAdapter(IInMemoryMessagePublishTopologyConfigurator<T> configurator, bool direct)
            {
                _configurator = configurator;
                _direct = direct;
            }

            public void Apply(IInMemoryPublishTopologyBuilder builder)
            {
                if (_direct)
                {
                    var implementedBuilder = builder.CreateImplementedBuilder();

                    _configurator.Apply(implementedBuilder);
                }
            }
        }
    }
}
