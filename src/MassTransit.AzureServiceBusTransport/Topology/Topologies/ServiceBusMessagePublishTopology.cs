namespace MassTransit.AzureServiceBusTransport.Topology.Topologies
{
    using System;
    using Builders;
    using Configuration;
    using Configuration.Configurators;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Microsoft.ServiceBus.Messaging;
    using Settings;
    using Transport;


    public class ServiceBusMessagePublishTopology<TMessage> :
        MessagePublishTopology<TMessage>,
        IServiceBusMessagePublishTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly IMessageTopology<TMessage> _messageTopology;
        readonly TopicConfigurator _topicConfigurator;
        readonly Lazy<TopicDescription> _topicDescription;

        public ServiceBusMessagePublishTopology(IMessageTopology<TMessage> messageTopology)
        {
            _messageTopology = messageTopology;

            _topicDescription = new Lazy<TopicDescription>(GetTopicDescription);

            _topicConfigurator = new TopicConfigurator(messageTopology.EntityName, TypeMetadataCache<TMessage>.IsTemporaryMessageType);
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

            var sendSettings = new TopicSendSettings(description);

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

        bool? IMessageEntityConfigurator.EnableExpress
        {
            set => _topicConfigurator.EnableExpress = value;
        }

        bool? IMessageEntityConfigurator.EnablePartitioning
        {
            set => _topicConfigurator.EnablePartitioning = value;
        }

        bool? IMessageEntityConfigurator.IsAnonymousAccessible
        {
            set => _topicConfigurator.IsAnonymousAccessible = value;
        }

        long? IMessageEntityConfigurator.MaxSizeInMegabytes
        {
            set => _topicConfigurator.MaxSizeInMegabytes = value;
        }

        bool? IMessageEntityConfigurator.RequiresDuplicateDetection
        {
            set => _topicConfigurator.RequiresDuplicateDetection = value;
        }

        bool? IMessageEntityConfigurator.SupportOrdering
        {
            set => _topicConfigurator.SupportOrdering = value;
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

            if (builder.Topic == null)
                builder.Topic = topicHandle;

            // TODO add publisher exchange bindings for message inherited types, similar to RMQ
            /*

            builder.CreateTopicSubscription(builder.Topic, topicHandle, new SubscriptionConfigurator(builder.Topic.Topic.TopicDescription.Path, topicHandle));

                builder.ExchangeBind(builder.Exchange, exchangeHandle, "", new Dictionary<string, object>());
            else
                builder.Exchange = exchangeHandle;

            foreach (IServiceBusMessagePublishTopology configurator in _implementedMessageTypes)
                configurator.Apply(builder);
*/
        }

        TopicDescription GetTopicDescription()
        {
            return _topicConfigurator.GetTopicDescription();
        }
    }
}
