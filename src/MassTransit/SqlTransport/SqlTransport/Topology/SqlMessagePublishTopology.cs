#nullable enable
namespace MassTransit.SqlTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Configuration;
    using MassTransit.Topology;


    public class SqlMessagePublishTopology<TMessage> :
        MessagePublishTopology<TMessage>,
        ISqlMessagePublishTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly List<ISqlMessagePublishTopology> _implementedMessageTypes;
        readonly SqlTopicConfigurator _topic;

        public SqlMessagePublishTopology(ISqlPublishTopology publishTopology, IMessageTopology<TMessage> messageTopology)
            : base(publishTopology)
        {
            var exchangeName = messageTopology.EntityName;

            _topic = new SqlTopicConfigurator(exchangeName);

            _implementedMessageTypes = new List<ISqlMessagePublishTopology>();
        }

        public Topic Topic => _topic;

        public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
        {
            if (Exclude)
                return;

            var topicHandle = builder.CreateTopic(_topic.TopicName);

            if (builder.Topic != null)
                builder.CreateTopicSubscription(builder.Topic, topicHandle);
            else
                builder.Topic = topicHandle;

            foreach (var configurator in _implementedMessageTypes)
                configurator.Apply(builder);
        }

        public override bool TryGetPublishAddress(Uri baseAddress, [NotNullWhen(true)] out Uri? publishAddress)
        {
            publishAddress = _topic.GetEndpointAddress(baseAddress);
            return true;
        }

        public BrokerTopology GetBrokerTopology()
        {
            var builder = new PublishEndpointBrokerTopologyBuilder();

            Apply(builder);

            return builder.BuildBrokerTopology();
        }

        public SendSettings GetSendSettings(Uri hostAddress)
        {
            return new QueueSendSettings(_topic.GetEndpointAddress(hostAddress));
        }

        public void AddImplementedMessageConfigurator<T>(ISqlMessagePublishTopologyConfigurator<T> configurator, bool direct)
            where T : class
        {
            var adapter = new ImplementedTypeAdapter<T>(configurator, direct);

            _implementedMessageTypes.Add(adapter);
        }


        class ImplementedTypeAdapter<T> :
            ISqlMessagePublishTopology
            where T : class
        {
            readonly ISqlMessagePublishTopologyConfigurator<T> _configurator;
            readonly bool _direct;

            public ImplementedTypeAdapter(ISqlMessagePublishTopologyConfigurator<T> configurator, bool direct)
            {
                _configurator = configurator;
                _direct = direct;
            }

            public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
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
