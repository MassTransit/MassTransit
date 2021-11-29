namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using Azure.Messaging.ServiceBus.Administration;
    using Configuration;
    using MassTransit.Topology;


    public class ServiceBusMessagePublishTopology<TMessage> :
        MessagePublishTopology<TMessage>,
        IServiceBusMessagePublishTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly Lazy<CreateTopicOptions> _createTopicOptions;
        readonly IList<IServiceBusMessagePublishTopology> _implementedMessageTypes;
        readonly IMessageTopology<TMessage> _messageTopology;
        readonly IServiceBusPublishTopology _publishTopology;
        readonly ServiceBusTopicConfigurator _topicConfigurator;

        public ServiceBusMessagePublishTopology(IMessageTopology<TMessage> messageTopology, IServiceBusPublishTopology publishTopology)
        {
            _messageTopology = messageTopology;
            _publishTopology = publishTopology;

            _createTopicOptions = new Lazy<CreateTopicOptions>(() => _topicConfigurator.GetCreateTopicOptions());

            _topicConfigurator = new ServiceBusTopicConfigurator(messageTopology.EntityName, MessageTypeCache<TMessage>.IsTemporaryMessageType);
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

        public ServiceBusSubscriptionConfigurator GetSubscriptionConfigurator(string subscriptionName)
        {
            return new ServiceBusSubscriptionConfigurator(CreateTopicOptions.Name, _publishTopology.FormatSubscriptionName(subscriptionName));
        }

        string IServiceBusMessageEntityConfigurator.Path => _topicConfigurator.Path;

        string IServiceBusMessageEntityConfigurator.BasePath
        {
            get => _topicConfigurator.BasePath;
            set => _topicConfigurator.BasePath = value;
        }

        string IServiceBusMessageEntityConfigurator.FullPath => _topicConfigurator.FullPath;

        TimeSpan? IServiceBusMessageEntityConfigurator.DuplicateDetectionHistoryTimeWindow
        {
            set => _topicConfigurator.DuplicateDetectionHistoryTimeWindow = value;
        }

        bool? IServiceBusMessageEntityConfigurator.EnablePartitioning
        {
            set => _topicConfigurator.EnablePartitioning = value;
        }

        long? IServiceBusMessageEntityConfigurator.MaxSizeInMB
        {
            set => _topicConfigurator.MaxSizeInMB = value;
        }

        bool? IServiceBusMessageEntityConfigurator.RequiresDuplicateDetection
        {
            set => _topicConfigurator.RequiresDuplicateDetection = value;
        }

        void IServiceBusMessageEntityConfigurator.EnableDuplicateDetection(TimeSpan historyTimeWindow)
        {
            _topicConfigurator.EnableDuplicateDetection(historyTimeWindow);
        }

        TimeSpan? IServiceBusEntityConfigurator.AutoDeleteOnIdle
        {
            set => _topicConfigurator.AutoDeleteOnIdle = value;
        }

        TimeSpan? IServiceBusEntityConfigurator.DefaultMessageTimeToLive
        {
            set => _topicConfigurator.DefaultMessageTimeToLive = value;
        }

        bool? IServiceBusEntityConfigurator.EnableBatchedOperations
        {
            set => _topicConfigurator.EnableBatchedOperations = value;
        }

        string IServiceBusEntityConfigurator.UserMetadata
        {
            set => _topicConfigurator.UserMetadata = value;
        }

        bool? IServiceBusTopicConfigurator.EnableFilteringMessagesBeforePublishing
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
