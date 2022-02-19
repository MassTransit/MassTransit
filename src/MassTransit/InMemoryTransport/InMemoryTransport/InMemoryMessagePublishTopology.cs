#nullable enable
namespace MassTransit.InMemoryTransport
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Configuration;
    using Topology;
    using Transports.Fabric;


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

        public ExchangeType ExchangeType { get; set; }

        public void Apply(IMessageFabricPublishTopologyBuilder builder)
        {
            var exchangeName = _messageTopology.EntityName;

            builder.ExchangeDeclare(exchangeName, ExchangeType);

            if (builder.ExchangeName != null)
                builder.ExchangeBind(builder.ExchangeName, exchangeName, builder.ExchangeType == ExchangeType.Topic ? "#" : default);
            else
            {
                builder.ExchangeName = exchangeName;
                builder.ExchangeType = ExchangeType;
            }

            foreach (var configurator in _implementedMessageTypes)
                configurator.Apply(builder);
        }

        public override bool TryGetPublishAddress(Uri baseAddress, out Uri? publishAddress)
        {
            var exchangeName = _messageTopology.EntityName;

            publishAddress = new InMemoryEndpointAddress(new InMemoryHostAddress(baseAddress), exchangeName, exchangeType: ExchangeType);
            return true;
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

            public void Apply(IMessageFabricPublishTopologyBuilder builder)
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
