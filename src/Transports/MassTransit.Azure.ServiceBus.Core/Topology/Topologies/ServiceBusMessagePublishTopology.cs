namespace MassTransit.Azure.ServiceBus.Core.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using global::Azure.Messaging.ServiceBus.Administration;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Settings;
    using Transport;


    public class ServiceBusMessagePublishTopology<TMessage> :
        MessagePublishTopology<TMessage>,
        IServiceBusMessagePublishTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly IList<IServiceBusMessagePublishTopology> _implementedMessageTypes;
        readonly IMessageTopology<TMessage> _messageTopology;
        readonly IServiceBusPublishTopology _publishTopology;
        readonly TopicConfigurator _topicConfigurator;
        readonly Lazy<CreateTopicOptions> _createTopicOptions;

        public ServiceBusMessagePublishTopology(IMessageTopology<TMessage> messageTopology, IServiceBusPublishTopology publishTopology)
        {
            _messageTopology = messageTopology;
            _publishTopology = publishTopology;

            _createTopicOptions = new Lazy<CreateTopicOptions>(() => _topicConfigurator.GetCreateTopicOptions());

            _topicConfigurator = new TopicConfigurator(messageTopology.EntityName, TypeMetadataCache<TMessage>.IsTemporaryMessageType);
            _implementedMessageTypes = new List<IServiceBusMessagePublishTopology>();
        }

        public override bool TryGetPublishAddress(Uri baseAddress, out Uri publishAddress)
        {
            publishAddress = new ServiceBusEndpointAddress(new Uri(baseAddress.GetLeftPart(UriPartial.Authority)), _topicConfigurator.FullPath);
            return true;
        }

        public CreateTopicOptions CreateTopicOptions => _createTopicOptions.Value;

        public SendSettings GetSendSettings()
        {
            var createTopicOptions = _topicConfigurator.GetCreateTopicOptions();

            var builder = new PublishEndpointBrokerTopologyBuilder(_publishTopology);

            Apply(builder);

            return new TopicSendSettings(createTopicOptions, builder.BuildBrokerTopology());
        }

        public SubscriptionConfigurator GetSubscriptionConfigurator(string subscriptionName)
        {
            return new SubscriptionConfigurator(CreateTopicOptions.Name, _publishTopology.FormatSubscriptionName(subscriptionName));
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
            if (Exclude)
                return;

            var topicHandle = builder.CreateTopic(_createTopicOptions.Value);

            builder.Topic = topicHandle;

            foreach (var configurator in _implementedMessageTypes)
                configurator.Apply(builder);
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
