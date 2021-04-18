namespace MassTransit.GrpcTransport.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;


    public class GrpcMessagePublishTopology<TMessage> :
        MessagePublishTopology<TMessage>,
        IGrpcMessagePublishTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly IList<IGrpcMessagePublishTopology> _implementedMessageTypes;
        readonly IMessageTopology<TMessage> _messageTopology;

        public GrpcMessagePublishTopology(IMessageTopology<TMessage> messageTopology)
        {
            _messageTopology = messageTopology;
            _implementedMessageTypes = new List<IGrpcMessagePublishTopology>();
        }

        public void Apply(IGrpcPublishTopologyBuilder builder)
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

        string ExchangeDeclare(IGrpcTopologyBuilder builder)
        {
            var exchangeName = _messageTopology.EntityName;

            builder.ExchangeDeclare(exchangeName);

            return exchangeName;
        }

        public void AddImplementedMessageConfigurator<T>(IGrpcMessagePublishTopologyConfigurator<T> configurator, bool direct)
            where T : class
        {
            var adapter = new TypeAdapter<T>(configurator, direct);

            _implementedMessageTypes.Add(adapter);
        }


        class TypeAdapter<T> :
            IGrpcMessagePublishTopology
            where T : class
        {
            readonly IGrpcMessagePublishTopologyConfigurator<T> _configurator;
            readonly bool _direct;

            public TypeAdapter(IGrpcMessagePublishTopologyConfigurator<T> configurator, bool direct)
            {
                _configurator = configurator;
                _direct = direct;
            }

            public void Apply(IGrpcPublishTopologyBuilder builder)
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
