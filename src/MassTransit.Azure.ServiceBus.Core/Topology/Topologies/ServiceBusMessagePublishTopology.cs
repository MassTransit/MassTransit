namespace MassTransit.Azure.ServiceBus.Core.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configuration;
    using Configuration.Configurators;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Microsoft.Azure.ServiceBus.Management;
    using Settings;
    using Transport;


    public class ServiceBusMessagePublishTopology<TMessage> :
        MessagePublishTopology<TMessage>,
        IServiceBusMessagePublishTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly IList<IServiceBusMessagePublishTopology> _implementedMessageTypes;
        readonly IMessageTopology<TMessage> _messageTopology;
        readonly TopicConfigurator _topicConfigurator;
        readonly Lazy<TopicDescription> _topicDescription;

        public ServiceBusMessagePublishTopology(IMessageTopology<TMessage> messageTopology)
        {
            _messageTopology = messageTopology;

            _topicDescription = new Lazy<TopicDescription>(GetTopicDescription);

            _topicConfigurator = new TopicConfigurator(messageTopology.EntityName, TypeMetadataCache<TMessage>.IsTemporaryMessageType);
            _implementedMessageTypes = new List<IServiceBusMessagePublishTopology>();
        }

        public override bool TryGetPublishAddress(Uri baseAddress, out Uri publishAddress)
        {
            publishAddress = new ServiceBusEndpointAddress(baseAddress, _messageTopology.EntityName);
            return true;
        }

        public TopicDescription TopicDescription => _topicDescription.Value;

        public SendSettings GetSendSettings()
        {
            var description = GetTopicDescription();

            var builder = new PublishEndpointBrokerTopologyBuilder();

            Apply(builder);

            var sendSettings = new TopicSendSettings(description, builder.BuildBrokerTopology());

            return sendSettings;
        }

        string IMessageEntityConfigurator.Path => _topicConfigurator.Path;

        string IMessageEntityConfigurator.BasePath
        {
            get => _topicConfigurator.BasePath;
            set => _topicConfigurator.BasePath = value;
        }

        string IMessageEntityConfigurator.FullPath => _topicConfigurator.FullPath;

        TimeSpan? IMessageEntityConfigurator.DuplicateDetectionHistoryTimeWindow
        {
            set => _topicConfigurator.DuplicateDetectionHistoryTimeWindow = value;
        }

        bool? IMessageEntityConfigurator.EnablePartitioning
        {
            set => _topicConfigurator.EnablePartitioning = value;
        }

        long? IMessageEntityConfigurator.MaxSizeInMB
        {
            set => _topicConfigurator.MaxSizeInMB = value;
        }

        bool? IMessageEntityConfigurator.RequiresDuplicateDetection
        {
            set => _topicConfigurator.RequiresDuplicateDetection = value;
        }

        void IMessageEntityConfigurator.EnableDuplicateDetection(TimeSpan historyTimeWindow)
        {
            _topicConfigurator.EnableDuplicateDetection(historyTimeWindow);
        }

        TimeSpan? IEntityConfigurator.AutoDeleteOnIdle
        {
            set => _topicConfigurator.AutoDeleteOnIdle = value;
        }

        TimeSpan? IEntityConfigurator.DefaultMessageTimeToLive
        {
            set => _topicConfigurator.DefaultMessageTimeToLive = value;
        }

        bool? IEntityConfigurator.EnableBatchedOperations
        {
            set => _topicConfigurator.EnableBatchedOperations = value;
        }

        string IEntityConfigurator.UserMetadata
        {
            set => _topicConfigurator.UserMetadata = value;
        }

        bool? ITopicConfigurator.EnableFilteringMessagesBeforePublishing
        {
            set => _topicConfigurator.EnableFilteringMessagesBeforePublishing = value;
        }

        public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
        {
            var topicHandle = builder.CreateTopic(_topicDescription.Value);

            builder.Topic = topicHandle;

            foreach (IServiceBusMessagePublishTopology configurator in _implementedMessageTypes)
                configurator.Apply(builder);
        }

        TopicDescription GetTopicDescription()
        {
            return _topicConfigurator.GetTopicDescription();
        }

        public void AddImplementedMessageConfigurator<T>(IServiceBusMessagePublishTopologyConfigurator<T> configurator, bool direct)
            where T : class
        {
            var adapter = new ImplementedTypeAdapter<T>(configurator, direct);

            _implementedMessageTypes.Add(adapter);
        }


        class ImplementedTypeAdapter<T> :
            IServiceBusMessagePublishTopology
            where T : class
        {
            readonly IServiceBusMessagePublishTopologyConfigurator<T> _configurator;
            readonly bool _direct;

            public ImplementedTypeAdapter(IServiceBusMessagePublishTopologyConfigurator<T> configurator, bool direct)
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
